using System;
using System.Collections.Generic;

namespace AppWorkflow.Common.DTO
{
    /// <summary>
    /// Data transfer object for workflow version migration requests
    /// </summary>
    public class WorkflowMigrationDto
    {
        /// <summary>
        /// The ID of the workflow instance to migrate
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// The target version to migrate to
        /// </summary>
        public string TargetVersion { get; set; }
        
        /// <summary>
        /// Optional migration strategy to use
        /// </summary>
        public string MigrationStrategy { get; set; } = "Auto";
        
        /// <summary>
        /// Optional custom mapping for workflow steps that can't be automatically mapped
        /// </summary>
        public Dictionary<Guid, Guid> CustomStepMapping { get; set; } = new();
    }

    /// <summary>
    /// Response object for workflow version migration results
    /// </summary>
    public class WorkflowMigrationResultDto
    {
        /// <summary>
        /// Whether the migration was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The ID of the migrated workflow instance
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// The source version of the workflow
        /// </summary>
        public string SourceVersion { get; set; }
        
        /// <summary>
        /// The target version the workflow was migrated to
        /// </summary>
        public string TargetVersion { get; set; }
        
        /// <summary>
        /// The migration strategy used
        /// </summary>
        public string MigrationStrategy { get; set; }
        
        /// <summary>
        /// The step mappings that were created during migration
        /// </summary>
        public Dictionary<string, string> StepMappings { get; set; } = new();
        
        /// <summary>
        /// Any warnings generated during the migration
        /// </summary>
        public List<string> Warnings { get; set; } = new();
        
        /// <summary>
        /// When the migration occurred
        /// </summary>
        public DateTime MigrationTime { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Data transfer object for workflow version information
    /// </summary>
    public class WorkflowVersionInfoDto
    {
        /// <summary>
        /// The workflow ID
        /// </summary>
        public Guid WorkflowId { get; set; }
        
        /// <summary>
        /// The version string (e.g., "1.0.0")
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// Whether this is the latest version
        /// </summary>
        public bool IsLatest { get; set; }
        
        /// <summary>
        /// When this version was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Who created this version
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// Number of active instances using this version
        /// </summary>
        public int ActiveInstances { get; set; }
    }
}
