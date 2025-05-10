namespace AppWorkflow.Domain.Schema;

public class RetryPolicy
{
    public int MaxRetries { get; set; }
    public TimeSpan RetryInterval { get; set; }
    public bool ExponentialBackoff { get; set; }
    public List<string> RetryableExceptions { get; set; } = new();
}