using Xunit;

namespace BE_BATATA.Tests.Integration;

/// <summary>
/// This class helps organize our tests by categories for better test runner filtering
/// </summary>
public static class TestCategories
{
    public const string WorkflowVersioning = "WorkflowVersioning";
    public const string WorkflowMigration = "WorkflowMigration";
    public const string WorkflowMonitoring = "WorkflowMonitoring";
    public const string WorkflowErrorRecovery = "WorkflowErrorRecovery";
}
