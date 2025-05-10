using AppWorkflow.Common.Enums;

namespace AppWorkflow.Common.DTO
{
    /// <summary>
    /// Data transfer object for workflow recovery operations
    /// </summary>
    public class RecoveryResult
    {
        /// <summary>
        /// Indicates if the recovery operation was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The ID of the workflow instance that was recovered
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// A message describing the result of the recovery operation
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// The new status of the workflow instance after recovery
        /// </summary>
        public string NewStatus { get; set; }
        
        /// <summary>
        /// The step that the workflow was recovered to
        /// </summary>
        public string RecoveredToStep { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for workflow checkpoint operations
    /// </summary>
    public class CheckpointResult
    {
        /// <summary>
        /// Indicates if the checkpoint operation was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The ID of the workflow instance
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// The ID of the created checkpoint
        /// </summary>
        public Guid CheckpointId { get; set; }
        
        /// <summary>
        /// The name of the checkpoint
        /// </summary>
        public string CheckpointName { get; set; }
        
        /// <summary>
        /// When the checkpoint was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for workflow checkpoint information
    /// </summary>
    public class CheckpointInfo
    {
        /// <summary>
        /// The ID of the checkpoint
        /// </summary>
        public Guid CheckpointId { get; set; }
        
        /// <summary>
        /// The ID of the workflow instance
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// The name of the checkpoint
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// When the checkpoint was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// The name of the step at which the checkpoint was created
        /// </summary>
        public string StepName { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for workflow recovery options
    /// </summary>
    public class RecoveryOptions
    {
        /// <summary>
        /// The recovery strategy to use
        /// </summary>
        public RecoveryStrategy Strategy { get; set; }
        
        /// <summary>
        /// The ID of a specific checkpoint to use for recovery (optional)
        /// </summary>
        public Guid? CheckpointId { get; set; }
        
        /// <summary>
        /// Additional options for the recovery operation
        /// </summary>
        public Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
    }
}
