using AppWorkflow.Common.Enums;

namespace AppWorkflow.Domain.Schema;

public class WorkflowVariable
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public VariableType Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string? ValidationExpression { get; set; }
}