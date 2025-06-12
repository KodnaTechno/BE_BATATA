using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Module.Domain.Schema.Properties;
using Module;
using Microsoft.EntityFrameworkCore;
using NCalc;
using System.Text.RegularExpressions;
using AppCommon.EnumShared;

namespace Module.Service
{
    public class FormulaCalculationService
    {
        private readonly ModuleDbContext _dbContext;
        private readonly Dictionary<string, object> _cache = new();
        // Dependency graph: property key -> set of dependent property keys
        private readonly Dictionary<string, HashSet<string>> _dependencyGraph = new();
        private readonly Dictionary<string, Property> _propertyKeyMap = new();
        private bool _graphBuilt = false;

        public FormulaCalculationService(ModuleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Call this on startup or when property definitions change
        public async Task BuildDependencyGraphAsync()
        {
            _dependencyGraph.Clear();
            _propertyKeyMap.Clear();
            var allProperties = await _dbContext.Properties.Include(p => p.PropertyFormulas).ToListAsync();
            foreach (var prop in allProperties)
            {
                _propertyKeyMap[prop.Key] = prop;
                if (prop.PropertyFormulas != null)
                {
                    foreach (var formula in prop.PropertyFormulas)
                    {
                        var dependencies = ExtractPropertyKeysFromFormula(formula.Formula);
                        foreach (var dep in dependencies)
                        {
                            if (!_dependencyGraph.ContainsKey(dep))
                                _dependencyGraph[dep] = new HashSet<string>();
                            _dependencyGraph[dep].Add(prop.Key);
                        }
                    }
                }
            }
            _graphBuilt = true;
        }

        // Extract property keys from formula using regex (e.g., [Task_ActualProgress])
        private static IEnumerable<string> ExtractPropertyKeysFromFormula(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula)) yield break;
            var matches = Regex.Matches(formula, @"\[([A-Za-z0-9_\.]+)\]");
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }

