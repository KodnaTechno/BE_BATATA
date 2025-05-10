using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Queries;
using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Domain.Schema;
using AppWorkflow.Infrastructure.Data.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitController : ControllerBase
    {
        private readonly IStringLocalizer<object> _localization;
        private readonly ILogger<InitController> _logger;
        private readonly IMediator _mediator;

        public InitController(ILogger<InitController> logger, IStringLocalizer<object> localization, IMediator mediator)
        {
            _logger = logger;
            _localization = localization;
            _mediator = mediator;
        }

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var welcomeMessage = _localization["Welcome"].Value;
        //    return Ok(welcomeMessage);
        //}
        [HttpGet("GetMe")]
        public IActionResult GetMe([FromServices] WorkflowDbContext dbContext)
        {
            var stepId = Guid.NewGuid();
            Workflow workflow= new Workflow();
            workflow.Id = Guid.NewGuid();
            workflow.Name = "Test";
            workflow.Description = "Test";
            workflow.ModuleType = "WorkSpace";
            workflow.Status = WorkflowStatus.Active;
            workflow.InitialStepId = stepId;
            workflow.CreatedBy = "sds";
            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = "sds";
            workflow.CreatedAt= DateTime.Now;
            workflow.PropertiesKeys = new();
            workflow.RetryPolicy = new();
            workflow.Version = "1.0";
            workflow.TriggerConfigs = new List<AppWorkflow.Infrastructure.Data.Configurations.TriggerConfiguration>();
            workflow.Steps = new List<WorkflowStep>
            {
                new WorkflowStep
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    Description = "Test",
                    ActionType = "CreateModuleAction",
                    ActionConfiguration= JsonSerializer.SerializeToDocument(new 
                    {
                        ModuleType = "WorkSpace",
                        ModuleData = new WorkflowModuleData()
                    }),
                    CreatedAt= DateTime.Now,
                    CreatedBy = "sds",
                    RetryPolicy= new RetryPolicy(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "sds",
                    Metadata = new Dictionary<string, string>(),
                    Status = StepStatus.Pending,
                    Timeout = TimeSpan.FromMinutes(5),
                    Transitions= new List<StepTransition>(),
                    IsParallel = false

                }};
            workflow.IsLatestVersion = true;
            dbContext.Workflows.Add(workflow);
            dbContext.SaveChanges();

            return Ok(workflow);
           
        }
        [HttpGet("HitMe")]
        public async Task<IActionResult> HitMe([FromServices] IWorkflowEngine s,Guid id)
        {
            var module= new WorkflowModuleData();   
             await s.StartWorkflowAsync(id, module);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateWorkspaceCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet("{WorkspaceId}")]
        public async Task<IActionResult> GetW([FromRoute] GetWorkspaceQuery query)
          => throw new NotImplementedException();

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetWorkspacesQuery query)
         => Ok(await _mediator.Send(query));




    }
}
