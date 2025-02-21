using System.Text.Json;
using AppCommon.DTOs;
using AppCommon.GlobalHelpers;
using Application.Common.Helpers;
using AppWorkflow.Common.DTO;
using AppWorkflow.Services.Interfaces;
using Module;
using Module.Domain.Schema;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;

namespace Application.Services.DefaultSetupService
{
    public class DefaultWorkspaceSetupService : IDefaultWorkspaceSetupService
    {
        private readonly ModuleDbContext _context;
        private readonly IWorkflowManagementService _workflowManagementService;
        public DefaultWorkspaceSetupService(ModuleDbContext context, IWorkflowManagementService workflowManagementService)
        {
            _context = context;
            _workflowManagementService = workflowManagementService;
        }

        public List<AppAction> AddDefaultActions(Guid workspaceId, Guid userId)
        {
            var workspace = _context.Workspaces.Find(workspaceId);
            if (workspace == null) return new();

            var now = DateTime.UtcNow;

            var defaultActions = new List<AppAction>
            {
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "اضافة", En = "Create" },
                    Description = new TranslatableValue {
                        Ar = "إنشاء مساحة العمل",
                        En = "Create workspace"
                    },
                    Type = ActionType.Create,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "تعديل", En = "Update" },
                    Description = new TranslatableValue {
                        Ar = "تحديث مساحة العمل",
                        En = "Update workspace"
                    },
                    Type = ActionType.Update,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "حذف", En = "Delete" },
                    Description = new TranslatableValue {
                        Ar = "حذف مساحة العمل",
                        En = "Delete workspace"
                    },
                    Type = ActionType.Delete,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    WorkspaceId = workspaceId,
                    Name = new() { Ar = "معاينة", En = "Read" },
                    Description = new TranslatableValue {
                        Ar = "قراءة مساحة العمل",
                        En = "Read workspace"
                    },
                    Type = ActionType.Read,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                }
            };

            _context.AppActions.AddRange(defaultActions);
            _context.SaveChanges();
            return defaultActions;
        }

        public List<Property> AddDefaultProperties(Guid workspaceId, Guid userId)
        {
            var workspace = _context.Workspaces.Find(workspaceId);
            if (workspace == null) return new();

            var now = DateTime.UtcNow;

            var defaultProperties = new List<Property>
            {
                new() {
                    Title = SharedPropertyConfigurations.Common.CreatedAtTitle,
                    Key = workspace.Key + "_" + SharedPropertyConfigurations.Common.CreatedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.CreatedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ إنشاء مساحة العمل",
                        En = "Date the workspace was created"
                    },
                    ViewType = SharedPropertyConfigurations.Common.CreatedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.CreatedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.CreatedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.CreatedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.CreatedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.CreatedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.CreatedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "المستخدم الذي أنشأ مساحة العمل",
                        En = "User who created the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.CreatedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.CreatedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.CreatedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.CreatedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.UpdatedAtTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.UpdatedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.UpdatedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ آخر تحديث",
                        En = "Last update date"
                    },
                    ViewType = SharedPropertyConfigurations.Common.UpdatedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.UpdatedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.UpdatedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.UpdatedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.UpdatedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.UpdatedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.UpdatedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "آخر مستخدم قام بتحديث مساحة العمل",
                        En = "Last user who updated the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.UpdatedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.UpdatedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.UpdatedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.UpdatedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.DeletedAtTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.DeletedAt.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.DeletedAt.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "تاريخ حذف مساحة العمل",
                        En = "Date the workspace was deleted"
                    },
                    ViewType = SharedPropertyConfigurations.Common.DeletedAt.ViewType,
                    DataType = SharedPropertyConfigurations.Common.DeletedAt.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.DeletedAt.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.DeletedAt.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                },
                new() {
                    Title = SharedPropertyConfigurations.Common.DeletedByTitle,
                    Key = workspace.Key + "_" +SharedPropertyConfigurations.Common.DeletedBy.NormalizedKey.ToLower(),
                    NormalizedKey = SharedPropertyConfigurations.Common.DeletedBy.NormalizedKey,
                    Description = new TranslatableValue {
                        Ar = "المستخدم الذي قام بحذف مساحة العمل",
                        En = "User who deleted the workspace"
                    },
                    ViewType = SharedPropertyConfigurations.Common.DeletedBy.ViewType,
                    DataType = SharedPropertyConfigurations.Common.DeletedBy.DataType,
                    SystemPropertyPath = SharedPropertyConfigurations.Common.DeletedBy.SystemPropertyPath,
                    IsSystem = true,
                    IsInternal = false,
                    Order = SharedPropertyConfigurations.Common.DeletedBy.DefaultOrder,
                    WorkspaceId = workspaceId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                }
            };

            _context.Properties.AddRange(defaultProperties);
            _context.SaveChanges();
            return defaultProperties;
        }

        public void AddDefaultActionsForWorkspaceModules(Guid workspaceId, Guid userId)
        {
            var existingWsmIds = _context.WorkspaceModules
                .Where(wsm => wsm.WorkspaceId == workspaceId)
                .Select(wsm => wsm.Id)
                .ToList();

            var orphanedActions = _context.AppActions
                .Where(a => a.WorkspaceId == workspaceId
                         && a.WorkspaceModuleId.HasValue
                         && !existingWsmIds.Contains(a.WorkspaceModuleId.Value))
                .ToList();

            if (orphanedActions.Count != 0)
            {
                _context.AppActions.RemoveRange(orphanedActions);
                _context.SaveChanges();
            }

            var now = DateTime.UtcNow;

            var existingActions = _context.AppActions
                .Where(a => a.WorkspaceId == workspaceId
                         && a.WorkspaceModuleId.HasValue
                         && existingWsmIds.Contains(a.WorkspaceModuleId.Value))
                .Select(a => new { a.WorkspaceModuleId, a.Type })
                .ToList();

            var actionsToAdd = new List<AppAction>();

            var actionTranslations = new Dictionary<ActionType, (string enName, string arName, string enDesc, string arDesc)>
            {
                {
                    ActionType.Create,
                    (
                        enName: "Create Record",
                        arName: "إنشاء سجل",
                        enDesc: "Default Create action",
                        arDesc: "الإجراء الافتراضي لإنشاء سجل"
                    )
                },
                {
                    ActionType.Read,
                    (
                        enName: "Read Record",
                        arName: "عرض سجل",
                        enDesc: "Default Read action",
                        arDesc: "الإجراء الافتراضي لعرض سجل"
                    )
                },
                {
                    ActionType.Update,
                    (
                        enName: "Update Record",
                        arName: "تعديل سجل",
                        enDesc: "Default Update action",
                        arDesc: "الإجراء الافتراضي لتعديل سجل"
                    )
                },
                {
                    ActionType.Delete,
                    (
                        enName: "Delete Record",
                        arName: "حذف سجل",
                        enDesc: "Default Delete action",
                        arDesc: "الإجراء الافتراضي لحذف سجل"
                    )
                },
            };

            foreach (var wsmId in existingWsmIds)
            {
                foreach (var actionType in actionTranslations.Keys)
                {
                    bool alreadyExists = existingActions.Any(e =>
                        e.WorkspaceModuleId == wsmId && e.Type == actionType);
                    if (alreadyExists) continue;

                    var (enName, arName, enDesc, arDesc) = actionTranslations[actionType];

                    actionsToAdd.Add(new AppAction
                    {
                        WorkspaceId = workspaceId,
                        WorkspaceModuleId = wsmId,
                        Type = actionType,
                        Name = new TranslatableValue
                        {
                            En = enName,
                            Ar = arName
                        },
                        Description = new TranslatableValue
                        {
                            En = enDesc,
                            Ar = arDesc
                        },
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            if (actionsToAdd.Count != 0)
            {
                _context.AppActions.AddRange(actionsToAdd);
                _context.SaveChanges();
            }
        }

        public void AddDefaultWorkflows(List<AppAction> actions, Guid userId)
        {
            foreach (var appAction in actions.Where(x=>x.Type!= ActionType.Read))
            {
                var workflowDto = new CreateWorkflowDto
                {
                    Name = new TranslatableValue
                    {
                        En = $"{appAction.Name.En} Workflow",
                        Ar = $"{appAction.Name.Ar} سير العمل"
                    }.AsText(),
                    Metadata = new Dictionary<string, string>{{"ModuleType","Action"},{"ModuleId",$"{appAction.Id}"}},
                    Description = $"Workflow for {appAction.Name.En} action",
                    Steps = new List<CreateWorkflowStepDto>
                    {
                        new CreateWorkflowStepDto
                        {
                            Name = "Initial Step",
                            Type = appAction.Type.GetWorkFlowActionType(),
                            Configuration = JsonDocument.Parse(JsonSerializer.Serialize(new { ActionId = appAction.Id })),
                        }
                    },
                 
                };

                _workflowManagementService.CreateWorkflowAsync(workflowDto).Wait();
            }
        }
    }
}
