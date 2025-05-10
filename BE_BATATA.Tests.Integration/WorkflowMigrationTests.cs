using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using AppCommon.DTOs;
using AppWorkflow.Common.DTO;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using AppWorkflow.Core.Interfaces.Services;
using Moq;
using AppWorkflow.Core.Domain.Data;
using System.Linq;

namespace BE_BATATA.Tests.Integration
{    public class WorkflowMigrationTests : IClassFixture<TestApiFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;
        private readonly Mock<IWorkflowVersionManager> _mockVersionManager;
        private readonly Mock<IWorkflowMigrationService> _mockMigrationService;

        public WorkflowMigrationTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _mockVersionManager = factory.MockServices.VersionManager;
            _mockMigrationService = factory.MockServices.MigrationService;
        }

        [Fact]
        public async Task GetInstancesForVersion_ShouldReturnInstances()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var version = 1;
              // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/instances/{version}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowInstanceDetails>>>();            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateMigration_ShouldReturnValidationResult()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var sourceVersion = 1;
            var targetVersion = 2;
            var instanceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            
            var migrationDto = new WorkflowMigrationDto
            {
                SourceVersion = sourceVersion,
                TargetVersion = targetVersion,
                InstanceIds = instanceIds
            };
            
            // Act
            var response = await _client.PostAsJsonAsync(
                $"/api/workflow/version/{workflowId}/validate-migration", 
                migrationDto);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMigrationValidationResult>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();            result.Data.IsValid.Should().BeTrue();
            result.Data.MigratableInstances.Should().Be(instanceIds.Count);
        }
        
        [Fact]
        public async Task MigrateInstances_ShouldSucceed()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var sourceVersion = 1;
            var targetVersion = 2;
            var instanceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            
            var migrationDto = new WorkflowMigrationDto
            {
                SourceVersion = sourceVersion,
                TargetVersion = targetVersion,
                InstanceIds = instanceIds
            };
            
            // Act
            var response = await _client.PostAsJsonAsync(
                $"/api/workflow/version/{workflowId}/migrate", 
                migrationDto);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMigrationResultDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();            result.Data.SuccessCount.Should().Be(instanceIds.Count);
            result.Data.FailedCount.Should().Be(0);
            result.Data.TotalCount.Should().Be(instanceIds.Count);
        }
        
        [Fact]
        public async Task GetCompatibleVersions_ShouldReturnVersions()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var sourceVersion = 1;
            
            // Act
            var response = await _client.GetAsync(
                $"/api/workflow/version/{workflowId}/{sourceVersion}/compatible-versions");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<int>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().NotBeEmpty();
            result.Data.Should().Contain(sourceVersion + 1);
            result.Data.Should().Contain(sourceVersion + 2);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowInstanceDetailsDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task CheckVersionIsLatest_ShouldReturnCorrectStatus()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var version = 1;
            
            // Create a version first
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/is-latest/{version}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            // The first version should be the latest
            result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task MigrateWorkflowInstance_ShouldSucceed()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            var workflowId = Guid.NewGuid();
            var sourceVersion = 1;
            var targetVersion = 2;
            
            // Act
            var response = await _client.PostAsync(
                $"/api/workflow/version/migrate-instance/{instanceId}?targetVersion={targetVersion}", 
                null);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMigrationResultDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetCompatibleVersions_ShouldReturnListOfVersions()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var version = 1;
            
            // Create some versions
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/compatible-versions/{version}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<int>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task CreateVersionWithMigrationPlan_ShouldCreateVersionWithPlan()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var migrationPlan = new WorkflowMigrationPlanDto
            {
                FieldMappings = new Dictionary<string, string>
                {
                    { "oldField", "newField" }
                },
                StateTransformations = new List<StateTransformationRuleDto>
                {
                    new StateTransformationRuleDto
                    {
                        SourceState = "Draft",
                        TargetState = "Review"
                    }
                }
            };
            
            // Act
            var response = await _client.PostAsJsonAsync(
                $"/api/workflow/version/{workflowId}/create-version-with-plan", 
                migrationPlan);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
