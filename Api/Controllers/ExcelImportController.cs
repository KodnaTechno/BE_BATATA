using AppCommon.GlobalHelpers;
using Import.IServices;
using Import.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Module;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

namespace Api.Controllers
{
    /**
        TODO
        1- Remove Logic Code From Here
        2- Add the Authentication and Authorizations Middlewares
     */
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelImportController : Controller
    {
        private readonly IImportFromExcel _importFromExcel;
        private readonly ModuleDbContext _dbContext;
        public ExcelImportController(IImportFromExcel importFromExcel, ModuleDbContext dbContext)
        {
            _importFromExcel = importFromExcel;
            _dbContext = dbContext;
        }
        [HttpPost]
        [Route("read-excel-headers")]
        public async Task<ActionResult<object>> ReadExcelHeaders(ReadeExcelHeaderCommand request)
        {
            return await _importFromExcel.ReadExcelHeader(request.File);
        }

        [HttpPost]
        [Route("get-selected-module-props")]
        public async Task<ActionResult<object>> ReadModuleProps(GetSelectedSchemaPropsCommand request)
        {
            var propsQ = _dbContext.Properties.Where(x => x.WorkspaceId == request.WorkspaceId);
            if(request.ModuleId != null && request.ModuleId != Guid.Empty){
                propsQ = propsQ.Where(x => x.ModuleId == request.ModuleId);
            }
            var props = propsQ.Select(x => new
            {
                Key = x.Key,
                Title = x.Title,
                ViewType = x.ViewType,
                DataType = x.DataType,
                IsRequired = x.ValidationRules.Any(x => x.RuleType == RuleTypeEnum.Required), //This must be exists on the mapping column (we need to set matching header and the default value if the value is not exists)
            }).ToList();
            return props;
        }

        [HttpPost]
        [Route("property-checker")]
        public async Task<ActionResult<object>> PropertyChecker(PropertyCheckerCommand request)
        {

            //Return if there is a prop as a lookup prop if yes return this prop with it's lookup itesm to match them with the Excel header value 
            // Return Them with header value which will be matched with lookup item

            var propsQ = _dbContext.Properties.Where(x => x.WorkspaceId == request.WorkspaceId);
            if (request.ModuleId != null && request.ModuleId != Guid.Empty)
            {
                propsQ = propsQ.Where(x => x.ModuleId == request.ModuleId);
            }
            var props = propsQ.Include(x => x.ValidationRules).ToList();



            //Check if All Required Props is Exists in Mapping Or Have A Deafult Value
            var requiredProps = props.Where(x => x.ValidationRules.Any(x => x.RuleType == RuleTypeEnum.Required)).ToList();
            var RequiredPropsNotExistsInMapping = requiredProps.Where(p => !request.Items.Select(i => i.PropertyKey).Contains(p.Key)).ToList();
            //Check if There Required Prop Not Have A Default Value 
            var RequiredPropsExistsInMappingWithOutDefaultValue = request.Items.Where(p => requiredProps.Any(x => x.Key == p.PropertyKey && string.IsNullOrEmpty(p.DeafultValue))).ToList();

            if(RequiredPropsNotExistsInMapping.Count > 0 || RequiredPropsExistsInMappingWithOutDefaultValue.Count > 0)
            {
                return new PropertyCheckerRes
                {
                    IsValid = false,
                    RequiredPropertyNotSetup = RequiredPropsNotExistsInMapping,
                    RequiredPropertyNeedDefaultValue = props.Where(x => RequiredPropsExistsInMappingWithOutDefaultValue.Select(e => e.PropertyKey).Contains(x.Key)).ToList(), // we have to set default value in case the value is not exists at some column on Excel
                };
            }

            var propKeys = request.Items.Select(x => x.PropertyKey).Distinct().ToList();

            var propertyNeedMappingTypes = new List<ViewTypeEnum>() { ViewTypeEnum.Lookup };

            //Properties Have Predefined Values We Need To Select The Value from them
            var propertyNeedMapping = props.Where(x => propKeys.Contains(x.Key) && propertyNeedMappingTypes.Contains(x.ViewType)).ToList();

            var ExcelHeaderMappedWithPredefiendValues = new List<ExcelHeaderMappedWithPredefiendValues>();

            foreach (var prop in propertyNeedMapping) { 
                var MappedProps = new List<ExcelToSystemLookupItems>();
                var item = request.Items.Where(x => x.PropertyKey == prop.Key).FirstOrDefault();
                
                var excelToSystemLookupItems = new ExcelToSystemLookupItems();
                
                excelToSystemLookupItems.ExcelHeader = await _importFromExcel.GetExcelHeaderValues(item.ExcelHeaderName, request.FileSessionKey);
                
                if (prop.ViewType == ViewTypeEnum.Lookup)
                {
                    //excelToSystemLookupItems.SystemItems = _dbContext.LookupItems.Where(x => x.Lookup.PropertyId == prop.Id).Select(x => new ExcelToSystemLookupItemSystemItem { Name = x.Name, RawValue = x.Key}).ToList();
                }
                ExcelHeaderMappedWithPredefiendValues.Add(new ExcelHeaderMappedWithPredefiendValues { Property = prop, MatchedValues = MappedProps });
            }

            //Mapping Props

            return new PropertyCheckerRes
            {
                IsValid = true,
                RequiredPropertyNotSetup = new (),
                RequiredPropertyNeedDefaultValue = new (), // we have to set default value in case the value is not exists at some column on Excel
                ExcelHeaderMappedWithPredefiendValues = ExcelHeaderMappedWithPredefiendValues
            };

        }

        [HttpPost]
        [Route("start-importing")]
        public async Task<ActionResult<object>> StartImporting(StartImportingExcelCommand request)
        {
            var propsQ = _dbContext.Properties.Where(x => x.WorkspaceId == request.WorkspaceId);
            if (request.ModuleId != null && request.ModuleId != Guid.Empty)
            {
                propsQ = propsQ.Where(x => x.ModuleId == request.ModuleId);
            }
            var props = propsQ.Include(x => x.ValidationRules).ToList();



            //Check if All Required Props is Exists in Mapping Or Have A Deafult Value
            var requiredProps = props.Where(x => x.ValidationRules.Any(x => x.RuleType == RuleTypeEnum.Required)).ToList();
            var RequiredPropsNotExistsInMapping = requiredProps.Where(p => !request.Items.Select(i => i.PropertyKey).Contains(p.Key)).ToList();
            //Check if There Required Prop Not Have A Default Value 
            var RequiredPropsExistsInMappingWithOutDefaultValue = request.Items.Where(p => requiredProps.Any(x => x.Key == p.PropertyKey && string.IsNullOrEmpty(p.DeafultValue))).ToList();

            var PropertyMustHaveSpecificValues = new List<Property>();

            //check the properties with pre-defined values
            var propKeys = request.Items.Select(x => x.PropertyKey).Distinct().ToList();

            var propertyNeedMappingTypes = new List<ViewTypeEnum>() { ViewTypeEnum.Lookup };

            //Properties Have Predefined Values We Need To Select The Value from them
            var propertiesHaveSpecificValuesAndNeedMapping = props.Where(x => propKeys.Contains(x.Key) && propertyNeedMappingTypes.Contains(x.ViewType)).ToList();

            foreach ( var prop in propertiesHaveSpecificValuesAndNeedMapping)
            {
                var item = request.Items.Where(x => x.PropertyKey == prop.Key).FirstOrDefault();
                var ExcelHeaderValues = await _importFromExcel.GetExcelHeaderValues(item.ExcelHeaderName , request.FileSessionKey);

                var ExcelHeaderNotSetup = ExcelHeaderValues.Where(v => !item.ReplacedValues.Select(x => x.ExcelValue).Contains(v)); //Not Matched with any system value
                if(ExcelHeaderNotSetup.Count() == 0)
                {
                    //This property have excel value didn't set with any lookup value (if there is no lookup item user need to set value)
                    propertiesHaveSpecificValuesAndNeedMapping.Add(prop);
                }
               
                if(item.ReplacedValues.Any(x => string.IsNullOrEmpty(x.SystemValue)))
                {
                    propertiesHaveSpecificValuesAndNeedMapping.Add(prop);
                }
            }

            if (RequiredPropsNotExistsInMapping.Count > 0 || RequiredPropsExistsInMappingWithOutDefaultValue.Count > 0 || PropertyMustHaveSpecificValues.Count > 0)
            {
                return new StartImportingExcelRes
                {
                    IsValid = false,
                    RequiredPropertyNotSetup = RequiredPropsNotExistsInMapping,
                    RequiredPropertyNeedDefaultValue = props.Where(x => RequiredPropsExistsInMappingWithOutDefaultValue.Select(e => e.PropertyKey).Contains(x.Key)).ToList(), // we have to set default value in case the value is not exists at some column on Excel
                    PropertyMustHaveSpecificValues = PropertyMustHaveSpecificValues
                };
            }

            var mappingRequest = request.Items.Select(x => new ConversionMap
            {
                DefaultValue = x.DeafultValue,
                ExcelHeaderName = x.ExcelHeaderName,
                PropertyKey = x.PropertyKey,
                ReplacedValues = x.ReplacedValues
            }).ToList();
            var prpoertyValues = await _importFromExcel.MapExcelDataToPropertyValues(mappingRequest, request.FileSessionKey);

            //Format Date for date types 

            //GUID, Key , Value 
            //Front-end developer will recive this data and send it to another endpoint (proccess builder endpoint or something like that) (to store the data)
            return await MakeSureExcelValueInCorrectFormats(prpoertyValues, props);
            return Ok();
        }

        [HttpPost]
        [Route("test-import-service")]
        public async Task<ActionResult<object>> TestImoortService(IFormFile file, int HeaderIndex)
        {
            var headers =  await _importFromExcel.ReadExcelHeader(file); //For Proccess Builder xlx [#, Name, Last Updated]

            //if (headers.Count > 0 && headers.Count >= HeaderIndex) {             
            //    var valuesForFirstHeader = await _importFromExcel.GetExcelHeaderValues(headers.ToArray()[HeaderIndex]);
            //    return new {headers, valuesForFirstHeader };
            //}

            var result = await _importFromExcel.MapExcelDataToPropertyValues(new List<ConversionMap>
            {
                new ConversionMap
                {
                    DefaultValue = "0",
                    ExcelHeaderName = "#",
                    PropertyKey= "Id",
                    ReplacedValues = new List<ReplacedValues> { new ReplacedValues {ExcelValue = "1", SystemValue = "1000" } }
                },
                new ConversionMap
                {
                    DefaultValue = "Name Default Value",
                    ExcelHeaderName = "Name",
                    PropertyKey= "Name",
                }
            }, headers.FileSessionKey);

            return result;
        }

        private async Task<List<object>> MakeSureExcelValueInCorrectFormats(List<RowValue> values , List<Property> props)
        {
            var data = new List<object>();
            foreach (RowValue row in values) {             
                foreach (var item in row.Properties) {
                    var prop = props.FirstOrDefault(x => x.Key == item.Key);
                    if (prop == null)
                        continue;

                    var value = item.Value;
                    // replace this with the helper function that we will use to map the prop value depends on (prop data type or view type)
                    if (prop.DataType == DataTypeEnum.DateTime)
                    {
                        //value = DateTime.ParseExact(value).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        //
                    }

                    data.Add(new
                    {
                        Id = prop.Id,
                        Key = prop.Key,
                        Value = value
                    });
                }
            }

            return data;
        }

    }

