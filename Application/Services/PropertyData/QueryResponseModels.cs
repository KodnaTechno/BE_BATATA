using AppCommon.EnumShared;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;

namespace Application.Services.PropertyData
{
    #region Query Models
    public class PropertyQueryModel
    {
        public Guid? ApplicationId { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public string[] IncludeProperties { get; set; }
        public PropertyFilter Filter { get; set; }
        public PropertySort[] Sort { get; set; }
        public PropertyGrouping[] GroupBy { get; set; }
        public PropertyAggregation[] Aggregations { get; set; }
        public PaginationModel Pagination { get; set; }
        public bool IncludeMetadata { get; set; }
        public CachePolicy CachePolicy { get; set; }
        public QueryOptimizationStrategy OptimizationStrategy { get; set; }
    }

    public class PropertyFilter
    {
        public PropertyFilterCondition[] Conditions { get; set; }
        public string LogicalOperator { get; set; } = "AND";
        public PropertyFilter[] NestedFilters { get; set; }
    }

    public class PropertyFilterCondition
    {
        public string PropertyKey { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public DataTypeEnum ValueType { get; set; }
    }

    public class PropertySort
    {
        public string PropertyKey { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string Direction { get; set; }
        public string NullHandling { get; set; }
    }

    public class PropertyGrouping
    {
        public string PropertyKey { get; set; }
        public string TimeFrame { get; set; }
        public string Format { get; set; }
    }

    public class PropertyAggregation
    {
        public string PropertyKey { get; set; }
        public string Function { get; set; }
        public string Alias { get; set; }
    }

    public class PaginationModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int? MaxPageSize { get; set; }
        public bool EnableTotalCount { get; set; } = true;
        public bool UseKeyset { get; set; }
        public Guid? LastId { get; set; }
        public string Direction { get; set; }  // FORWARD or BACKWARD
    }

    public class CachePolicy
    {
        public bool EnableCache { get; set; } = true;
        public TimeSpan? Expiry { get; set; }
        public string CacheKey { get; set; }
        public string[] InvalidationTags { get; set; }
    }

    public class QueryOptimizationStrategy
    {
        public bool EnableParallelization { get; set; } = true;
        public bool EnableBatchFetching { get; set; } = true;
        public int BatchSize { get; set; } = 1000;
        public bool EnableQuerySplitting { get; set; } = true;
    }
    #endregion

    #region Response Models
    public class QueryResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, object> Aggregations { get; set; }
        public Dictionary<string, IEnumerable<GroupResult>> Groups { get; set; }
        public QueryMetadata Metadata { get; set; }
    }

    public class PropertyDataResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DataTypeEnum DataType { get; set; }
        public ViewTypeEnum ViewType { get; set; }
        public object Value { get; set; }
        public PropertyMetadata Metadata { get; set; }
        public PropertyContext Context { get; set; }
    }

    public class PropertyContext
    {
        public string Source { get; set; }
        public Guid SourceId { get; set; }
        public string SystemPropertyPath { get; set; }
    }

    public class PropertyMetadata
    {
        public bool IsSystem { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsTranslatable { get; set; }
        public string Configuration { get; set; }
        public ValidationRule[] ValidationRules { get; set; }
    }

    public class GroupResult
    {
        public string Key { get; set; }
        public int Count { get; set; }
        public Dictionary<string, object> Aggregations { get; set; }
        public IEnumerable<PropertyDataResponse> Items { get; set; }
    }

    public class QueryMetadata
    {
        public bool FromCache { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public string[] AppliedOptimizations { get; set; }
    }
    #endregion
}