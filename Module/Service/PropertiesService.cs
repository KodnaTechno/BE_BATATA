using AppCommon.GlobalHelpers;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Service
{
    
    public class PropertiesService
    {
        private readonly ModuleDbContext _moduleDbContext;

        public PropertiesService(ModuleDbContext moduleDbContext)
        {
            _moduleDbContext = moduleDbContext;
        }

        public PropertiesService()
        {
            
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
                    .Include(x => x.ProperatyData)
                    .Include(x => x.WorkspaceData)
                        .ThenInclude(x => x.ProperatyData)
                    .FirstOrDefault(x => x.Id == dataId);
                if (ModuleData == null) return;
                
                PropertyData.AddRange(ModuleData.ProperatyData);
                if (ModuleData.WorkspaceData != null)
                    PropertyData.AddRange(ModuleData.WorkspaceData.ProperatyData);

                FormulaPropertyIds = cachedProperties.Where(x => x.ModuleId == ModuleData.ModulId && x.PropertyFormulas.Count > 0).Select(x => x.Id).ToList();
            }
            else //Workspace
            {
                var WorkspaceData = _moduleDbContext.WorkspaceData
                    .Include(x => x.ProperatyData)
                    .FirstOrDefault(x => x.Id == dataId);

                if (WorkspaceData  == null) return;

                PropertyData.AddRange(WorkspaceData.ProperatyData);

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
