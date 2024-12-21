using Microsoft.Extensions.Logging;
using Module;

namespace Application.Services.PropertyData
{
    public class QueryOptimizer
    {
        private readonly List<string> _appliedOptimizations = new();
        private readonly ILogger<QueryOptimizer> _logger;
        public QueryOptimizer(ILogger<QueryOptimizer> logger, ModuleDbContext context)
        {
            _logger = logger;
        }

        public PropertyQueryModel OptimizeQuery(
            PropertyQueryModel query,
            QueryOptimizationStrategy strategy = null)
        {
            _appliedOptimizations.Clear();
            var optimizedQuery = query;

            try
            {
                // Apply all optimizations
                optimizedQuery = OptimizeIncludes(optimizedQuery);
                optimizedQuery = OptimizeFilter(optimizedQuery);
                optimizedQuery = OptimizeSort(optimizedQuery);
                optimizedQuery = OptimizeGrouping(optimizedQuery);
                optimizedQuery = OptimizePagination(optimizedQuery);

                _logger.LogInformation(
                    "Query optimization completed with {OptimizationCount} optimizations",
                    _appliedOptimizations.Count);

                return optimizedQuery;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during query optimization");
                return query; // Return original query if optimization fails
            }
        }

        public IEnumerable<string> GetAppliedOptimizations()
        {
            return _appliedOptimizations.ToList();
        }

        private PropertyQueryModel OptimizeIncludes(PropertyQueryModel query)
        {
            if (query.IncludeProperties == null || !query.IncludeProperties.Any())
                return query;

            // Remove duplicate includes
            query.IncludeProperties = query.IncludeProperties.Distinct().ToArray();

            // Sort includes for consistent query plans
            query.IncludeProperties = query.IncludeProperties.OrderBy(x => x).ToArray();

            _appliedOptimizations.Add("IncludeOptimization");
            return query;
        }

        private PropertyQueryModel OptimizeFilter(PropertyQueryModel query)
        {
            if (query.Filter == null)
                return query;

            var optimizedFilter = new PropertyFilter
            {
                LogicalOperator = query.Filter.LogicalOperator,
                Conditions = OptimizeFilterConditions(query.Filter.Conditions),
                NestedFilters = query.Filter.NestedFilters?
                    .Select(f => OptimizeNestedFilter(f))
                    .Where(f => f != null)
                    .ToArray()
            };

            query.Filter = optimizedFilter;
            _appliedOptimizations.Add("FilterOptimization");
            return query;
        }

        private PropertyFilterCondition[] OptimizeFilterConditions(
            PropertyFilterCondition[] conditions)
        {
            if (conditions == null)
                return Array.Empty<PropertyFilterCondition>();

            return conditions
                .Where(c => c != null && !string.IsNullOrEmpty(c.PropertyKey))
                .OrderBy(c => c.PropertyKey)
                .ToArray();
        }

        private PropertyFilter OptimizeNestedFilter(PropertyFilter filter)
        {
            if (filter == null)
                return null;

            return new PropertyFilter
            {
                LogicalOperator = filter.LogicalOperator,
                Conditions = OptimizeFilterConditions(filter.Conditions),
                NestedFilters = filter.NestedFilters?
                    .Select(f => OptimizeNestedFilter(f))
                    .Where(f => f != null)
                    .ToArray()
            };
        }

        private PropertyQueryModel OptimizeSort(PropertyQueryModel query)
        {
            if (query.Sort == null || !query.Sort.Any())
                return query;

            // Remove invalid sort conditions
            query.Sort = query.Sort
                .Where(s => !string.IsNullOrEmpty(s.PropertyKey))
                .ToArray();

            // Ensure consistent direction format
            foreach (var sort in query.Sort)
            {
                sort.Direction = sort.Direction?.ToUpper() == "DESC" ? "DESC" : "ASC";
            }

            _appliedOptimizations.Add("SortOptimization");
            return query;
        }

        private PropertyQueryModel OptimizeGrouping(PropertyQueryModel query)
        {
            if (query.GroupBy == null || !query.GroupBy.Any())
                return query;

            // Remove invalid grouping conditions
            query.GroupBy = query.GroupBy
                .Where(g => !string.IsNullOrEmpty(g.PropertyKey))
                .ToArray();

            // Optimize TimeFrame formats if present
            foreach (var group in query.GroupBy.Where(g => !string.IsNullOrEmpty(g.TimeFrame)))
            {
                group.TimeFrame = OptimizeTimeFrame(group.TimeFrame);
            }

            _appliedOptimizations.Add("GroupingOptimization");
            return query;
        }

        private string OptimizeTimeFrame(string timeFrame)
        {
            return timeFrame.ToUpper() switch
            {
                "D" or "DAY" => "DAY",
                "W" or "WEEK" => "WEEK",
                "M" or "MONTH" => "MONTH",
                "Q" or "QUARTER" => "QUARTER",
                "Y" or "YEAR" => "YEAR",
                _ => timeFrame
            };
        }

        private PropertyQueryModel OptimizePagination(PropertyQueryModel query)
        {
            if (query.Pagination == null)
            {
                query.Pagination = new PaginationModel();
                _appliedOptimizations.Add("DefaultPaginationAdded");
            }

            // Ensure reasonable page size
            if (query.Pagination.PageSize <= 0 || query.Pagination.PageSize > 1000)
            {
                query.Pagination.PageSize = 50;
                _appliedOptimizations.Add("PageSizeOptimization");
            }

            // Ensure valid page number
            if (query.Pagination.Page <= 0)
            {
                query.Pagination.Page = 1;
                _appliedOptimizations.Add("PageNumberOptimization");
            }

            return query;
        }
    }

    // Extension methods for query optimization
    public static class QueryOptimizerExtensions
    {
        public static PropertyQueryModel WithDefaultOptimizations(
            this PropertyQueryModel query)
        {
            if (query.OptimizationStrategy == null)
            {
                query.OptimizationStrategy = new QueryOptimizationStrategy
                {
                    EnableParallelization = true,
                    EnableBatchFetching = true,
                    BatchSize = 1000,
                    EnableQuerySplitting = true
                };
            }

            return query;
        }

        public static PropertyQueryModel WithMinimalOptimizations(
            this PropertyQueryModel query)
        {
            query.OptimizationStrategy = new QueryOptimizationStrategy
            {
                EnableParallelization = false,
                EnableBatchFetching = false,
                EnableQuerySplitting = false
            };

            return query;
        }

        public static PropertyQueryModel WithCustomBatchSize(
            this PropertyQueryModel query,
            int batchSize)
        {
            query.OptimizationStrategy ??= new QueryOptimizationStrategy();

            query.OptimizationStrategy.BatchSize = batchSize;
            query.OptimizationStrategy.EnableBatchFetching = true;

            return query;
        }
    }
}