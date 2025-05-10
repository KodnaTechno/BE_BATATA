using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Interfaces.Repository;
using AppWorkflow.Infrastructure.Triggers;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services.Monitoring;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BE_BATATA.Tests.Integration
{
    public class TestApiFactory : WebApplicationFactory<Program>
    {
        private readonly MockWorkflowServices _mockServices;

        public TestApiFactory()
        {
            _mockServices = new MockWorkflowServices();
        }
        
        public MockWorkflowServices MockServices => _mockServices;
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Find the database contexts and replace them with in-memory versions
                var dbContextDescriptors = services.Where(
                    d => d.ServiceType.Namespace != null && 
                         d.ServiceType.Namespace.Contains("DbContext")).ToList();

                foreach (var descriptor in dbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Remove the real implementations of our services
                var workflowVersionManagerDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWorkflowVersionManager));
                if (workflowVersionManagerDescriptor != null)
                {
                    services.Remove(workflowVersionManagerDescriptor);
                }
                
                var workflowMigrationServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWorkflowMigrationService));
                if (workflowMigrationServiceDescriptor != null)
                {
                    services.Remove(workflowMigrationServiceDescriptor);
                }
                
                var monitoringServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWorkflowMonitoringService));
                if (monitoringServiceDescriptor != null)
                {
                    services.Remove(monitoringServiceDescriptor);
                }
                
                var recoveryServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWorkflowRecoveryService));
                if (recoveryServiceDescriptor != null)
                {
                    services.Remove(recoveryServiceDescriptor);
                }
                
                // Remove the existing trigger handlers
                var triggerHandlerDescriptors = services.Where(
                    d => d.ServiceType == typeof(IWorkflowTriggerHandler)).ToList();
                foreach (var descriptor in triggerHandlerDescriptors)
                {
                    services.Remove(descriptor);
                }
                
                // Add in-memory database
                services.AddDbContext<DbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });
                
                // Add our mock services
                _mockServices.ConfigureServices(services);

                // Configure the test service provider
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var logger = scopedServices.GetRequiredService<ILogger<TestApiFactory>>();

                    try
                    {
                        // Initialize test data if needed
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database with test data.");
                    }                }
            });
        }
    }
}
