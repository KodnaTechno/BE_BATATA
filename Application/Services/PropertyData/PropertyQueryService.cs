using Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;
using Module.Domain.Shared;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using AppCommon.EnumShared;

namespace Application.Services.PropertyData
{
    public class PropertyQueryService
    {
        private readonly ModuleDbContext _context;
        private readonly IEntityCacheService _cacheService;
        private readonly QueryOptimizer _queryOptimizer;
        private readonly ILogger<PropertyQueryService> _logger;

        public PropertyQueryService(
            ModuleDbContext context,
            IEntityCacheService cacheService,
            QueryOptimizer queryOptimizer,
            ILogger<PropertyQueryService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _queryOptimizer = queryOptimizer ?? throw new ArgumentNullException(nameof(queryOptimizer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<QueryResult<PropertyDataResponse>> QueryPropertiesAsync(
            PropertyQueryModel query,
            CancellationToken cancellationToken = default)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            try
            {
                var cacheKey = GenerateCacheKey(query);

                return await _cacheService.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var optimizedQuery = _queryOptimizer.OptimizeQuery(query);
                        var baseQuery = BuildBaseQuery(optimizedQuery);

                        // Apply performance optimizations
                        if (optimizedQuery.OptimizationStrategy?.EnableBatchFetching == true)
                        {
                            // Potentially use compiled queries here if needed
                        }

                        if (optimizedQuery.OptimizationStrategy?.EnableQuerySplitting == true)
                        {
                            // Already handled in BuildBaseQuery via AsSplitQuery()
                        }

                        if (optimizedQuery.OptimizationStrategy?.EnableParallelization == true)
                        {
                            baseQuery = baseQuery.AsNoTrackingWithIdentityResolution();
                        }

                        if (optimizedQuery.GroupBy?.Any() == true)
                        {
                            return await ExecuteGroupedQueryAsync(baseQuery, optimizedQuery, cancellationToken);
                        }
                        else
                        {
                            return await ExecuteQueryAsync(baseQuery, optimizedQuery, cancellationToken);
                        }
                    },
                    query.CachePolicy?.Expiry ?? TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing property query");
                throw;
            }
        }



        public async Task<QueryResult<PropertyDataResponse>> GetByModuleDataIdAsync(
            Guid moduleDataId,
            bool includeMetadata = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.PropertyData.AsNoTracking()
                            .Include(pd => pd.Property)
                            .Where(pd => pd.ModuleDataId == moduleDataId);

                if (includeMetadata)
                {
                    query = query.Include(pd => pd.Property.ValidationRules)
                                 .Include(pd => pd.Property.PropertyFormulas);
                }

                var data = await query.ToListAsync(cancellationToken);

                return new QueryResult<PropertyDataResponse>
                {
                    Data = data.Select(MapToResponse).ToList(),
                    TotalCount = data.Count,
                    Metadata = new QueryMetadata
                    {
                        FromCache = false,
                        QueryExecutionTime = TimeSpan.Zero
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties by module data ID: {ModuleDataId}", moduleDataId);
                throw;
            }
        }


        public async Task<QueryResult<PropertyDataResponse>> GetByWorkspaceDataIdAsync(
            Guid workspaceDataId,
            bool includeMetadata = false,
            TimeSpan? cacheExpiry = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.PropertyData.AsNoTracking()
                            .Include(pd => pd.Property)
                            .Where(pd => pd.WorkspaceDataId == workspaceDataId);

                if (includeMetadata)
                {
                    query = query.Include(pd => pd.Property.ValidationRules)
                                 .Include(pd => pd.Property.PropertyFormulas);
                }

                var data = await query.ToListAsync(cancellationToken);

                return new QueryResult<PropertyDataResponse>
                {
                    Data = data.Select(MapToResponse).ToList(),
                    TotalCount = data.Count,
                    Metadata = new QueryMetadata
                    {
                        FromCache = false,
                        QueryExecutionTime = TimeSpan.Zero
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties by workspace data ID: {WorkspaceDataId}", workspaceDataId);
                throw;
            }
        }


        public async Task<PropertyDataResponse> GetSinglePropertyAsync(
            Guid propertyId,
            Guid? moduleDataId = null,
            Guid? workspaceDataId = null,
            TimeSpan? cacheExpiry = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"property_{propertyId}_{moduleDataId}_{workspaceDataId}";

            try
            {
                return await _cacheService.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var query = _context.PropertyData.AsNoTracking()
                            .Include(pd => pd.Property)
                            .Where(pd => pd.PropertyId == propertyId);

                        if (moduleDataId.HasValue)
                            query = query.Where(pd => pd.ModuleDataId == moduleDataId);

                        if (workspaceDataId.HasValue)
                            query = query.Where(pd => pd.WorkspaceDataId == workspaceDataId);

                        var data = await query.FirstOrDefaultAsync(cancellationToken);
                        return data != null ? MapToResponse(data) : null;
                    },
                    cacheExpiry ?? TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving single property: {PropertyId}", propertyId);
                throw;
            }
        }


        #region Query Construction Methods

        private IQueryable<Module.Domain.Data.PropertyData> BuildBaseQuery(PropertyQueryModel query)
        {
            var baseQuery = _context.PropertyData.AsNoTracking();

            if (query.OptimizationStrategy?.EnableQuerySplitting == true)
            {
                baseQuery = baseQuery.AsSplitQuery();
            }

            if (query.ApplicationId.HasValue)
            {
                baseQuery = baseQuery.Where(pd =>
                    (pd.ModuleData != null && pd.ModuleData.Module.ApplicationId == query.ApplicationId) ||
                    (pd.WorkspaceData != null && pd.WorkspaceData.Workspace.ApplicationId == query.ApplicationId));
            }

            if (query.WorkspaceId.HasValue)
            {
                baseQuery = baseQuery.Where(pd =>
                    pd.WorkspaceData != null && pd.WorkspaceData.WorkspaceId == query.WorkspaceId);
            }

            if (query.ModuleId.HasValue)
            {
                baseQuery = baseQuery.Where(pd =>
                    pd.ModuleData != null && pd.ModuleData.ModuleId == query.ModuleId);
            }

            return baseQuery;
        }

        #endregion

        #region Filtering Expressions

        private Expression BuildPropertyValueAccess(ParameterExpression parameter, DataTypeEnum dataType)
        {
            var propertyName = dataType switch
            {
                DataTypeEnum.Guid => nameof(Module.Domain.Data.PropertyData.GuidValue),
                DataTypeEnum.String => nameof(Module.Domain.Data.PropertyData.StringValue),
                DataTypeEnum.Int => nameof(Module.Domain.Data.PropertyData.IntValue),
                DataTypeEnum.DateTime => nameof(Module.Domain.Data.PropertyData.DateTimeValue),
                DataTypeEnum.DateOnly => nameof(Module.Domain.Data.PropertyData.DateValue),
                DataTypeEnum.Double => nameof(Module.Domain.Data.PropertyData.DoubleValue),
                DataTypeEnum.Decimal => nameof(Module.Domain.Data.PropertyData.DecimalValue),
                DataTypeEnum.Bool => nameof(Module.Domain.Data.PropertyData.BoolValue),
                _ => throw new NotSupportedException($"DataType '{dataType}' is not supported.")
            };
            return Expression.Property(parameter, propertyName);
        }

        private object ConvertFilterValue(object value, DataTypeEnum targetType)
        {
            if (value == null) return null;

            return targetType switch
            {
                DataTypeEnum.Guid => (value is Guid g) ? g : Guid.Parse(value.ToString()),
                DataTypeEnum.String => value.ToString(),
                DataTypeEnum.Int => Convert.ToInt32(value),
                DataTypeEnum.DateTime => (value is DateTime dt) ? dt : DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture),
                DataTypeEnum.DateOnly => (value is DateOnly d) ? d : DateOnly.FromDateTime(DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture)),
                DataTypeEnum.Double => Convert.ToDouble(value),
                DataTypeEnum.Decimal => Convert.ToDecimal(value),
                DataTypeEnum.Bool => Convert.ToBoolean(value),
                _ => throw new NotSupportedException($"DataType '{targetType}' is not supported.")
            };
        }

