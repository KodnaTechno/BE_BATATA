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
{
    public class WorkflowMonitoringTests : IClassFixture<TestApiFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;

        public WorkflowMonitoringTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }        [Fact]
        public async Task GetWorkflowDashboard_ShouldReturnDashboardData()
        {
            // Act
            var response = await _client.GetAsync("/api/workflow/monitoring/dashboard");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowMonitoringDashboardDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ActiveWorkflows.Should().BeGreaterOrEqualTo(0);
            result.Data.CompletedWorkflows.Should().BeGreaterOrEqualTo(0);
            result.Data.FailedWorkflows.Should().BeGreaterOrEqualTo(0);
            result.Data.AverageCompletionTimeSeconds.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetWorkflowInstanceDetails_ShouldReturnInstanceDetails()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/monitoring/instance/{instanceId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
              var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowInstanceDetailsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.InstanceId.Should().Be(instanceId);
        }
        
        [Fact]
        public async Task GetWorkflowPerformanceMetrics_ShouldReturnMetrics()
        {
            // Act
            var response = await _client.GetAsync("/api/workflow/monitoring/performance");
            
            // Assert
            response.EnsureSuccessStatusCode();
              var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowPerformanceMetricsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.AverageCompletionTimeSeconds.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetActiveWorkflows_ShouldReturnActiveWorkflows()
        {
            // Act
            var response = await _client.GetAsync("/api/workflow/monitoring/active");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkflowStatusSummaryDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }
        
        [Fact]
        public async Task GetPerformanceMetrics_ShouldReturnMetrics()
        {
            // Arrange
            var workflowId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/workflow/monitoring/performance/{workflowId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowPerformanceMetricsDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.WorkflowId.Should().Be(workflowId);
        }
        
        [Fact]
        public async Task GetErrorReport_ShouldReturnErrorDetails()
        {
            // Act
            var response = await _client.GetAsync("/api/workflow/monitoring/errors");
            
            // Assert
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkflowErrorReportDto>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }
    }
}
