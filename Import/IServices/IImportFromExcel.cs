using Import.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.IServices
{
    public interface IImportFromExcel
    {
        Task<List<string>> GetExcelHeaderValues(string headerName, string SessionKey);
        Task<List<RowValue>> MapExcelDataToPropertyValues(List<ConversionMap> conversionMaps, string SessionKey);
        Task<ExcelHeadersWithSessionKey> ReadExcelHeader(IFormFile file);
    }
}