        private Expression BuildComparisonExpression(
            Expression propertyAccess,
            Expression constantValue,
            Func<Expression, Expression, Expression> comparisonFactory)
        {
            if (IsNullableType(propertyAccess.Type))
            {
                var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
                var valueProperty = Expression.Property(propertyAccess, "Value");
                var comparison = comparisonFactory(valueProperty, constantValue);
                return Expression.AndAlso(hasValueProperty, comparison);
            }
            return comparisonFactory(propertyAccess, constantValue);
        }

        private Expression BuildStringOperation(Expression propertyAccess, Expression constantValue, string methodName)
        {
            var method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var methodCall = Expression.Call(propertyAccess, method, constantValue);
            return Expression.AndAlso(nullCheck, methodCall);
        }

        private Expression BuildInExpression(Expression propertyAccess, object values, DataTypeEnum dataType)
        {
            if (values is not IEnumerable enumerable)
                return Expression.Constant(false);

            var items = enumerable.Cast<object>()
                .Select(v => ConvertFilterValue(v, dataType))
                .ToList();

            if (!items.Any())
                return Expression.Constant(false);

            if (IsNullableType(propertyAccess.Type))
            {
                var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
                var valueProperty = Expression.Property(propertyAccess, "Value");
                var containsMethod = typeof(List<>).MakeGenericType(GetNonNullableType(propertyAccess.Type))
                    .GetMethod("Contains", new[] { GetNonNullableType(propertyAccess.Type) });
                var list = Expression.Constant(items, typeof(List<>).MakeGenericType(GetNonNullableType(propertyAccess.Type)));
                var contains = Expression.Call(list, containsMethod, valueProperty);
                return Expression.AndAlso(hasValueProperty, contains);
            }

            var method = typeof(List<>).MakeGenericType(propertyAccess.Type).GetMethod("Contains", new[] { propertyAccess.Type });
            return Expression.Call(Expression.Constant(items, typeof(List<>).MakeGenericType(propertyAccess.Type)), method, propertyAccess);
        }

