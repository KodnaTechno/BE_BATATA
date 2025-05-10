using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using AppCommon.DTOs;
using AppWorkflow.Common.DTO;
using Application.Features.WorkFlow.Command;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;

namespace BE_BATATA.Tests.Integration
{
    public class WorkflowErrorRecoveryTests : IClassFixture<TestApiFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;

        public WorkflowErrorRecoveryTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RetryFailedWorkflow_ShouldSucceed()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            
            // Act
            var response = await _client.PostAsync($"/api/workflow/recovery/retry/{instanceId}", null);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowCommandResult>>();
            result.Should().NotBeNull();            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task LoadWorkflowCheckpoint_ShouldReturnCheckpointData()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/recovery/checkpoint/{instanceId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, object>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task RestoreWorkflowFromCheckpoint_ShouldSucceed()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            var checkpointData = new Dictionary<string, object>
            {
                { "currentState", "Review" },
                { "variables", new Dictionary<string, object> { { "approvalCount", 0 } } }
            };
            
            // Act
            var response = await _client.PostAsJsonAsync($"/api/workflow/recovery/restore/{instanceId}", checkpointData);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowCommandResult>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetWorkflowCheckpoints_ShouldReturnCheckpoints()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/recovery/checkpoints/{instanceId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowCheckpointDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task RestoreFromCheckpoint_ShouldSucceed()
        {
            // Arrange
            var checkpointId = Guid.NewGuid();
            
            // Act
            var response = await _client.PostAsync($"/api/workflow/recovery/restore/{checkpointId}", null);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowRecoveryResultDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Restored.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetDiagnosticInfo_ShouldReturnDiagnostics()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/recovery/diagnostics/{instanceId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowDiagnosticsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.InstanceId.Should().Be(instanceId);
        }
    }
}