    public class ReadeExcelHeaderCommand
    {
        public IFormFile File { get; set; }
    }
    
    public class GetSelectedSchemaPropsCommand
    {
        public Guid WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
    }

    public class PropertyCheckerCommand
    {
        public string FileSessionKey { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public List<PorpertyCheckerItem> Items { get; set; }
    }

    public class PorpertyCheckerItem
    {
        public string PropertyKey { get; set; }
        public ViewTypeEnum ViewType { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string DeafultValue { get; set; }
        public string ExcelHeaderName { get; set; }
    }

    public class ExcelHeaderMappedWithPredefiendValues
    {
        public Property Property { get; set; }
        public List<ExcelToSystemLookupItems> MatchedValues { get; set; }
    }
    public class ExcelToSystemLookupItems
    {
        public List<string> ExcelHeader { get; set; }
        public List<ExcelToSystemLookupItemSystemItem> SystemItems { get; set; }
    }
    public class ExcelToSystemLookupItemSystemItem
    {
        public string Name { get; set; }
        public string RawValue { get; set; }
    }

    public class PropertyCheckerRes
    {
        public bool IsValid { get; set; }
        public List<Property> RequiredPropertyNotSetup { get; set; }
        public List<Property> RequiredPropertyNeedDefaultValue { get; set; }
        public List<ExcelHeaderMappedWithPredefiendValues> ExcelHeaderMappedWithPredefiendValues { get; set; }
    }

    public class StartImportingExcelCommand
    {
        public string FileSessionKey { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public List<PropertyExcelImportingItems> Items { get; set; }
    }

    public class PropertyExcelImportingItems
    {
        public string PropertyKey { get; set; }
        public ViewTypeEnum ViewType { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string DeafultValue { get; set; }
        public string ExcelHeaderName { get; set; }

        public List<ReplacedValues> ReplacedValues { get; set; }
    }

    //public class ReplacedValues
    //{
    //    public string ExcelValue { get; set; }
    //    public string SystemValue { get; set; }
    //}

    public class StartImportingExcelRes
    {
        public bool IsValid { get; set; }
        public List<Property> RequiredPropertyNotSetup { get; set; }
        public List<Property> RequiredPropertyNeedDefaultValue { get; set; }
        public List<Property> PropertyMustHaveSpecificValues { get; set; }
    }
}
