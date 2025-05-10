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
{    public class WorkflowVersionControllerTests : IClassFixture<TestApiFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;
        private readonly Mock<IWorkflowVersionManager> _mockVersionManager;
        private readonly Mock<IWorkflowMigrationService> _mockMigrationService;

        public WorkflowVersionControllerTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _mockVersionManager = factory.MockServices.VersionManager;
            _mockMigrationService = factory.MockServices.MigrationService;
        }

        [Fact]
        public async Task CreateNewVersion_ShouldReturnSuccess_WhenWorkflowExists()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Act
            var response = await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.WorkflowId.Should().Be(workflowId);
            result.Data.Version.Should().BeGreaterThan(0);
            result.Data.IsLatest.Should().BeTrue();
        }

        [Fact]
        public async Task GetLatestVersion_ShouldReturnLatestVersion_WhenWorkflowExists()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
              // Create a version first
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/latest");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();            result.Data.WorkflowId.Should().Be(workflowId);
            result.Data.IsLatest.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetVersionHistory_ShouldReturnAllVersions_WhenWorkflowExists()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Create a version first
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/history");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowVersionInfoDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();            result.Data.Should().HaveCountGreaterThan(0);
            result.Data.Last().IsLatest.Should().BeTrue();
        }
        
        [Fact]
        public async Task IsLatestVersion_ShouldReturnTrue_WhenVersionIsLatest()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Create a version first
            var createResponse = await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            var version = createResult.Data.Version;
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/{version}/is-latest");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            result.Should().NotBeNull();            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetVersionMetrics_ShouldReturnMetrics_WhenVersionExists()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Create a version first
            var createResponse = await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            var version = createResult.Data.Version;
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/{version}/metrics");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionMetricsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.TotalInstances.Should().BeGreaterOrEqualTo(0);
            result.Data.ActiveInstances.Should().BeGreaterOrEqualTo(0);
            result.Data.CompletedInstances.Should().BeGreaterOrEqualTo(0);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionInfoDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.WorkflowId.Should().Be(workflowId);
            result.Data.IsLatest.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateMigration_ShouldReturnValidationResult()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var migrationRequest = new WorkflowMigrationDto
            {
                WorkflowId = workflowId,
                SourceVersion = 1,
                TargetVersion = 2,
                InstanceIds = new List<Guid> { Guid.NewGuid() }
            };
            
            // Act
            var response = await _client.PostAsJsonAsync("/api/workflow/version/validate-migration", migrationRequest);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMigrationValidationResultDto>>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task MigrateInstances_ShouldReturnMigrationResult()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var migrationRequest = new WorkflowMigrationDto
            {
                WorkflowId = workflowId,
                SourceVersion = 1,
                TargetVersion = 2,
                InstanceIds = new List<Guid> { Guid.NewGuid() }
            };
            
            // Act
            var response = await _client.PostAsJsonAsync("/api/workflow/version/migrate", migrationRequest);
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMigrationResultDto>>();
            result.Should().NotBeNull();
        }
        
        [Fact]
        public async Task GetVersionHistory_ShouldReturnHistoryWithMetrics()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Create some versions first
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            await _client.PostAsync($"/api/workflow/version/{workflowId}/create-version", null);
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/history");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowVersionHistoryDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            result.Data.Count.Should().BeGreaterThanOrEqualTo(2);
        }
        
        [Fact]
        public async Task GetVersionMetrics_ShouldReturnMetricsData()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            var version = 1;
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/version/{workflowId}/metrics/{version}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowVersionMetricsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
