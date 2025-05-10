using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppWorkflow.Common.DTO;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Triggers;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BE_BATATA.Tests.Integration
{    public class MockWorkflowServices
    {
        public Mock<IWorkflowVersionManager> VersionManager { get; private set; }
        public Mock<IWorkflowMigrationService> MigrationService { get; private set; }
        public Mock<IWorkflowMonitoringService> MonitoringService { get; private set; }
        public Mock<IWorkflowRecoveryService> RecoveryService { get; private set; }
        public Mock<IWorkflowTriggerHandler> EventTriggerHandler { get; private set; }
        public Mock<IWorkflowTriggerHandler> ScheduledTriggerHandler { get; private set; }
        public Mock<IWorkflowTriggerHandler> ManualTriggerHandler { get; private set; }
        public Mock<IWorkflowTriggerHandler> ApiTriggerHandler { get; private set; }
        
        public MockWorkflowServices()
        {
            SetupVersionManager();
            SetupMigrationService();
            SetupMonitoringService();
            SetupRecoveryService();
            SetupTriggerHandlers();
        }
        
        private void SetupVersionManager()
        {
            VersionManager = new Mock<IWorkflowVersionManager>();
            
            // Setup CreateNewVersionAsync
            VersionManager
                .Setup(vm => vm.CreateNewVersionAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid workflowId) => new WorkflowVersion
                {
                    Id = workflowId,
                    Version = 1,
                    IsLatestVersion = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                });
                
            // Setup GetLatestVersionAsync
            VersionManager
                .Setup(vm => vm.GetLatestVersionAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid workflowId) => new WorkflowVersion
                {
                    Id = workflowId,
                    Version = 1,
                    IsLatestVersion = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                });
                
            // Setup GetVersionHistoryAsync
            VersionManager
                .Setup(vm => vm.GetVersionHistoryAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid workflowId) => new List<WorkflowVersion>
                {
                    new WorkflowVersion
                    {
                        Id = workflowId,
                        Version = 1,
                        IsLatestVersion = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        CreatedBy = "system"
                    },
                    new WorkflowVersion
                    {
                        Id = workflowId,
                        Version = 2,
                        IsLatestVersion = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "system"
                    }
                });
                
            // Setup IsLatestVersionAsync
            VersionManager
                .Setup(vm => vm.IsLatestVersionAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync((Guid workflowId, int version) => version == 1);
                
            // Setup GetVersionMetricsAsync
            VersionManager
                .Setup(vm => vm.GetVersionMetricsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync((Guid workflowId, int version) => new WorkflowVersionMetrics
                {
                    TotalInstances = 10,
                    ActiveInstances = 5,
                    CompletedInstances = 3,
                    FailedInstances = 2,
                    AverageCompletionTimeSeconds = 120,
                    SuccessRate = 60
                });
        }
        
        private void SetupMigrationService()
        {
            MigrationService = new Mock<IWorkflowMigrationService>();
            
            // Setup ValidateMigrationAsync
            MigrationService
                .Setup(ms => ms.ValidateMigrationAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync((Guid workflowId, int sourceVersion, int targetVersion, List<Guid> instanceIds) => 
                    new WorkflowMigrationValidationResult
                    {
                        IsValid = true,
                        ImpactAssessment = "No breaking changes detected",
                        MigratableInstances = instanceIds.Count,
                        FieldMappings = new Dictionary<string, string>(),
                        Warnings = new List<string>()
                    });
                    
            // Setup MigrateInstancesAsync
            MigrationService
                .Setup(ms => ms.MigrateInstancesAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync((Guid workflowId, int sourceVersion, int targetVersion, List<Guid> instanceIds) => 
                    new WorkflowMigrationResult
                    {
                        SuccessCount = instanceIds.Count,
                        FailedCount = 0,
                        TotalCount = instanceIds.Count,
                        FailedInstanceIds = new List<Guid>(),
                        Warnings = new List<string>()
                    });
                    
            // Setup GetInstancesForVersionAsync
            MigrationService
                .Setup(ms => ms.GetInstancesForVersionAsync(It.IsAny<int>()))
                .ReturnsAsync((int version) => new List<WorkflowInstanceDetails>
                {
                    new WorkflowInstanceDetails
                    {
                        InstanceId = Guid.NewGuid(),
                        WorkflowId = Guid.NewGuid(),
                        Version = version,
                        Status = "Active",
                        CurrentState = "Review",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        LastUpdatedAt = DateTime.UtcNow
                    }
                });
                
            // Setup GetCompatibleVersionsAsync
            MigrationService
                .Setup(ms => ms.GetCompatibleVersionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync((Guid workflowId, int version) => new List<int> { version + 1, version + 2 });
                
            // Setup MigrateInstanceAsync
            MigrationService
                .Setup(ms => ms.MigrateInstanceAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync((Guid instanceId, int targetVersion) => 
                    new WorkflowMigrationResult
                    {
                        SuccessCount = 1,
                        FailedCount = 0,
                        TotalCount = 1,
                        FailedInstanceIds = new List<Guid>(),
                        Warnings = new List<string>()
                    });
        }
        
        private void SetupMonitoringService()
        {
            MonitoringService = new Mock<IWorkflowMonitoringService>();
            
            // Setup GetDashboardSummaryAsync
            MonitoringService
                .Setup(ms => ms.GetDashboardSummaryAsync())
                .ReturnsAsync(() => new WorkflowMonitoringSummary 
                {
                    TotalWorkflows = 10,
                    TotalInstances = 50,
                    ActiveInstances = 20,
                    CompletedInstances = 25,
                    FailedInstances = 5,
                    SuspendedInstances = 0,
                    TerminatedInstances = 0,
                    InstancesByStatus = new Dictionary<string, int>
                    {
                        ["Active"] = 20,
                        ["Completed"] = 25,
                        ["Failed"] = 5
                    },
                    InstancesToday = 15,
                    WorkflowsCreatedToday = 2
                });
                
            // Setup GetInstanceDetailsAsync
            MonitoringService
                .Setup(ms => ms.GetInstanceDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid instanceId) => new WorkflowInstanceDetails
                {
                    InstanceId = instanceId,
                    WorkflowId = Guid.NewGuid(),
                    Version = "1.0",
                    Status = "Active",
                    CurrentState = "Review",
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    LastUpdatedAt = DateTime.UtcNow,
                    Variables = new Dictionary<string, object>(),
                    CompletedSteps = new List<string> { "Initialize", "Validate" },
                    CurrentStepStartedAt = DateTime.UtcNow.AddMinutes(-10),
                    Errors = new List<string>()
                });
                
            // Setup GetWorkflowPerformanceMetricsAsync
            MonitoringService
                .Setup(ms => ms.GetWorkflowPerformanceMetricsAsync(It.IsAny<Guid?>()))
                .ReturnsAsync((Guid? workflowId) => new WorkflowPerformanceMetrics
                {
                    TotalExecutions = 100,
                    SuccessRate = 90,
                    AverageExecutionTime = 120,
                    P95ExecutionTime = 180,
                    StepMetrics = new Dictionary<string, StepPerformanceMetric>
                    {
                        ["Initialize"] = new StepPerformanceMetric { AverageExecutionTime = 10, FailureRate = 1 },
                        ["Process"] = new StepPerformanceMetric { AverageExecutionTime = 50, FailureRate = 5 },
                        ["Complete"] = new StepPerformanceMetric { AverageExecutionTime = 20, FailureRate = 2 }
                    }
                });
                
            // Setup GetActiveWorkflowInstancesAsync
            MonitoringService
                .Setup(ms => ms.GetActiveWorkflowInstancesAsync(It.IsAny<int>()))
                .ReturnsAsync((int maxResults) => new List<WorkflowInstanceSummary>
                {
                    new WorkflowInstanceSummary
                    {
                        InstanceId = Guid.NewGuid(),
                        WorkflowName = "Sample Workflow",
                        Status = "Active",
                        StartedAt = DateTime.UtcNow.AddHours(-2),
                        LastUpdatedAt = DateTime.UtcNow.AddMinutes(-30),
                        CurrentStep = "Review"
                    }
                });
                
            // Setup GetRecentWorkflowInstancesAsync
            MonitoringService
                .Setup(ms => ms.GetRecentWorkflowInstancesAsync(It.IsAny<int>()))
                .ReturnsAsync((int maxResults) => new List<WorkflowInstanceSummary>
                {
                    new WorkflowInstanceSummary
                    {
                        InstanceId = Guid.NewGuid(),
                        WorkflowName = "Sample Workflow",
                        Status = "Completed",
                        StartedAt = DateTime.UtcNow.AddDays(-1),
                        LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                        CurrentStep = "Completed"
                    }
                });
                
            // Setup SearchWorkflowInstancesAsync
            MonitoringService
                .Setup(ms => ms.SearchWorkflowInstancesAsync(It.IsAny<WorkflowSearchCriteria>()))
                .ReturnsAsync((WorkflowSearchCriteria criteria) => new List<WorkflowInstanceSummary>
                {
                    new WorkflowInstanceSummary
                    {
                        InstanceId = Guid.NewGuid(),
                        WorkflowName = "Sample Workflow",
                        Status = criteria.Status?.ToString() ?? "Active",
                        StartedAt = DateTime.UtcNow.AddDays(-1),
                        LastUpdatedAt = DateTime.UtcNow,
                        CurrentStep = "Process"
                    }
                });
        }
        
        private void SetupRecoveryService()
        {
            RecoveryService = new Mock<IWorkflowRecoveryService>();
            
            // Setup RecoverWorkflowInstanceAsync
            RecoveryService
                .Setup(rs => rs.RecoverWorkflowInstanceAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync((Guid instanceId, bool useLatestCheckpoint) => new RecoveryResult
                {
                    Success = true,
                    InstanceId = instanceId,
                    Message = "Successfully recovered workflow instance",
                    NewStatus = "Active",
                    RecoveredToStep = "Review"
                });
                
            // Setup CreateCheckpointAsync
            RecoveryService
                .Setup(rs => rs.CreateCheckpointAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((Guid instanceId, string checkpointName) => new CheckpointResult
                {
                    Success = true,
                    InstanceId = instanceId,
                    CheckpointId = Guid.NewGuid(),
                    CheckpointName = checkpointName,
                    CreatedAt = DateTime.UtcNow
                });
                
            // Setup GetCheckpointsAsync
            RecoveryService
                .Setup(rs => rs.GetCheckpointsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid instanceId) => new List<CheckpointInfo>
                {
                    new CheckpointInfo
                    {
                        CheckpointId = Guid.NewGuid(),
                        InstanceId = instanceId,
                        Name = "Before Processing",
                        CreatedAt = DateTime.UtcNow.AddHours(-2),
                        StepName = "Initialize"
                    },
                    new CheckpointInfo
                    {
                        CheckpointId = Guid.NewGuid(),
                        InstanceId = instanceId,
                        Name = "After Validation",
                        CreatedAt = DateTime.UtcNow.AddHours(-1),
                        StepName = "Validate"
                    }
                });
                
            // Setup RestoreFromCheckpointAsync
            RecoveryService
                .Setup(rs => rs.RestoreFromCheckpointAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid instanceId, Guid checkpointId) => new RecoveryResult
                {
                    Success = true,
                    InstanceId = instanceId,
                    Message = "Successfully restored from checkpoint",
                    NewStatus = "Active",
                    RecoveredToStep = "Validate"
                });
                
            // Setup RetryStepAsync
            RecoveryService
                .Setup(rs => rs.RetryStepAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((Guid instanceId, string stepName) => new RecoveryResult
                {
                    Success = true,
                    InstanceId = instanceId,
                    Message = $"Successfully retried step {stepName}",
                    NewStatus = "Active",
                    RecoveredToStep = stepName
                });
        }
        
        private void SetupTriggerHandlers()
        {
            // Setup Event Trigger Handler
            EventTriggerHandler = new Mock<IWorkflowTriggerHandler>();
            EventTriggerHandler.Setup(h => h.TriggerType).Returns("Event");
            EventTriggerHandler
                .Setup(h => h.CanHandle(It.Is<string>(s => s == "Event")))
                .Returns(true);
            EventTriggerHandler
                .Setup(h => h.HandleTriggerAsync(It.IsAny<TriggerContext>()))
                .ReturnsAsync((TriggerContext ctx) => true);
            EventTriggerHandler
                .Setup(h => h.RegisterTriggerAsync(It.IsAny<TriggerConfiguration>()))
                .Returns(Task.CompletedTask);
            EventTriggerHandler
                .Setup(h => h.UnregisterTriggerAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            
            // Setup Scheduled Trigger Handler
            ScheduledTriggerHandler = new Mock<IWorkflowTriggerHandler>();
            ScheduledTriggerHandler.Setup(h => h.TriggerType).Returns("Scheduled");
            ScheduledTriggerHandler
                .Setup(h => h.CanHandle(It.Is<string>(s => s == "Scheduled")))
                .Returns(true);
            ScheduledTriggerHandler
                .Setup(h => h.HandleTriggerAsync(It.IsAny<TriggerContext>()))
                .ReturnsAsync((TriggerContext ctx) => true);
            ScheduledTriggerHandler
                .Setup(h => h.RegisterTriggerAsync(It.IsAny<TriggerConfiguration>()))
                .Returns(Task.CompletedTask);
            ScheduledTriggerHandler
                .Setup(h => h.UnregisterTriggerAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
                
            // Setup Manual Trigger Handler
            ManualTriggerHandler = new Mock<IWorkflowTriggerHandler>();
            ManualTriggerHandler.Setup(h => h.TriggerType).Returns("Manual");
            ManualTriggerHandler
                .Setup(h => h.CanHandle(It.Is<string>(s => s == "Manual")))
                .Returns(true);
            ManualTriggerHandler
                .Setup(h => h.HandleTriggerAsync(It.IsAny<TriggerContext>()))
                .ReturnsAsync((TriggerContext ctx) => true);
            ManualTriggerHandler
                .Setup(h => h.RegisterTriggerAsync(It.IsAny<TriggerConfiguration>()))
                .Returns(Task.CompletedTask);
            ManualTriggerHandler
                .Setup(h => h.UnregisterTriggerAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
                
            // Setup API Trigger Handler
            ApiTriggerHandler = new Mock<IWorkflowTriggerHandler>();
            ApiTriggerHandler.Setup(h => h.TriggerType).Returns("Api");
            ApiTriggerHandler
                .Setup(h => h.CanHandle(It.Is<string>(s => s == "Api")))
                .Returns(true);
            ApiTriggerHandler
                .Setup(h => h.HandleTriggerAsync(It.IsAny<TriggerContext>()))
                .ReturnsAsync((TriggerContext ctx) => true);
            ApiTriggerHandler
                .Setup(h => h.RegisterTriggerAsync(It.IsAny<TriggerConfiguration>()))
                .Returns(Task.CompletedTask);
            ApiTriggerHandler
                .Setup(h => h.UnregisterTriggerAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
        }
          public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWorkflowVersionManager>(VersionManager.Object);
            services.AddSingleton<IWorkflowMigrationService>(MigrationService.Object);
            services.AddSingleton<IWorkflowMonitoringService>(MonitoringService.Object);
            services.AddSingleton<IWorkflowRecoveryService>(RecoveryService.Object);
            
            // Register trigger handlers
            services.AddSingleton<IWorkflowTriggerHandler>(EventTriggerHandler.Object);
            services.AddSingleton<IWorkflowTriggerHandler>(ScheduledTriggerHandler.Object);
            services.AddSingleton<IWorkflowTriggerHandler>(ManualTriggerHandler.Object);
            services.AddSingleton<IWorkflowTriggerHandler>(ApiTriggerHandler.Object);
        }
    }
}