        // Topological sort for recalculation order
        private List<string> GetRecalculationOrder(IEnumerable<string> changedKeys)
        {
            var visited = new HashSet<string>();
            var result = new List<string>();
            void Visit(string key)
            {
                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    if (_dependencyGraph.TryGetValue(key, out var dependents))
                    {
                        foreach (var dep in dependents)
                            Visit(dep);
                    }
                    result.Add(key);
                }
            }
            foreach (var key in changedKeys)
                Visit(key);
            return result;
        }

        // Recalculate all system properties (for daily background job)
        public async Task RecalculateAllSystemPropertiesAsync()
        {
            if (!_graphBuilt) await BuildDependencyGraphAsync();
            var allCalculatedProps = _propertyKeyMap.Values
                .Where(p => p.IsCalculated && p.PropertyFormulas.Any())
                .ToList();
            
            var order = GetRecalculationOrder(allCalculatedProps.Select(p => p.Key));

            // This method needs to identify all instances for each property.
            // For simplicity, this example will assume properties are global or it needs
            // to iterate over all ModuleData, WorkspaceData etc. to find relevant instances.
            // This is a complex operation if not using the Parallel version or instance-specific versions.
            // Consider deprecating or redesigning if instance-level recalculation is preferred.

            foreach (var key in order)
            {
                if (_propertyKeyMap.TryGetValue(key, out var prop))
                {
                    // TODO: Identify all target instances (ModuleData, WorkspaceData etc.) for this property 'prop'
                    // For each instance:
                    //   var instanceContext = ...; (e.g., { "ModuleDataId", moduleData.Id })
                    //   var preloadedData = await PreloadDataForInstanceContext(instanceContext);
                    //   var value = await CalculatePropertyFormulaAsync(prop, prop.ApplicationId, prop.WorkspaceId, prop.ModuleId, prop.WorkspaceModuleId, instanceContext, preloadedData);
                    //   await SavePropertyValueAsync(prop, instanceContext, value);
                    await CalculateAndPersistPropertyAsync(prop); // Original call, needs rework as described above.
                }
            }
        }

        // Optimized: Recalculate all system properties in parallel (for large sets)
        public async Task RecalculateAllSystemPropertiesParallelAsync()
        {
            if (!_graphBuilt) await BuildDependencyGraphAsync();
            
            var allCalculatedProps = await _dbContext.Properties
                .Include(p => p.PropertyFormulas)
                .Where(p => p.IsCalculated && p.PropertyFormulas.Any())
                .ToListAsync();
            
            if (!allCalculatedProps.Any()) return;

            var allPropertyDataInstances = await _dbContext.PropertyData
                .Include(pd => pd.ModuleData).ThenInclude(md => md.Module)
                .Include(pd => pd.ModuleData).ThenInclude(md => md.WorkspaceData).ThenInclude(wd => wd.Workspace)
                .Include(pd => pd.WorkspaceData).ThenInclude(wd => wd.Workspace)
                .ToListAsync();

            var order = GetRecalculationOrder(allCalculatedProps.Select(p => p.Key));
            
            // Group PropertyData by their instance (ModuleDataId or WorkspaceDataId) to create contexts
            var instanceContexts = new List<Dictionary<string, object>>();
            var moduleDataIds = allPropertyDataInstances.Where(pd => pd.ModuleDataId.HasValue).Select(pd => pd.ModuleDataId.Value).Distinct();
            foreach (var mId in moduleDataIds)
            {
                var md = allPropertyDataInstances.First(pd => pd.ModuleDataId == mId).ModuleData;
                var context = new Dictionary<string, object> { { "ModuleDataId", mId } };
                if (md.WorkSpaceDataId.HasValue) context["WorkspaceDataId"] = md.WorkSpaceDataId.Value;
                instanceContexts.Add(context);
            }
            var workspaceDataIds = allPropertyDataInstances.Where(pd => pd.WorkspaceDataId.HasValue && !allPropertyDataInstances.Any(x => x.ModuleData != null && x.ModuleData.WorkSpaceDataId == pd.WorkspaceDataId.Value)).Select(pd => pd.WorkspaceDataId.Value).Distinct();
            foreach (var wId in workspaceDataIds)
            {
                instanceContexts.Add(new Dictionary<string, object> { { "WorkspaceDataId", wId } });
            }
            // TODO: Add ApplicationData contexts if applicable

            var propertyDataLookup = allPropertyDataInstances.GroupBy(pd => pd.PropertyId)
                                       .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var key in order) // Process properties in dependency order
            {
                var propToCalculate = allCalculatedProps.FirstOrDefault(p => p.Key == key);
                if (propToCalculate == null) continue;

                var tasks = new List<Task>();
                // For each instance context, calculate this property if it applies
                foreach (var instanceContext in instanceContexts)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        // Determine if this property applies to this instance context
                        bool applies = false;
                        if (propToCalculate.ModuleId.HasValue && instanceContext.ContainsKey("ModuleDataId")) applies = true;
                        else if (propToCalculate.WorkspaceId.HasValue && instanceContext.ContainsKey("WorkspaceDataId")) applies = true;
                        // TODO: Add Application scope check

                        if (applies)
                        {
                            var value = await CalculatePropertyFormulaAsync(
                                propToCalculate,
                                propToCalculate.ApplicationId, // These are definition IDs
                                propToCalculate.WorkspaceId,
                                propToCalculate.ModuleId,
                                propToCalculate.WorkspaceModuleId,
                                instanceContext,
                                propertyDataLookup // Pass all loaded PropertyData
                            );
                            // Corrected call to SavePropertyValueAsync
                            var currentInstanceContext = new InstanceContextIds(
                                instanceContext.TryGetValue("ModuleDataId", out var mId) ? (Guid?)mId : null,
                                instanceContext.TryGetValue("WorkspaceDataId", out var wId) ? (Guid?)wId : null,
                                instanceContext.TryGetValue("ApplicationDataId", out var aId) ? (Guid?)aId : null,
                                // Assuming ApplicationDefinitionId might be derived or passed differently for parallel processing
                                // For now, it's not directly available in this simplified instanceContext dictionary.
                                // This might need adjustment based on how ApplicationDefinitionId is determined for each instance.
                                propToCalculate.ApplicationId 
                            );
                            await SavePropertyValueAsync(propToCalculate, currentInstanceContext, value, propertyDataLookup);
                        }
                    }));
                }
                (applicationId.HasValue && p.ApplicationId == applicationId) ||
                (workspaceId.HasValue && p.WorkspaceId == workspaceId) ||
                (moduleId.HasValue && p.ModuleId == moduleId) ||
                (workspaceModuleId.HasValue && p.WorkspaceModuleId == workspaceModuleId)
            ).Where(p => p.IsCalculated && p.PropertyFormulas.Any()).ToList();
            var order = GetRecalculationOrder(props.Select(p => p.Key));
            foreach (var key in order)
            {
                if (_propertyKeyMap.TryGetValue(key, out var prop))
                {
                    await CalculateAndPersistPropertyAsync(prop, applicationId, workspaceId, moduleId, workspaceModuleId);
                }
            }
        }

        // Recalculate all calculated properties for a specific module instance (e.g., a task instance)
        public async Task RecalculateForModuleInstanceAsync(Guid moduleDefinitionId, Guid moduleInstanceId)
        {
            if (!_graphBuilt) await BuildDependencyGraphAsync();

            var moduleInstance = await _dbContext.ModuleData
                .AsNoTracking()
                .Include(md => md.Module)
                .Include(md => md.WorkspaceData).ThenInclude(wd => wd.Workspace)
                .FirstOrDefaultAsync(md => md.Id == moduleInstanceId);

            if (moduleInstance == null || moduleInstance.Module.Id != moduleDefinitionId) return;

            var calculatedPropsForModule = await _dbContext.Properties
                .Include(p => p.PropertyFormulas)
                .Where(p => p.ModuleId == moduleDefinitionId && p.IsCalculated && p.PropertyFormulas.Any())
                .ToListAsync();

            if (!calculatedPropsForModule.Any()) return;
            
            var order = GetRecalculationOrder(calculatedPropsForModule.Select(p => p.Key));
            
            var instanceContext = new Dictionary<string, object> { { "ModuleDataId", moduleInstanceId } };
            if (moduleInstance.WorkSpaceDataId.HasValue)
            {
                instanceContext["WorkspaceDataId"] = moduleInstance.WorkSpaceDataId.Value;
            }

            var preloadedData = await PreloadDataForInstanceContext(instanceContext);

            foreach (var key in order)
            {
                var propToCalculate = calculatedPropsForModule.FirstOrDefault(p => p.Key == key);
                if (propToCalculate == null) continue;

                var value = await CalculatePropertyFormulaAsync(
                    propToCalculate,
                    moduleInstance.Module.ApplicationId ?? moduleInstance.WorkspaceData?.Workspace?.ApplicationId,
                    moduleInstance.WorkspaceData?.WorkspaceId,
                    moduleDefinitionId,
                    null, 
                    instanceContext,
                    preloadedData);

                await SavePropertyValueAsync(propToCalculate, instanceContext, value, preloadedData);
            }
            await _dbContext.SaveChangesAsync();
        }

        // Recalculate all calculated properties for a specific workspace instance
        public async Task RecalculateForWorkspaceInstanceAsync(Guid workspaceDefinitionId, Guid workspaceInstanceId)
        {
            if (!_graphBuilt) await BuildDependencyGraphAsync();

            var workspaceInstance = await _dbContext.WorkspaceData
                .AsNoTracking()
                .Include(wd => wd.Workspace).ThenInclude(w => w.Application)
                .FirstOrDefaultAsync(wd => wd.Id == workspaceInstanceId);

            if (workspaceInstance == null || workspaceInstance.Workspace.Id != workspaceDefinitionId) return;

            var calculatedPropsForWorkspace = await _dbContext.Properties
                .Include(p => p.PropertyFormulas)
                .Where(p => p.WorkspaceId == workspaceDefinitionId && p.IsCalculated && p.PropertyFormulas.Any())
                .ToListAsync();

            if (!calculatedPropsForWorkspace.Any()) return;

            var order = GetRecalculationOrder(calculatedPropsForWorkspace.Select(p => p.Key));

            var instanceContext = new Dictionary<string, object> { { "WorkspaceDataId", workspaceInstanceId } };
            // Application context might be needed if app-level properties are involved
            // Guid? applicationInstanceId = null; // If applications have their own "Data" instances
            
            var preloadedData = await PreloadDataForInstanceContext(instanceContext);

            foreach (var key in order)
            {
                var propToCalculate = calculatedPropsForWorkspace.FirstOrDefault(p => p.Key == key);
                if (propToCalculate == null) continue;

                var value = await CalculatePropertyFormulaAsync(
                    propToCalculate,
                    workspaceInstance.Workspace.ApplicationId,
                    workspaceDefinitionId,
                    null,
                    null,
                    instanceContext,
                    preloadedData);
                
                await SavePropertyValueAsync(propToCalculate, instanceContext, value, preloadedData);
            }
            await _dbContext.SaveChangesAsync();
        }

        // Recalculate all calculated properties for a specific application instance
        public async Task RecalculateForApplicationInstanceAsync(Guid applicationDefinitionId, Guid applicationInstanceId)
        {
            // This method assumes an "ApplicationData" entity or similar concept for application instances
            // if application-level properties are instance-specific and not just global definitions.
            // The current model doesn't explicitly show ApplicationData, so this is a template.

            if (!_graphBuilt) await BuildDependencyGraphAsync();
            
            // Example: var applicationInstance = await _dbContext.ApplicationData.FindAsync(applicationInstanceId);
            // if (applicationInstance == null) return;

            var calculatedPropsForApp = await _dbContext.Properties
                .Include(p => p.PropertyFormulas)
                .Where(p => p.ApplicationId == applicationDefinitionId && p.IsCalculated && p.PropertyFormulas.Any())
                .ToListAsync();

            if (!calculatedPropsForApp.Any()) return;

            var order = GetRecalculationOrder(calculatedPropsForApp.Select(p => p.Key));
            
            var instanceContext = new Dictionary<string, object> { { "ApplicationDataId", applicationInstanceId } };
            var preloadedData = await PreloadDataForInstanceContext(instanceContext); // Might need to preload all global app props

            foreach (var key in order)
            {
                var propToCalculate = calculatedPropsForApp.FirstOrDefault(p => p.Key == key);
                if (propToCalculate == null) continue;

                var value = await CalculatePropertyFormulaAsync(
                    propToCalculate,
                    applicationDefinitionId,
                    null,
                    null,
                    null,
                    instanceContext,
                    preloadedData);

                // await SavePropertyValueAsync(propToCalculate, instanceContext, value, preloadedData);
                // TODO: Implement SavePropertyValueAsync for ApplicationData if it exists
            }
            // await _dbContext.SaveChangesAsync();
        }
        
        private async Task<Dictionary<Guid, List<Module.Domain.Data.PropertyData>>> PreloadDataForInstanceContext(Dictionary<string, object> instanceContextIds)
        {
            var preloadedData = new Dictionary<Guid, List<Module.Domain.Data.PropertyData>>();
            var propertyIdsToFetch = new HashSet<Guid>();

            Guid? moduleDataId = instanceContextIds.TryGetValue("ModuleDataId", out var mId) ? (Guid?)mId : null;
            Guid? workspaceDataId = instanceContextIds.TryGetValue("WorkspaceDataId", out var wId) ? (Guid?)wId : null;
            Guid? applicationDataId = instanceContextIds.TryGetValue("ApplicationDataId", out var aId) ? (Guid?)aId : null; // Assuming ApplicationDataId

            // Define a broader set of properties that might be needed (e.g., all properties in the hierarchy)
            // For simplicity, fetching based on known instance IDs. A more robust way would be to parse all formulas
            // to determine exactly which property keys are needed and then map them to property IDs.

            var query = _dbContext.PropertyData.AsQueryable();
            bool hasCondition = false;

            if (moduleDataId.HasValue)
            {
                query = query.Where(pd => pd.ModuleDataId == moduleDataId.Value);
                hasCondition = true;
            }
            if (workspaceDataId.HasValue)
            {
                query = hasCondition ? query.Union(_dbContext.PropertyData.Where(pd => pd.WorkspaceDataId == workspaceDataId.Value))
                                     : query.Where(pd => pd.WorkspaceDataId == workspaceDataId.Value);
                hasCondition = true;
            }
            // TODO: Add ApplicationDataId based preloading if applicable

            if (hasCondition)
            {
                var data = await query.ToListAsync();
                foreach (var group in data.GroupBy(pd => pd.PropertyId))
                {
                    preloadedData[group.Key] = group.ToList();
                }
            }
            return preloadedData;
        }

        private async Task SavePropertyValueAsync(
            Property propertyDefinition,
            Dictionary<string, object> instanceContextIds,
            object value,
            Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedDataCache)
        {
            Guid? moduleDataId = instanceContextIds.TryGetValue("ModuleDataId", out var mId) ? (Guid?)mId : null;
            Guid? workspaceDataId = instanceContextIds.TryGetValue("WorkspaceDataId", out var wId) ? (Guid?)wId : null;
            // Guid? applicationDataId = ...

            Module.Domain.Data.PropertyData propDataInstance = null;

            // Try to find in preloaded data first to update the tracked entity
            if (preloadedDataCache.TryGetValue(propertyDefinition.Id, out var cachedList))
            {
                if (moduleDataId.HasValue) 
                    propDataInstance = cachedList.FirstOrDefault(pd => pd.ModuleDataId == moduleDataId && pd.PropertyId == propertyDefinition.Id);
                else if (workspaceDataId.HasValue)
                    propDataInstance = cachedList.FirstOrDefault(pd => pd.WorkspaceDataId == workspaceDataId && pd.PropertyId == propertyDefinition.Id);
                // TODO: ApplicationDataId case
            }

            if (propDataInstance == null) // Not in cache or not tracked, fetch from DB to update
            {
                 if (moduleDataId.HasValue)
                    propDataInstance = await _dbContext.PropertyData.FirstOrDefaultAsync(pd => pd.ModuleDataId == moduleDataId && pd.PropertyId == propertyDefinition.Id);
                else if (workspaceDataId.HasValue)
                    propDataInstance = await _dbContext.PropertyData.FirstOrDefaultAsync(pd => pd.WorkspaceDataId == workspaceDataId && pd.PropertyId == propertyDefinition.Id);
                // TODO: ApplicationDataId case
            }

            if (propDataInstance == null) // Still null, so create new
            {
                propDataInstance = new Module.Domain.Data.PropertyData
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyDefinition.Id,
                    DataType = propertyDefinition.DataType,
                    ViewType = propertyDefinition.ViewType,
                };
                if (moduleDataId.HasValue) propDataInstance.ModuleDataId = moduleDataId;
                else if (workspaceDataId.HasValue) propDataInstance.WorkspaceDataId = workspaceDataId;
                // TODO: Set ApplicationDataId if applicable
                _dbContext.PropertyData.Add(propDataInstance);
            }
            propDataInstance.SetValue(value?.ToString());

            // Update cache for subsequent calculations in the same batch
            if (!preloadedDataCache.ContainsKey(propertyDefinition.Id))
            {
                preloadedDataCache[propertyDefinition.Id] = new List<Module.Domain.Data.PropertyData>();
            }
            var existingInCache = preloadedDataCache[propertyDefinition.Id].FirstOrDefault(pd => pd.Id == propDataInstance.Id);
            if (existingInCache == null)
            {
                preloadedDataCache[propertyDefinition.Id].Add(propDataInstance);
            }
            else // If it was already there, ensure it's the same instance or update its value
            {
                existingInCache.SetValue(value?.ToString());
            }
        }

        // Recalculate all calculated properties for a specific application instance
        public async Task RecalculateForApplicationInstanceAsync(Guid applicationDefinitionId, Guid applicationInstanceId)
        {
            if (!_graphBuilt) await BuildDependencyGraphAsync();
            var calculatedProps = await _dbContext.Properties
                .Where(p => p.ApplicationId == applicationDefinitionId && p.IsCalculated && p.PropertyFormulas.Any())
                .ToListAsync();
            if (!calculatedProps.Any()) return;
            var allKeys = GetRecalculationOrder(calculatedProps.Select(p => p.Key));
            // ApplicationData not shown in your model, so this is a placeholder for your actual application instance data
            foreach (var key in allKeys)
            {
                var prop = calculatedProps.FirstOrDefault(p => p.Key == key);
                if (prop == null) continue;
                var value = await CalculatePropertyFormulaAsync(prop, applicationDefinitionId, null, null, null, new Dictionary<string, object> { ["ApplicationDataId"] = applicationInstanceId });
                // TODO: Update or insert PropertyData for application instance (implement as needed)
            }
            // await _dbContext.SaveChangesAsync();
        }

        // Calculate and persist property value
        private async Task CalculateAndPersistPropertyAsync(Property property, Guid? applicationId = null, Guid? workspaceId = null, Guid? moduleId = null, Guid? workspaceModuleId = null)
        {
            // This method needs significant rework to:
            // 1. Identify ALL relevant instances (ModuleData, WorkspaceData) for the given scope.
            // 2. For EACH instance:
            //    a. Construct its specific instanceContext (e.g., { "ModuleDataId", id })
            //    b. Preload data for that instance context.
            //    c. Call the main CalculatePropertyFormulaAsync.
            //    d. Save the value using SavePropertyValueAsync.
            // 3. Batch SaveChangesAsync at the very end.
            // Example for a single property definition, assuming it's for a module:
            if (moduleId.HasValue)
            {
                var moduleInstances = await _dbContext.ModuleData.Where(md => md.ModuleId == moduleId).ToListAsync();
                foreach (var instance in moduleInstances)
                {
                    var instanceContext = new Dictionary<string, object> { { "ModuleDataId", instance.Id } };
                    if (instance.WorkSpaceDataId.HasValue) instanceContext["WorkspaceDataId"] = instance.WorkSpaceDataId.Value;
                    
                    var preloadedData = await PreloadDataForInstanceContext(instanceContext);
                    var value = await CalculatePropertyFormulaAsync(property, applicationId, workspaceId, moduleId, workspaceModuleId, instanceContext, preloadedData);
                    await SavePropertyValueAsync(property, instanceContext, value, preloadedData);
                }
                await _dbContext.SaveChangesAsync(); // Batch save after processing all instances for this property
            }
            // Similar logic for workspaceId, applicationId scopes.
        }

        // Add common functions to NCalc
        private void AddCommonFunctions(Expression expr)
        {
            expr.EvaluateFunction += (name, args) =>
            {
                switch (name.ToLower())
                {
                    case "avg":
                        if (args.Parameters.Length == 2)
                        {
                            var a = Convert.ToDouble(args.Parameters[0].Evaluate());
                            var b = Convert.ToDouble(args.Parameters[1].Evaluate());
                            args.Result = (a + b) / 2.0;
                        }
                        else if (args.Parameters.Length > 0)
                        {
                            double sum = 0;
                            foreach (var p in args.Parameters) sum += Convert.ToDouble(p.Evaluate());
                            args.Result = sum / args.Parameters.Length;
                        }
                        break;
                    case "sum":
                        double total = 0;
                        foreach (var p in args.Parameters) total += Convert.ToDouble(p.Evaluate());
                        args.Result = total;
                        break;
                    case "min":
                        args.Result = args.Parameters.Select(p => Convert.ToDouble(p.Evaluate())).Min();
                        break;
                    case "max":
                        args.Result = args.Parameters.Select(p => Convert.ToDouble(p.Evaluate())).Max();
                        break;
                    case "datediff":
                        // Usage: DateDiff('d', date1, date2)
                        var type = args.Parameters[0].Evaluate().ToString();
                        var d1 = Convert.ToDateTime(args.Parameters[1].Evaluate());
                        var d2 = Convert.ToDateTime(args.Parameters[2].Evaluate());
                        if (type == "d") args.Result = (d2 - d1).Days;
                        else if (type == "h") args.Result = (d2 - d1).TotalHours;
                        else if (type == "m") args.Result = (d2 - d1).TotalMinutes;
                        else args.Result = (d2 - d1).TotalSeconds;
                        break;
                    // Add more as needed
                }
            };
        }

        // Add domain-specific functions for top-down (workspace/application) aggregation
        private void AddDomainFunctions(Expression expr, InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            expr.EvaluateFunction += async (name, args) =>
            {
                // These currentInstance... Ids are the *instance IDs* of the workspace/app
                // the currently calculated property belongs to, or is contextually under.
                // The domain functions might use these to filter their queries.
                switch (name.ToLower())
                {
                    case "countmodule":
                        // Usage: CountModule('ModuleKey')
                        // This function likely counts module instances under currentInstanceWorkspaceId or currentInstanceApplicationId
                        var moduleKey = args.Parameters[0].Evaluate().ToString();
                        args.Result = await CountModuleInstancesAsync(moduleKey, instanceContextIds, preloadedPropertyData);
                        break;
                    case "sumproperty":
                        // Usage: SumProperty('ModuleKey', 'PropertyKey', 'DatePropertyKey', 'dateCondition')
                        args.Result = await SumPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate().ToString(), // Optional: datePropertyKey
                            args.Parameters.Length > 3 ? args.Parameters[3].Evaluate().ToString() : null, // Optional: dateCondition
                            instanceContextIds, preloadedPropertyData);
                        break;
                    case "avgproperty":
                        args.Result = await AvgPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate().ToString(),
                            args.Parameters.Length > 3 ? args.Parameters[3].Evaluate().ToString() : null,
                            instanceContextIds, preloadedPropertyData);
                        break;
                    case "minproperty":
                        args.Result = await MinPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate().ToString(),
                            args.Parameters.Length > 3 ? args.Parameters[3].Evaluate().ToString() : null,
                            instanceContextIds, preloadedPropertyData);
                        break;
                    case "maxproperty":
                        args.Result = await MaxPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate().ToString(),
                            args.Parameters.Length > 3 ? args.Parameters[3].Evaluate().ToString() : null,
                            instanceContextIds, preloadedPropertyData);
                        break;
                    case "countproperty":
                        args.Result = await CountPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate(),
                            instanceContextIds, preloadedPropertyData);
                        break;
                    case "existsproperty":
                        args.Result = await ExistsPropertyOnModuleAsync(
                            args.Parameters[0].Evaluate().ToString(),
                            args.Parameters[1].Evaluate().ToString(),
                            args.Parameters[2].Evaluate(),
                            instanceContextIds, preloadedPropertyData);
                        break;
                    // Add more as needed
                }
            };
        }

        private async Task<double> AvgPropertyOnModuleAsync(string moduleKey, string propertyKey, string datePropertyKey, string dateCondition, 
            InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var values = await GetFilteredPropertyValues(moduleKey, propertyKey, datePropertyKey, dateCondition, instanceContextIds, preloadedPropertyData);
            return values.Any() ? values.Average() : 0;
        }
        private async Task<double> MinPropertyOnModuleAsync(string moduleKey, string propertyKey, string datePropertyKey, string dateCondition, 
            InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var values = await GetFilteredPropertyValues(moduleKey, propertyKey, datePropertyKey, dateCondition, instanceContextIds, preloadedPropertyData);
            return values.Any() ? values.Min() : 0;
        }
        private async Task<double> MaxPropertyOnModuleAsync(string moduleKey, string propertyKey, string datePropertyKey, string dateCondition, 
            InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var values = await GetFilteredPropertyValues(moduleKey, propertyKey, datePropertyKey, dateCondition, instanceContextIds, preloadedPropertyData);
            return values.Any() ? values.Max() : 0;
        }
        private async Task<int> CountPropertyOnModuleAsync(string moduleKey, string propertyKey, object value, InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var query = _dbContext.ModuleData.AsNoTracking().Where(md => md.Module.Key == moduleKey);
            if (instanceContextIds.WorkspaceDataId.HasValue)
                query = query.Where(md => md.WorkSpaceDataId == instanceContextIds.WorkspaceDataId.Value);
            if (instanceContextIds.ApplicationDefinitionId.HasValue)
                query = query.Where(md => md.Module.ApplicationId == instanceContextIds.ApplicationDefinitionId.Value);
            var moduleIds = await query.Select(md => md.Id).ToListAsync();
            var propData = _dbContext.PropertyData.AsNoTracking().Where(pd => moduleIds.Contains(pd.ModuleDataId.Value) && pd.Property.Key == propertyKey);
            return await propData.CountAsync(pd => pd.StringValue == value?.ToString());
        }
        private async Task<bool> ExistsPropertyOnModuleAsync(string moduleKey, string propertyKey, object value, InstanceContextIds instanceContextIds, Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var query = _dbContext.ModuleData.AsNoTracking().Where(md => md.Module.Key == moduleKey);
            if (instanceContextIds.WorkspaceDataId.HasValue)
                query = query.Where(md => md.WorkSpaceDataId == instanceContextIds.WorkspaceDataId.Value);
            if (instanceContextIds.ApplicationDefinitionId.HasValue)
                query = query.Where(md => md.Module.ApplicationId == instanceContextIds.ApplicationDefinitionId.Value);
            var moduleIds = await query.Select(md => md.Id).ToListAsync();
            var propData = _dbContext.PropertyData.AsNoTracking().Where(pd => moduleIds.Contains(pd.ModuleDataId.Value) && pd.Property.Key == propertyKey);
            return await propData.AnyAsync(pd => pd.StringValue == value?.ToString());
        }
        private async Task<List<double>> GetFilteredPropertyValues(
            string moduleKey, 
            string propertyKeyToAggregate, 
            string datePropertyKey, 
            string dateCondition, 
            InstanceContextIds instanceContextIds, 
            Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            // This method's effectiveness with preloadedPropertyData depends on whether that cache contains
            // PropertyData for all child modules of the current context (e.g., workspace).
            // If not, DB queries are still necessary. The focus here is using instanceContextIds for accurate DB query scoping.

            var query = _dbContext.ModuleData.AsNoTracking().Where(md => md.Module.Key == moduleKey);

            if (instanceContextIds.WorkspaceDataId.HasValue)
            {
                query = query.Where(md => md.WorkSpaceDataId == instanceContextIds.WorkspaceDataId.Value);
            }
            else if (instanceContextIds.ApplicationDefinitionId.HasValue) // Fallback to application definition scope if workspace not specified
            {
                // This assumes Module.ApplicationId links to the ApplicationDefinitionId
                query = query.Where(md => md.Module.ApplicationId == instanceContextIds.ApplicationDefinitionId.Value);
            }
            else
            {
                // No workspace or application scope provided for filtering modules, this might return too many.
                // Consider if this case should be an error or return empty. For now, proceeds without this filter.
            }
            
            var moduleInstances = await query.Select(md => md.Id).ToListAsync();

            if (!moduleInstances.Any()) return new List<double>();

            var propDataQuery = _dbContext.PropertyData.AsNoTracking()
                                .Where(pd => pd.ModuleDataId.HasValue && moduleInstances.Contains(pd.ModuleDataId.Value) && 
                                             pd.Property.Key == propertyKeyToAggregate);

            if (!string.IsNullOrEmpty(datePropertyKey) && !string.IsNullOrEmpty(dateCondition))
            {
                var datePropDataQuery = _dbContext.PropertyData.AsNoTracking()
                                        .Where(pd => pd.ModuleDataId.HasValue && moduleInstances.Contains(pd.ModuleDataId.Value) && 
                                                     pd.Property.Key == datePropertyKey);
                
                var today = DateTime.UtcNow.Date;
                List<Guid> validModuleDataIdsForDateCondition = new List<Guid>();

                if (dateCondition == "<today")
                {
                    validModuleDataIdsForDateCondition = await datePropDataQuery
                        .Where(pd => pd.DateValue.Value< today)
                        .Select(pd => pd.ModuleDataId.Value)
                        .Distinct()
                        .ToListAsync();
                }
                else if (dateCondition == ">today")
                {
                     validModuleDataIdsForDateCondition = await datePropDataQuery
                        .Where(pd => pd.DateValue > today)
                        .Select(pd => pd.ModuleDataId.Value)
                        .Distinct()
                        .ToListAsync();
                }
                propDataQuery = propDataQuery.Where(pd => pd.ModuleDataId.HasValue && validModuleDataIdsForDateCondition.Contains(pd.ModuleDataId.Value));
            }
            
            return await propDataQuery.Select(pd => pd.DoubleValue ?? 0).ToListAsync();
        }

        public async Task<object> CalculatePropertyFormulaAsync(
            Property propertyToCalculate, 
            Guid? contextApplicationDefinitionId,    
            Guid? contextWorkspaceDefinitionId,      
            Guid? contextModuleDefinitionId,         
            Guid? contextWorkspaceModuleDefinitionId,
            Dictionary<string, object> instanceContextIdsAsDict, 
            Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyDataForContext)
        {
            var formula = propertyToCalculate.PropertyFormulas?.OrderBy(f => f.Order).FirstOrDefault()?.Formula;
            if (string.IsNullOrWhiteSpace(formula))
            {
                // If IsCalculated is true but no formula, return default. Otherwise, could be an error.
                return !string.IsNullOrEmpty(propertyToCalculate.DefaultValue) ? ConvertValue(propertyToCalculate.DefaultValue, propertyToCalculate.DataType)
                                                                                : GetDefaultValueForType(propertyToCalculate.DataType);
            }

            var parameters = await BuildParameterDictionaryAsync(
                                propertyToCalculate, 
                                instanceContextIdsAsDict, 
                                preloadedPropertyDataForContext);
            
            var expr = new Expression(formula);
            expr.Parameters = parameters; // Set all resolved parameters directly

            // Resolve current instance IDs for domain functions
            Guid? currentModuleDataId = instanceContextIdsAsDict.TryGetValue("ModuleDataId", out var mId) ? (Guid?)mId : null;
            Guid? currentWorkspaceDataId = instanceContextIdsAsDict.TryGetValue("WorkspaceDataId", out var wId) ? (Guid?)wId : null;
            Guid? currentApplicationDataId = instanceContextIdsAsDict.TryGetValue("ApplicationDataId", out var aId) ? (Guid?)aId : null;

            // If calculating for a module, ensure its parent workspace instance ID is available for domain functions
            if (currentModuleDataId.HasValue && !currentWorkspaceDataId.HasValue)
            {
                var moduleInstance = await _dbContext.ModuleData.AsNoTracking().Select(md => new { md.Id, md.WorkSpaceDataId }).FirstOrDefaultAsync(md => md.Id == currentModuleDataId.Value);
                currentWorkspaceDataId = moduleInstance?.WorkSpaceDataId;
            }
            // If a workspace (either directly or via module) is involved, get its application definition ID for domain functions
            Guid? domainFunctionAppDefId = contextApplicationDefinitionId;
            if (currentWorkspaceDataId.HasValue && domainFunctionAppDefId == null)
            {
                 var wsInstance = await _dbContext.WorkspaceData.AsNoTracking().Include(w => w.Workspace).FirstOrDefaultAsync(w => w.Id == currentWorkspaceDataId.Value);
                 domainFunctionAppDefId = wsInstance?.Workspace?.ApplicationId;
            }


            var domainFunctionsContext = new InstanceContextIds(
                currentModuleDataId,
                currentWorkspaceDataId,
                currentApplicationDataId,
                domainFunctionAppDefId 
            );

            AddCommonFunctions(expr);
            AddDomainFunctions(expr, domainFunctionsContext, preloadedPropertyDataForContext); // Pass relevant instance/definition IDs
            
            return await EvaluateAsync(expr);
        }

        // Helper to support async evaluation for domain functions
        private async Task<object> EvaluateAsync(Expression expr)
        {
            // NCalc's EvaluateFunction can be async. If all functions are synchronous, this can be expr.Evaluate().
            // If there are async functions, NCalc handles their invocation.
            // The result of expr.Evaluate() would be Task<object> if async functions are used and awaited within NCalc's mechanism,
            // or object if they complete synchronously or NCalc's async handling is different.
            // For safety with async EvaluateFunction delegates:
            var evaluationTask = Task.Run(() => expr.Evaluate());
            return await evaluationTask;
        }

        private async Task<Dictionary<string, object>> BuildParameterDictionaryAsync(
            Property propertyToCalculate, // The property whose formula we are evaluating
            Dictionary<string, object> instanceContextIds, // e.g., { "ModuleDataId": guid, "WorkspaceDataId": guid, "ApplicationDataId": guid }
            Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            var parameters = new Dictionary<string, object>();
            var formula = propertyToCalculate.PropertyFormulas?.OrderBy(f => f.Order).FirstOrDefault()?.Formula;
            if (string.IsNullOrWhiteSpace(formula)) return parameters;

            var referencedPropertyKeys = ExtractPropertyKeysFromFormula(formula).Distinct().ToList();

            Guid? currentModuleDataId = instanceContextIds.TryGetValue("ModuleDataId", out var mId) ? (Guid?)mId : null;
            Guid? currentWorkspaceDataId = instanceContextIds.TryGetValue("WorkspaceDataId", out var wId) ? (Guid?)wId : null;
            Guid? currentApplicationDataId = instanceContextIds.TryGetValue("ApplicationDataId", out var aId) ? (Guid?)aId : null;

            if (currentModuleDataId.HasValue && !currentWorkspaceDataId.HasValue)
            {
                var moduleInstance = await _dbContext.ModuleData.AsNoTracking()
                                                .Select(md => new { md.Id, md.WorkSpaceDataId })
                                                .FirstOrDefaultAsync(md => md.Id == currentModuleDataId.Value);
                if (moduleInstance?.WorkSpaceDataId.HasValue == true)
                {
                    currentWorkspaceDataId = moduleInstance.WorkSpaceDataId.Value;
                }
            }
            // Note: Resolving currentApplicationDataId from currentWorkspaceDataId depends on your data model
            // (e.g., if WorkspaceData has a direct link to an ApplicationData instance or if it's inferred).
            // For now, we assume currentApplicationDataId is explicitly passed if relevant for this level.

            foreach (var key in referencedPropertyKeys)
            {
                if (!_propertyKeyMap.TryGetValue(key, out var referencedPropertyDefinition))
                {
                    parameters[key] = null; 
                    continue;
                }
                
                Guid? effectiveModuleDataId = null;
                Guid? effectiveWorkspaceDataId = null;
                Guid? effectiveApplicationDataId = null;

                if (referencedPropertyDefinition.ModuleId.HasValue)
                {
                    effectiveModuleDataId = currentModuleDataId;
                    effectiveWorkspaceDataId = currentWorkspaceDataId; 
                    effectiveApplicationDataId = currentApplicationDataId; 
                }
                else if (referencedPropertyDefinition.WorkspaceId.HasValue)
                {
                    effectiveWorkspaceDataId = currentWorkspaceDataId;
                    effectiveApplicationDataId = currentApplicationDataId; 
                }
                else if (referencedPropertyDefinition.ApplicationId.HasValue)
                {
                    effectiveApplicationDataId = currentApplicationDataId;
                }
                
                parameters[key] = await GetActualPropertyValueAsync(
                    referencedPropertyDefinition, 
                    effectiveModuleDataId, 
                    effectiveWorkspaceDataId, 
                    effectiveApplicationDataId, 
                    preloadedPropertyData);
            }
            return parameters;
        }

        private async Task<object> GetActualPropertyValueAsync(
            Property propertyDefinition, 
            Guid? currentModuleDataId,
            Guid? currentWorkspaceDataId,
            Guid? currentApplicationDataId, 
            Dictionary<Guid, List<Module.Domain.Data.PropertyData>> preloadedPropertyData)
        {
            Module.Domain.Data.PropertyData propertyDataInstance = null;

            if (preloadedPropertyData != null && preloadedPropertyData.TryGetValue(propertyDefinition.Id, out var dataList))
            {
                if (propertyDefinition.ModuleId.HasValue && currentModuleDataId.HasValue)
                {
                    propertyDataInstance = dataList.FirstOrDefault(pd => pd.ModuleDataId == currentModuleDataId.Value && pd.PropertyId == propertyDefinition.Id);
                }
                else if (propertyDefinition.WorkspaceId.HasValue && currentWorkspaceDataId.HasValue)
                {
                    propertyDataInstance = dataList.FirstOrDefault(pd => pd.WorkspaceDataId == currentWorkspaceDataId.Value && pd.PropertyId == propertyDefinition.Id);
                }
                else if (propertyDefinition.ApplicationId.HasValue)
                {
                    if (currentApplicationDataId.HasValue) {
                       propertyDataInstance = dataList.FirstOrDefault(pd => pd.ApplicationDataId == currentApplicationDataId.Value && pd.PropertyId == propertyDefinition.Id);
                    }
                    // Else: Handle lookup for global application properties if they are not instance-specific
                    // or if they are stored as PropertyData with ApplicationDataId == null.
                }
            }

            if (propertyDataInstance == null)
            {
                IQueryable<Module.Domain.Data.PropertyData> query = _dbContext.PropertyData.AsNoTracking()
                                                                        .Where(pd => pd.PropertyId == propertyDefinition.Id);

                if (propertyDefinition.ModuleId.HasValue && currentModuleDataId.HasValue)
                {
                    query = query.Where(pd => pd.ModuleDataId == currentModuleDataId.Value);
                    propertyDataInstance = await query.FirstOrDefaultAsync();
                }
                else if (propertyDefinition.WorkspaceId.HasValue && currentWorkspaceDataId.HasValue)
                {
                    query = query.Where(pd => pd.WorkspaceDataId == currentWorkspaceDataId.Value);
                    propertyDataInstance = await query.FirstOrDefaultAsync();
                }
                else if (propertyDefinition.ApplicationId.HasValue)
                {
                    if (currentApplicationDataId.HasValue) {
                        query = query.Where(pd => pd.ApplicationDataId == currentApplicationDataId.Value);
                        propertyDataInstance = await query.FirstOrDefaultAsync();
                    }
                    // Else: Handle DB query for global application properties.
                }
            }

            if (propertyDataInstance != null)
            {
                return ConvertValue(propertyDataInstance.GetValue(), propertyDefinition.DataType);
            }

            if (!string.IsNullOrEmpty(propertyDefinition.DefaultValue))
            {
                return ConvertValue(propertyDefinition.DefaultValue, propertyDefinition.DataType);
            }
            
            return GetDefaultValueForType(propertyDefinition.DataType);
        }

        // Define a helper class/struct for InstanceContextIds
        private class InstanceContextIds
        {
            public Guid? ModuleDataId { get; }
            public Guid? WorkspaceDataId { get; }
            public Guid? ApplicationDataId { get; } // Instance ID for application-specific data
            public Guid? ApplicationDefinitionId { get; } // Definition ID for application

            public InstanceContextIds(Guid? moduleDataId, Guid? workspaceDataId, Guid? applicationDataId, Guid? applicationDefinitionId)
            {
                ModuleDataId = moduleDataId;
                WorkspaceDataId = workspaceDataId;
                ApplicationDataId = applicationDataId;
                ApplicationDefinitionId = applicationDefinitionId;
            }
        }

        private object ConvertValue(string value, DataTypeEnum targetType)
        {
            if (value == null) return GetDefaultValueForType(targetType);
            switch (targetType)
            {
                case DataTypeEnum.String: return value;
                case DataTypeEnum.Int: return int.TryParse(value, out var i) ? i : GetDefaultValueForType(targetType);
                case DataTypeEnum.Double: return double.TryParse(value, out var d) ? d : GetDefaultValueForType(targetType);
                case DataTypeEnum.Decimal: return decimal.TryParse(value, out var dec) ? dec : GetDefaultValueForType(targetType);
                case DataTypeEnum.Bool: return bool.TryParse(value, out var b) ? b : GetDefaultValueForType(targetType);
                case DataTypeEnum.Guid: return Guid.TryParse(value, out var g) ? g : GetDefaultValueForType(targetType);
                case DataTypeEnum.DateTime: return DateTime.TryParse(value, out var dt) ? dt : GetDefaultValueForType(targetType);
                case DataTypeEnum.DateOnly: return DateOnly.TryParse(value, out var d_o) ? d_o : GetDefaultValueForType(targetType);
                default: return null;
            }
        }
        
        private object GetDefaultValueForType(DataTypeEnum dataType)
        {
            switch (dataType)
            {
                case DataTypeEnum.String: return string.Empty;
                case DataTypeEnum.Int: return 0;
                case DataTypeEnum.Double: return 0.0;
                case DataTypeEnum.Decimal: return 0.0m;
                case DataTypeEnum.Bool: return false;
                case DataTypeEnum.Guid: return Guid.Empty;
                case DataTypeEnum.DateTime: return DateTime.MinValue;
                case DataTypeEnum.DateOnly: return DateOnly.MinValue;
                default: return null;
            }
        }
    }
}
