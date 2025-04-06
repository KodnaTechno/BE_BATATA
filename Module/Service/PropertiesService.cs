using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using AppCommon.DTOs.Modules.PropertyConfigs;
using AppCommon.EnumShared;
using AppCommon.GlobalHelpers;
using Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Service
{
    public class PropertiesService
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly IEntityCacheService _cacheService;

        public PropertiesService(ModuleDbContext moduleDbContext)
        {
            _moduleDbContext = moduleDbContext;
        }

        public async Task<List<Domain.Schema.Properties.Property>> GetProperties(
        Guid? applicationId,
        Guid? moduleId,
        Guid? workspaceId,
        Guid? workspaceModuleId,
        bool useCache,                    
        TimeSpan? cacheExpiry = null,             
        CancellationToken cancellationToken = default)
        {
            if (!useCache)
                return await GetPropertiesInternal(applicationId, moduleId, workspaceId, workspaceModuleId, cancellationToken);


            var cacheKey = BuildCacheKey(applicationId, moduleId, workspaceId, workspaceModuleId);

            var list = await _cacheService.GetOrSetListAsync(
                cacheKey,
                async () => await GetPropertiesInternal(applicationId, moduleId, workspaceId, workspaceModuleId, cancellationToken),
                cacheExpiry
            );

            return list;
        }


        private async Task<List<Domain.Schema.Properties.Property>> GetPropertiesInternal(
         Guid? applicationId,
         Guid? moduleId,
         Guid? workspaceId,
        Guid? workspaceModuleId,
        CancellationToken cancellationToken)
        {
            // 1) Build the base query
            var propertyQuery = BuildPropertyQuery(applicationId, moduleId, workspaceId, workspaceModuleId)
                                    .OrderBy(p => p.Order);

            var dbProperties = await propertyQuery.ToListAsync(cancellationToken);

            // 2) If we have a WorkspaceId, add "connection" properties
            if (workspaceId.HasValue)
            {
                var connectionProps = await GetConnectionProperties(workspaceId.Value, cancellationToken);
                dbProperties.AddRange(connectionProps);
            }

            // 3) Add the "virtual" foreign-key-like properties (if applicable)
            AddVirtualProperties(dbProperties, applicationId, moduleId, workspaceId, workspaceModuleId);

            return dbProperties;
        }

        private IQueryable<Domain.Schema.Properties.Property> BuildPropertyQuery(
            Guid? applicationId,
            Guid? moduleId,
            Guid? workspaceId,
            Guid? workspaceModuleId)
        {
            var query = _moduleDbContext.Properties.AsQueryable()
                .Where(p => !p.IsDeleted);

            if (applicationId.HasValue)
                query = query.Where(p => p.ApplicationId == applicationId);

            if (moduleId.HasValue)
                query = query.Where(p => p.ModuleId == moduleId);

            if (workspaceId.HasValue)
                query = query.Where(p => p.WorkspaceId == workspaceId);

            if (workspaceModuleId.HasValue)
                query = query.Where(p => p.WorkspaceModuleId == workspaceModuleId);

            return query;
        }

        private async Task<List<Domain.Schema.Properties.Property>> GetConnectionProperties(
            Guid workspaceId,
            CancellationToken cancellationToken)
        {
            var result = new List<Domain.Schema.Properties.Property>();

            var connections = await _moduleDbContext.WorkspaceConnections
                .Include(c => c.TargetWorkspace)
                .Include(c => c.SourceWorkspace)
                .Where(c => c.SourceWorkspaceId == workspaceId)
                .ToListAsync(cancellationToken);

            foreach (var conn in connections)
            {
                var connectionDto = new Domain.Schema.Properties.Property
                {
                    Id = conn.Id,
                    Key = conn.TargetWorkspace?.Key ?? "Connection",
                    NormalizedKey = (conn.TargetWorkspace?.Key ?? "Connection").ToUpperInvariant(),
                    Title = conn.TargetWorkspace?.Title,
                    DataType = DataTypeEnum.String,
                    ViewType = ViewTypeEnum.Connection
                };

                var targetData = await _moduleDbContext.WorkspaceData
                     .Where(d => d.WorkspaceId == conn.TargetWorkspaceId && !d.IsDeleted)
                     .Include(d => d.PropertyData.Where(pd => pd.Property.NormalizedKey.ToLower() == "title"))
                     .ToListAsync(cancellationToken);

                var options = new List<ConnectionOptionDto>();
                foreach (var row in targetData)
                {
                    var titlePd = row.PropertyData.FirstOrDefault();
                    var titleValue = titlePd?.GetValue()?.ToString() ?? $"Item {row.Id}";
                    options.Add(new ConnectionOptionDto
                    {
                        Id = row.Id,
                        Title = titleValue
                    });
                }

                var connectionConfig = new ConnectionConfig
                {
                    AllowMany = conn.AllowManySource,
                    IsOptional = conn.IsOptional,
                    Options = options
                };
                connectionDto.Configuration = connectionConfig.AsText();

                result.Add(connectionDto);
            }

            return result;
        }

        private void AddVirtualProperties(
            List<Domain.Schema.Properties.Property> propertiesList,
            Guid? applicationId,
            Guid? moduleId,
            Guid? workspaceId,
            Guid? workspaceModuleId)
        {
            if (applicationId.HasValue)
            {
                propertiesList.Add(new Domain.Schema.Properties.Property
                {
                    Id = Guid.NewGuid(),
                    Key = "ApplicationId",
                    NormalizedKey = "APPLICATIONID",
                    Title = new TranslatableValue
                    {
                        En = "Application Id",
                        Ar = "معرّف التطبيق"
                    },
                    DataType = DataTypeEnum.Guid,
                    ViewType = ViewTypeEnum.ForeignKey
                });
            }

            if (moduleId.HasValue)
            {
                propertiesList.Add(new Domain.Schema.Properties.Property
                {
                    Id = Guid.NewGuid(),
                    Key = "ModuleId",
                    NormalizedKey = "MODULEID",
                    Title = new TranslatableValue
                    {
                        En = "Module Id",
                        Ar = "معرّف الوحدة"
                    },
                    DataType = DataTypeEnum.Guid,
                    ViewType = ViewTypeEnum.ForeignKey
                });
            }
            if (workspaceId.HasValue)
            {
                propertiesList.Add(new Domain.Schema.Properties.Property
                {
                    Id = Guid.NewGuid(),
                    Key = "WorkspaceId",
                    NormalizedKey = "WORKSPACEID",
                    Title = new TranslatableValue
                    {
                        En = "Workspace Id",
                        Ar = "معرّف مساحة العمل"
                    },
                    DataType = DataTypeEnum.Guid,
                    ViewType = ViewTypeEnum.ForeignKey
                });
            }
            if (workspaceModuleId.HasValue)
            {
                propertiesList.Add(new Domain.Schema.Properties.Property
                {
                    Id = Guid.NewGuid(),
                    Key = "WorkspaceModuleId",
                    NormalizedKey = "WORKSPACEMODULEID",
                    Title = new TranslatableValue
                    {
                        En = "Workspace Module Id",
                        Ar = "معرّف وحدة مساحة العمل"
                    },
                    DataType = DataTypeEnum.Guid,
                    ViewType = ViewTypeEnum.ForeignKey
                });
            }
        }

        private string BuildCacheKey(Guid? applicationId, Guid? moduleId, Guid? workspaceId, Guid? workspaceModuleId)
        {
            var key = "properties:";

            if (applicationId.HasValue)
                key += $"app_{applicationId.Value}:";

            if (moduleId.HasValue)
                key += $"mod_{moduleId.Value}:";

            if (workspaceId.HasValue)
                key += $"ws_{workspaceId.Value}:";

            if (workspaceModuleId.HasValue)
                key += $"wsm_{workspaceModuleId.Value}:";

            return key.TrimEnd(':');
        }

        private void HandleFormulaProperties(
            Guid dataId,
            bool IsModule
        )
        {

            //missing and errors
            // - missing property data for the related worspaces (parent and children) we need to fetch them in both module and workspace data
            // - errors :- in workspace fetching prop data for related module => this will duplicate the prop key as the workspace data have many of module data


            //Fetch PropData That Related With Formual Property Type 
            //but we need to make sure here that in every new property add from control panle must create the property data for all workspaces or data

            var FormulaPropertyIds = new List<Guid>();

            //Prepare Values and Keys 

            Dictionary<string, string> propertiesValuesKeyValue = new Dictionary<string, string>();

            var PropertyData = new List<PropertyData>();
            var cachedProperties = _moduleDbContext.Properties.ToList(); //Replace This and read data from cache 
            var FormulaPropertiesData = new List<PropertyData>();

            if (IsModule)
            {
                var ModuleData = _moduleDbContext.ModuleData
                    .Include(x => x.PropertyData)
                    .Include(x => x.WorkspaceData)
                        .ThenInclude(x => x.PropertyData)
                    .FirstOrDefault(x => x.Id == dataId);
                if (ModuleData == null) return;
                
                PropertyData.AddRange(ModuleData.PropertyData);
                if (ModuleData.WorkspaceData != null)
                    PropertyData.AddRange(ModuleData.WorkspaceData.PropertyData);

                FormulaPropertyIds = cachedProperties.Where(x => x.ModuleId == ModuleData.ModuleId && x.PropertyFormulas.Count > 0).Select(x => x.Id).ToList();
            }
            else //Workspace
            {
                var WorkspaceData = _moduleDbContext.WorkspaceData
                    .Include(x => x.PropertyData)
                    .FirstOrDefault(x => x.Id == dataId);

                if (WorkspaceData  == null) return;

                PropertyData.AddRange(WorkspaceData.PropertyData);

                var AllRelatedModulePropData = _moduleDbContext.PropertyData
                    .Where(x => x.ModuleDataId != null && x.ModuleData.WorkSpaceDataId == dataId).ToList();

                PropertyData.AddRange(AllRelatedModulePropData);

                FormulaPropertyIds = cachedProperties.Where(x => x.WorkspaceId == WorkspaceData.WorkspaceId && x.PropertyFormulas.Count > 0).Select(x => x.Id).ToList();
            }

            foreach (var item in PropertyData)
            {
                var prop = cachedProperties.First(x => x.Id == item.PropertyId);
                item.Property = prop;
                propertiesValuesKeyValue.Add(prop.Key, item.GetValue().ToString());
            }



            //Calculated Property Data
            //
            var calculatedPropertyData = PropertyData.Where(x => FormulaPropertyIds.Contains(x.PropertyId)).ToList();

            calculatedPropertyData.ForEach(propData =>
            {
                var Formulas = propData.Property.PropertyFormulas.OrderBy(x => x.Order);
                foreach (var formula in Formulas)
                {
                    var value = propData?.GetValue() ?? propData.Property.DefaultValue;
                    var result = Utils.ExecuteFormulaWithResult(formula.Formula, propertiesValuesKeyValue, value).Result.AsText();

                    if (propertiesValuesKeyValue.ContainsKey(propData.Property.Key))
                        propertiesValuesKeyValue[key: propData.Property.Key] = result;
                    else
                        propertiesValuesKeyValue.Add(propData.Property.Key, result);
                    if (!formula.IsConditional)
                    {
                        propData.SetValue(result);
                        break;
                    }
                    if(result == "true")
                    {
                        propData.SetValue(formula.Value);
                        break;
                    }
                }

            });

            _moduleDbContext.SaveChanges();
        }

    }
}