        private Expression BuildBetweenExpression(Expression propertyAccess, object value, DataTypeEnum dataType)
        {
            if (value is not IList<object> range || range.Count != 2)
                return Expression.Constant(false);

            var lowerBound = ConvertFilterValue(range[0], dataType);
            var upperBound = ConvertFilterValue(range[1], dataType);

            if (IsNullableType(propertyAccess.Type))
            {
                var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
                var valueProperty = Expression.Property(propertyAccess, "Value");

                var greaterThan = Expression.GreaterThanOrEqual(valueProperty, Expression.Constant(lowerBound));
                var lessThan = Expression.LessThanOrEqual(valueProperty, Expression.Constant(upperBound));
                var between = Expression.AndAlso(greaterThan, lessThan);

                return Expression.AndAlso(hasValueProperty, between);
            }

            var gt = Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(lowerBound));
            var lt = Expression.LessThanOrEqual(propertyAccess, Expression.Constant(upperBound));
            return Expression.AndAlso(gt, lt);
        }

        private Expression BuildNullCheckExpression(ParameterExpression parameter, DataTypeEnum dataType, bool checkForNull)
        {
            var propertyAccess = BuildPropertyValueAccess(parameter, dataType);

            if (IsNullableType(propertyAccess.Type))
            {
                var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
                return checkForNull ? Expression.Not(hasValueProperty) : hasValueProperty;
            }

            return checkForNull
                ? Expression.Equal(propertyAccess, Expression.Constant(null, propertyAccess.Type))
                : Expression.NotEqual(propertyAccess, Expression.Constant(null, propertyAccess.Type));
        }

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
        }

        #endregion

        #region Extended Operators Support

        private Expression BuildFuzzyMatchExpression(Expression propertyAccess, Expression constantValue)
        {
            // Using EF.Functions.Like for a basic fuzzy match (e.g., LIKE '%value%')
            // Adjust logic for more complex fuzzy logic if needed
            var likeMethod = typeof(Microsoft.EntityFrameworkCore.DbFunctionsExtensions)
                .GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            // For EF.Functions.Like we need EF.Functions instance (not available here as static).
            // We'll simulate a closure variable or consider refactoring to build the expression differently.
            var dbFunctionsProperty = Expression.Property(null, typeof(EF), nameof(EF.Functions));
            var pattern = Expression.Constant("%" + (string)((ConstantExpression)constantValue).Value + "%", typeof(string));
            return Expression.Call(likeMethod, dbFunctionsProperty, propertyAccess, pattern);
        }

        private Expression BuildRegexMatchExpression(Expression propertyAccess, Expression constantValue)
        {
            // Regex.IsMatch(property, pattern)
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string) });
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var callIsMatch = Expression.Call(isMatchMethod, propertyAccess, constantValue);
            return Expression.AndAlso(nullCheck, callIsMatch);
        }

        private Expression BuildDateRangeExpression(Expression propertyAccess, object value)
        {
            // Expecting value to be something like { Start = DateTimeOffset, End = DateTimeOffset } or similar
            // For simplicity, let's say value is a two-element array [start, end] of DateTime, consider timezone if needed.
            if (value is not IList<object> range || range.Count != 2)
                return Expression.Constant(false);

            var start = range[0] is DateTimeOffset s ? s : DateTimeOffset.Parse(range[0].ToString(), CultureInfo.InvariantCulture);
            var end = range[1] is DateTimeOffset e ? e : DateTimeOffset.Parse(range[1].ToString(), CultureInfo.InvariantCulture);

            // Assuming propertyAccess is a DateTime or DateTime? value
            // We could convert DateTime to DateTimeOffset if needed.
            // For simplicity, treat property as DateTime and compare.
            var propertyType = propertyAccess.Type;
            if (propertyType == typeof(DateTime?) || propertyType == typeof(DateTime))
            {
                // Convert start/end to DateTime to compare
                var startDt = start.UtcDateTime;
                var endDt = end.UtcDateTime;

                Expression valueProperty = propertyAccess;
                if (IsNullableType(propertyType))
                {
                    var hasValue = Expression.Property(propertyAccess, "HasValue");
                    var actualValue = Expression.Property(propertyAccess, "Value");
                    var inRange = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(actualValue, Expression.Constant(startDt)),
                        Expression.LessThanOrEqual(actualValue, Expression.Constant(endDt))
                    );
                    return Expression.AndAlso(hasValue, inRange);
                }
                else
                {
                    var inRange = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(startDt)),
                        Expression.LessThanOrEqual(propertyAccess, Expression.Constant(endDt))
                    );
                    return inRange;
                }
            }

            return Expression.Constant(false);
        }

        #endregion

        #region Filter Construction

        private Expression BuildConditionExpression(ParameterExpression parameter, PropertyFilterCondition condition)
        {
            var propertyAccess = BuildPropertyValueAccess(parameter, condition.ValueType);
            var constantValue = condition.Operator?.ToUpper() switch
            {
                "REGEX" or "FUZZY" or "DATE_RANGE" => null, // We'll handle these differently
                _ => Expression.Constant(ConvertFilterValue(condition.Value, condition.ValueType))
            };

            return condition.Operator?.ToUpper() switch
            {
                "EQ" => BuildComparisonExpression(propertyAccess, constantValue, Expression.Equal),
                "NEQ" => BuildComparisonExpression(propertyAccess, constantValue, Expression.NotEqual),
                "GT" => BuildComparisonExpression(propertyAccess, constantValue, Expression.GreaterThan),
                "GTE" => BuildComparisonExpression(propertyAccess, constantValue, Expression.GreaterThanOrEqual),
                "LT" => BuildComparisonExpression(propertyAccess, constantValue, Expression.LessThan),
                "LTE" => BuildComparisonExpression(propertyAccess, constantValue, Expression.LessThanOrEqual),
                "CONTAINS" when condition.ValueType == DataTypeEnum.String =>
                    BuildStringOperation(propertyAccess, constantValue, "Contains"),
                "STARTSWITH" when condition.ValueType == DataTypeEnum.String =>
                    BuildStringOperation(propertyAccess, constantValue, "StartsWith"),
                "ENDSWITH" when condition.ValueType == DataTypeEnum.String =>
                     BuildStringOperation(propertyAccess, constantValue, "EndsWith"),
                "IN" => BuildInExpression(propertyAccess, condition.Value, condition.ValueType),
                "NOTIN" => Expression.Not(BuildInExpression(propertyAccess, condition.Value, condition.ValueType)),
                "BETWEEN" => BuildBetweenExpression(propertyAccess, condition.Value, condition.ValueType),
                "ISNULL" => BuildNullCheckExpression(parameter, condition.ValueType, true),
                "ISNOTNULL" => BuildNullCheckExpression(parameter, condition.ValueType, false),
                "REGEX" when condition.ValueType == DataTypeEnum.String && condition.Value != null =>
                    BuildRegexMatchExpression(propertyAccess, Expression.Constant(condition.Value.ToString())),
                "FUZZY" when condition.ValueType == DataTypeEnum.String && condition.Value != null =>
                    BuildFuzzyMatchExpression(propertyAccess, Expression.Constant(condition.Value.ToString())),
                "DATE_RANGE" when condition.ValueType == DataTypeEnum.DateTime =>
                    BuildDateRangeExpression(propertyAccess, condition.Value),
                _ => Expression.Constant(true)
            };
        }

        private Expression<Func<Module.Domain.Data.PropertyData, bool>> BuildFilterPredicate(PropertyFilter filter)
        {
            var parameter = Expression.Parameter(typeof(Module.Domain.Data.PropertyData), "pd");
            var expression = BuildFilterExpression(parameter, filter);
            return Expression.Lambda<Func<Module.Domain.Data.PropertyData, bool>>(expression, parameter);
        }

        private Expression BuildFilterExpression(ParameterExpression parameter, PropertyFilter filter)
        {
            if (filter == null || (filter.Conditions?.Any() != true && filter.NestedFilters?.Any() != true))
                return Expression.Constant(true);

            var conditions = filter.Conditions?
                .Where(c => c != null && !string.IsNullOrEmpty(c.PropertyKey))
                .Select(c => BuildConditionExpression(parameter, c))
                .ToList() ?? new List<Expression>();

            var nestedFilters = filter.NestedFilters?
                .Where(nf => nf != null)
                .Select(f => BuildFilterExpression(parameter, f))
                .ToList() ?? new List<Expression>();

            var expressions = conditions.Concat(nestedFilters).ToList();

            if (!expressions.Any())
                return Expression.Constant(true);

            return filter.LogicalOperator?.ToUpper() switch
            {
                "OR" => expressions.Aggregate(Expression.OrElse),
                "XOR" => expressions.Aggregate(Expression.ExclusiveOr),
                "NAND" => Expression.Not(expressions.Aggregate(Expression.AndAlso)),
                "NOR" => Expression.Not(expressions.Aggregate(Expression.OrElse)),
                _ => expressions.Aggregate(Expression.AndAlso) // default AND
            };
        }

        private IQueryable<Module.Domain.Data.PropertyData> ApplyFilters(
            IQueryable<Module.Domain.Data.PropertyData> query,
            PropertyFilter filter)
        {
            if (filter == null) return query;
            var predicate = BuildFilterPredicate(filter);
            return query.Where(predicate);
        }

        #endregion

        #region Grouping and Aggregations

        private IQueryable<IGrouping<object, Module.Domain.Data.PropertyData>> ApplyGroupingForResult(
    IQueryable<Module.Domain.Data.PropertyData> query,
    PropertyGrouping[] groupings)
        {
            if (groupings == null || !groupings.Any())
                return null;

            var parameter = Expression.Parameter(typeof(Module.Domain.Data.PropertyData), "pd");
            var memberBindings = new List<MemberBinding>();

            for (int i = 0; i < groupings.Length && i < 5; i++)
            {
                var grouping = groupings[i];
                var propertyExpression = BuildGroupingExpression(parameter, grouping);

                // Bind the property to a DynamicGrouping's KeyX
                var propInfo = typeof(DynamicGrouping).GetProperty($"Key{i}");
                if (propInfo != null)
                {
                    memberBindings.Add(Expression.Bind(propInfo, Expression.Convert(propertyExpression, typeof(object))));
                }
            }

            var constructor = typeof(DynamicGrouping).GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(constructor);
            var memberInit = Expression.MemberInit(newExpression, memberBindings);

            // Convert the DynamicGrouping object to object
            var converted = Expression.Convert(memberInit, typeof(object));
            var lambda = Expression.Lambda<Func<Module.Domain.Data.PropertyData, object>>(converted, parameter);

            return query.GroupBy(lambda);
        }

        private Expression BuildGroupingExpression(ParameterExpression parameter, PropertyGrouping grouping)
        {
            var propertyAccess = Expression.Property(parameter, grouping.PropertyKey);

            return grouping.TimeFrame?.ToUpper() switch
            {
                "YEAR" => Expression.Property(propertyAccess, "Year"),
                "MONTH" => Expression.Property(propertyAccess, "Month"),
                "WEEK" => BuildWeekGrouping(propertyAccess),
                "DAY" => Expression.Property(propertyAccess, "Day"),
                "HOUR" => Expression.Property(propertyAccess, "Hour"),
                _ => propertyAccess
            };
        }

        private Expression BuildWeekGrouping(Expression dateExpression)
        {
            var culture = Expression.Property(null, typeof(System.Globalization.CultureInfo), "CurrentCulture");
            var calendarProperty = Expression.Property(culture, "Calendar");

            return Expression.Call(
                calendarProperty,
                "GetWeekOfYear",
                Type.EmptyTypes,
                dateExpression,
                Expression.Constant(System.Globalization.CalendarWeekRule.FirstFourDayWeek),
                Expression.Constant(DayOfWeek.Monday)
            );
        }

        private async Task<QueryResult<PropertyDataResponse>> ExecuteGroupedQueryAsync(
            IQueryable<Module.Domain.Data.PropertyData> baseQuery,
            PropertyQueryModel query,
            CancellationToken cancellationToken)
        {
            var filteredQuery = ApplyFilters(baseQuery, query.Filter);

            // Perform grouping
            var groupedQuery = ApplyGroupingForResult(filteredQuery, query.GroupBy);
            if (groupedQuery == null)
            {
                // No grouping, fall back
                return await ExecuteQueryAsync(baseQuery, query, cancellationToken);
            }

            // We must now evaluate each group and apply aggregations if any
            var aggregationTask = query.Aggregations?.Any() == true
                ? CalculateGroupedAggregationsAsync(groupedQuery, query.Aggregations, cancellationToken)
                : Task.FromResult(new Dictionary<object, Dictionary<string, object>>());

            // Sorting and pagination on grouped results is trickier. We might sort within groups or by keys only.
            // For simplicity, let's not apply sorting/pagination on groups at the group level here.
            // If needed, apply sorting to filteredQuery before grouping.

            var groupList = await groupedQuery.ToListAsync(cancellationToken);
            var groupedResults = await aggregationTask;
            var totalCount = query.Pagination?.EnableTotalCount == true ? groupList.Sum(g => g.Count()) : 0;

            // Build structured result
            var groupsDict = new Dictionary<string, IEnumerable<GroupResult>>();

            // Convert groups to GroupResult objects
            // Assume single or multiple keys from DynamicGrouping, we must extract them as a string key
            var groupResults = groupList.Select(g =>
            {
                var keyObject = g.Key;
                var dynamicKey = keyObject as DynamicGrouping;
                var keyStr = BuildGroupKeyString(dynamicKey);

                groupedResults.TryGetValue(g.Key, out var aggDict);

                return new GroupResult
                {
                    Key = keyStr,
                    Count = g.Count(),
                    Aggregations = aggDict,
                    Items = g.Select(MapToResponse)
                };
            }).ToList();

            groupsDict["DefaultGroup"] = groupResults;

            var metadata = new QueryMetadata
            {
                FromCache = false,
                QueryExecutionTime = TimeSpan.Zero,
                AppliedOptimizations = _queryOptimizer.GetAppliedOptimizations().ToArray()
            };

            return new QueryResult<PropertyDataResponse>
            {
                Data = groupResults.SelectMany(gr => gr.Items),
                TotalCount = totalCount,
                Groups = groupsDict,
                Metadata = metadata
            };
        }

        private string BuildGroupKeyString(DynamicGrouping dynamicKey)
        {
            // Build a composite key string
            var keys = new List<string>();
            if (dynamicKey.Key0 != null) keys.Add(dynamicKey.Key0.ToString());
            if (dynamicKey.Key1 != null) keys.Add(dynamicKey.Key1.ToString());
            if (dynamicKey.Key2 != null) keys.Add(dynamicKey.Key2.ToString());
            if (dynamicKey.Key3 != null) keys.Add(dynamicKey.Key3.ToString());
            if (dynamicKey.Key4 != null) keys.Add(dynamicKey.Key4.ToString());

            return string.Join("_", keys);
        }

        private async Task<Dictionary<object, Dictionary<string, object>>> CalculateGroupedAggregationsAsync(
            IQueryable<IGrouping<object, Module.Domain.Data.PropertyData>> groupedQuery,
            PropertyAggregation[] aggregations,
            CancellationToken cancellationToken)
        {
            var result = new Dictionary<object, Dictionary<string, object>>();
            if (aggregations == null || !aggregations.Any())
                return result;

            // For each group, compute aggregations
            var groupList = await groupedQuery.ToListAsync(cancellationToken);
            foreach (var group in groupList)
            {
                var aggResults = new Dictionary<string, object>();
                foreach (var agg in aggregations)
                {
                    try
                    {
                        var value = await CalculateSingleAggregationAsync(group.AsQueryable(), agg, cancellationToken);
                        if (value != null)
                        {
                            aggResults[agg.Alias ?? $"{agg.Function}_{agg.PropertyKey}"] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calculating aggregation {Function} for {PropertyKey} in grouped result",
                            agg.Function, agg.PropertyKey);
                    }
                }

                if (aggResults.Any())
                {
                    result[group.Key] = aggResults;
                }
            }

            return result;
        }

        #endregion

        #region Sorting and Pagination

        private IQueryable<Module.Domain.Data.PropertyData> ApplySorting(
            IQueryable<Module.Domain.Data.PropertyData> query,
            PropertySort[] sorts)
        {
            if (sorts == null || !sorts.Any())
                return query.OrderBy(pd => pd.Id);

            var parameter = Expression.Parameter(typeof(Module.Domain.Data.PropertyData), "pd");
            IOrderedQueryable<Module.Domain.Data.PropertyData> orderedQuery = null;

            foreach (var sort in sorts)
            {
                var propertyAccess = BuildPropertyValueAccess(parameter, sort.DataType);
                var lambda = Expression.Lambda(propertyAccess, parameter);
                var orderMethodName = DetermineSortMethod(sort, orderedQuery == null);

                var genericMethod = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == orderMethodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(Module.Domain.Data.PropertyData), GetNonNullableType(propertyAccess.Type));

                var sourceQuery = orderedQuery ?? query;
                orderedQuery = (IOrderedQueryable<Module.Domain.Data.PropertyData>)genericMethod
                    .Invoke(null, new object[] { sourceQuery, lambda });
            }

            return orderedQuery ?? query;
        }

        private string DetermineSortMethod(PropertySort sort, bool isFirst)
        {
            return (sort.Direction?.ToUpper(), isFirst) switch
            {
                ("DESC", true) => "OrderByDescending",
                ("DESC", false) => "ThenByDescending",
                (_, true) => "OrderBy",
                (_, false) => "ThenBy"
            };
        }

        private IQueryable<Module.Domain.Data.PropertyData> ApplyPagination(
            IQueryable<Module.Domain.Data.PropertyData> query,
            PaginationModel pagination)
        {
            if (pagination == null)
                return query;

            var page = Math.Max(1, pagination.Page);
            var pageSize = Math.Clamp(pagination.PageSize, 1, pagination.MaxPageSize ?? 1000);

            if (pagination.UseKeyset && pagination.LastId.HasValue)
            {
                return pagination.Direction?.ToUpper() == "BACKWARD"
                    ? query.Where(pd => pd.Id < pagination.LastId)
                           .OrderByDescending(pd => pd.Id)
                           .Take(pageSize)
                    : query.Where(pd => pd.Id > pagination.LastId)
                           .OrderBy(pd => pd.Id)
                           .Take(pageSize);
            }

            var skip = (page - 1) * pageSize;
            return query.Skip(skip).Take(pageSize);
        }

        #endregion

        #region Execution and Mapping

        private async Task<QueryResult<PropertyDataResponse>> ExecuteQueryAsync(
            IQueryable<Module.Domain.Data.PropertyData> baseQuery,
            PropertyQueryModel query,
            CancellationToken cancellationToken)
        {
            var filteredQuery = ApplyFilters(baseQuery, query.Filter);
            var sortedQuery = ApplySorting(filteredQuery, query.Sort);
            var pagedQuery = ApplyPagination(sortedQuery, query.Pagination);

            var dataTask = pagedQuery.ToListAsync(cancellationToken);
            var countTask = query.Pagination?.EnableTotalCount == true
                ? filteredQuery.CountAsync(cancellationToken)
                : Task.FromResult(0);

            var aggregationTask = query.Aggregations?.Any() == true
                ? CalculateAggregationsAsync(filteredQuery, query.Aggregations, cancellationToken)
                : Task.FromResult(new Dictionary<string, object>());

            await Task.WhenAll(dataTask, countTask, aggregationTask).ConfigureAwait(false);

            var metadata = new QueryMetadata
            {
                FromCache = false,
                QueryExecutionTime = TimeSpan.Zero,
                AppliedOptimizations = _queryOptimizer.GetAppliedOptimizations().ToArray()
            };

            return new QueryResult<PropertyDataResponse>
            {
                Data = dataTask.Result.Select(MapToResponse).ToList(),
                TotalCount = countTask.Result,
                Aggregations = aggregationTask.Result,
                Metadata = metadata
            };
        }

        private PropertyDataResponse MapToResponse(Module.Domain.Data.PropertyData data)
        {
            if (data?.Property == null)
                return null;

            return new PropertyDataResponse
            {
                Id = data.Id,
                Key = data.Property.Key,
                Title = data.Property.Title,
                Description = data.Property.Description,
                DataType = data.DataType,
                ViewType = data.Property.ViewType,
                Value = data.GetValue(),
                Metadata = new PropertyMetadata
                {
                    IsSystem = data.Property.IsSystem,
                    IsCalculated = data.Property.IsCalculated,
                    IsEncrypted = data.Property.IsEncrypted,
                    IsTranslatable = data.Property.IsTranslatable,
                    Configuration = data.Property.Configuration,
                    ValidationRules = data.Property.ValidationRules?.ToArray()
                },
                Context = new PropertyContext
                {
                    Source = DetermineSource(data),
                    SourceId = DetermineSourceId(data),
                    SystemPropertyPath = data.SystemPropertyPath
                }
            };
        }

        private async Task<Dictionary<string, object>> CalculateAggregationsAsync(
            IQueryable<Module.Domain.Data.PropertyData> query,
            PropertyAggregation[] aggregations,
            CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, object>();

            if (aggregations == null || !aggregations.Any())
                return result;

            foreach (var agg in aggregations)
            {
                try
                {
                    var value = await CalculateSingleAggregationAsync(query, agg, cancellationToken).ConfigureAwait(false);
                    if (value != null)
                    {
                        result[agg.Alias ?? $"{agg.Function}_{agg.PropertyKey}"] = value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating aggregation {Function} for {PropertyKey}",
                        agg.Function, agg.PropertyKey);
                }
            }

            return result;
        }

        private async Task<object> CalculateSingleAggregationAsync(
            IQueryable<Module.Domain.Data.PropertyData> query,
            PropertyAggregation aggregation,
            CancellationToken cancellationToken)
        {
            switch (aggregation.Function?.ToUpper())
            {
                case "COUNT":
                    return await query.CountAsync(cancellationToken).ConfigureAwait(false);

                case "COUNTDISTINCT":
                    return await query.Select(pd => pd.GetValue()).Distinct().CountAsync(cancellationToken).ConfigureAwait(false);

                case "SUM":
                case "AVG":
                    {
                        // Ensure values are numeric
                        var numericValues = await query.Select(pd => pd.GetValue()).ToListAsync(cancellationToken).ConfigureAwait(false);
                        var doubleValues = numericValues.Select(v => TryConvertToDouble(v)).Where(d => d.HasValue).Select(d => d.Value).ToList();

                        if (!doubleValues.Any())
                            throw new InvalidOperationException($"No numeric values found for aggregation {aggregation.Function}.");

                        return aggregation.Function.ToUpper() == "SUM"
                            ? doubleValues.Sum()
                            : doubleValues.Average();
                    }

                case "MIN":
                    return await query.Select(pd => pd.GetValue()).MinAsync(cancellationToken).ConfigureAwait(false);

                case "MAX":
                    return await query.Select(pd => pd.GetValue()).MaxAsync(cancellationToken).ConfigureAwait(false);

                case "FIRST":
                    return await query.Select(pd => pd.GetValue()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                case "LAST":
                    // For LAST we should define an ordering, here we assume Id ordering
                    return await query.OrderBy(pd => pd.Id).Select(pd => pd.GetValue()).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                default:
                    throw new NotSupportedException($"Aggregation function '{aggregation.Function}' is not supported.");
            }
        }

        private double? TryConvertToDouble(object value)
        {
            if (value == null) return null;
            if (value is double d) return d;
            if (value is float f) return f;
            if (value is int i) return i;
            if (value is long l) return l;
            if (value is decimal dec) return (double)dec;

            double parsedDouble;
            return double.TryParse(value.ToString(), out parsedDouble) ? parsedDouble : null;
        }

        #endregion

        #region Helper Methods

        private string GenerateCacheKey(PropertyQueryModel query)
        {
            if (!string.IsNullOrEmpty(query.CachePolicy?.CacheKey))
                return query.CachePolicy.CacheKey;

            var keyBuilder = new StringBuilder("prop_query:");

            if (query.ApplicationId.HasValue)
                keyBuilder.Append($"app_{query.ApplicationId}_");

            if (query.WorkspaceId.HasValue)
                keyBuilder.Append($"ws_{query.WorkspaceId}_");

            if (query.ModuleId.HasValue)
                keyBuilder.Append($"mod_{query.ModuleId}_");

            if (query.Filter != null)
                keyBuilder.Append($"filter_{JsonSerializer.Serialize(query.Filter)}_");

            return keyBuilder.ToString();
        }

        private string DetermineSource(Module.Domain.Data.PropertyData data)
        {
            if (data.ModuleDataId.HasValue)
                return "Module";
            if (data.WorkspaceDataId.HasValue)
                return "Workspace";
            return "System";
        }

        private Guid DetermineSourceId(Module.Domain.Data.PropertyData data)
        {
            if (data.ModuleDataId.HasValue)
                return data.ModuleDataId.Value;
            if (data.WorkspaceDataId.HasValue)
                return data.WorkspaceDataId.Value;
            return data.Id;
        }

        private class DynamicGrouping
        {
            public object Key0 { get; set; }
            public object Key1 { get; set; }
            public object Key2 { get; set; }
            public object Key3 { get; set; }
            public object Key4 { get; set; }
        }

        #endregion
    }
}
