using AppWorkflow.Infrastructure.Services.Actions;
using AppWorkflow.Infrastructure.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using Application.AppWorkflowActions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.AppWorkflowActions
{
    public class ApprovalAction : WorkflowActionBase
    {
        private readonly IApprovalTargetResolver _targetResolver;
        private readonly IApprovalService _approvalService;

        public ApprovalAction(
            ILogger<ApprovalAction> logger,
            IServiceProvider serviceProvider,
            IApprovalTargetResolver targetResolver,
            IApprovalService approvalService)
            : base(logger, serviceProvider)
        {
            _targetResolver = targetResolver;
            _approvalService = approvalService;
        }

        public override async Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            try
            {
                LogActionStart(context);

                // Deserialize configuration
                var config = await DeserializeConfiguration<ApprovalActionConfiguration>(context.ActionConfiguration);

                // Resolve target users
                var targetUsers = await ResolveTargetUsersAsync(context, config);

                // Create approval request
                var approvalRequest = new ApprovalRequest
                {
                    WorkflowDataId = context.WorkflowDataId,
                    StepDataId = context.StepId,
                    ApproverIds = targetUsers,
                    EditableProperties = config.EditableProperties,
                    ModuleData = context.ModuleData,
                    ExpiresAt = DateTime.UtcNow.AddHours(config.TimeoutInHours),
                    ApprovalMessage = config.ApprovalMessage
                };

                // Create and save approval request
                var approvalId = await _approvalService.CreateApprovalRequestAsync(approvalRequest);

                // Return result indicating workflow should pause/wait for approval
                return new ActionResult {
                    Success = true,
                    Message = "Approval request created",
                    OutputVariables = new Dictionary<string, object> {
                        ["approvalId"] = approvalId,
                        ["status"] = "pending",
                        ["targetUsers"] = targetUsers
                    },
                    Command = StepCommandType.Waiting
                };
            }
            catch (Exception ex)
            {
                LogActionError(context, ex);
                return new ActionResult {
                    Success = false,
                    Exception = ex,
                    Message = $"Failed to create approval request: {ex.Message}",
                    Command = StepCommandType.Failed
                };
            }
        }

        private async Task<List<string>> ResolveTargetUsersAsync(
            ActionContext context,
            ApprovalActionConfiguration config)
        {
            if (config.UseTargetResolver)
            {
                return await _targetResolver.ResolveTargetUsersAsync(context);
            }

            switch (config.Type.ToLower())
            {
                case "role":
                    return await _approvalService.GetUsersByRoleAsync(config.RoleId);
                case "group":
                    return await _approvalService.GetUsersByGroupAsync(config.GroupId);
                case "user":
                    return new List<string> { config.UserId };
                default:
                    throw new InvalidOperationException($"Invalid approval type: {config.Type}");
            }
        }

        public override async Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        {
            try
            {
                var approvalConfig = config.Deserialize<ApprovalActionConfiguration>();

                var errors = new List<string>();

                // Validate basic configuration
                if (string.IsNullOrEmpty(approvalConfig.Type))
                    errors.Add("Approval type is required");

                switch (approvalConfig.Type?.ToLower())
                {
                    case "role" when string.IsNullOrEmpty(approvalConfig.RoleId):
                        errors.Add("RoleId is required for role-based approval");
                        break;
                    case "group" when string.IsNullOrEmpty(approvalConfig.GroupId):
                        errors.Add("GroupId is required for group-based approval");
                        break;
                    case "user" when string.IsNullOrEmpty(approvalConfig.UserId):
                        errors.Add("UserId is required for user-based approval");
                        break;
                }

                // Validate editable properties
                if (approvalConfig.EditableProperties?.Any() == true)
                {
                    foreach (var prop in approvalConfig.EditableProperties)
                    {
                        if (string.IsNullOrEmpty(prop.PropertyName))
                            errors.Add("Property name is required for editable properties");
                    }
                }

                // Validate timeout
                if (approvalConfig.TimeoutInHours <= 0)
                    errors.Add("Timeout must be greater than 0");

                return await CreateValidationResult(!errors.Any(), errors: errors);
            }
            catch (Exception ex)
            {
                return await CreateValidationResult(false, errors: new List<string> { ex.Message });
            }
        }

        public override async Task<object> GetConfigurationSchemaAsync()
        {
            return new
            {
                type = "object",
                properties = new
                {
                    type = new { type = "string", enumz  = new[] { "Role", "Group", "User" }
},
                roleId = new { type = "string" },
                groupId = new { type = "string" },
                userId = new { type = "string" },
                useTargetResolver = new { type = "boolean" },
                targetResolverType = new { type = "string" },
                editableProperties = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            propertyName = new { type = "string" },
                            isRequired = new { type = "boolean" },
                            validationExpression = new { type = "string" },
                            isReadOnly = new { type = "boolean" }
                        }
                    }
                },
                approvalMessage = new { type = "string" },
                timeoutInHours = new { type = "integer", minimum = 1 },
                successTransition = new { type = "string" },
                rejectTransition = new { type = "string" }
            },
            required = new[] { "type", "timeoutInHours" }
        };
    }
}
