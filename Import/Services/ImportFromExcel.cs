using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Import.IServices;
using Import.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Services
{
    public class ImportFromExcel : IImportFromExcel
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ExcelFileSessionKey = "ExcelFile";

        public ImportFromExcel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GenerateSessionKey()
        {
            return $"ExcelFile_{Guid.NewGuid()}";
        }

        private void SaveFileToSession(IFormFile file, string SessionKey)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                _httpContextAccessor.HttpContext.Session.Set(SessionKey, memoryStream.ToArray());
            }
        }

        private byte[] GetFileFromSession(string SessionKey)
        {
            _httpContextAccessor.HttpContext.Session.TryGetValue(SessionKey, out var fileBytes);
            return fileBytes;
        }
        public async Task<ExcelHeadersWithSessionKey> ReadExcelHeader(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new Exception("File Not Setup");
            }
            var SessionKey = GenerateSessionKey();
            SaveFileToSession(file, SessionKey);

            var headers = new List<string>();
            //Read File Headers 

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming you want the first worksheet
                    var firstRow = worksheet.Row(1); // Assuming headers are in the first row

                    foreach (var cell in firstRow.CellsUsed())
                    {
                        headers.Add(cell.Value.ToString());
                    }
                }
            }
            return new ExcelHeadersWithSessionKey { FileSessionKey = SessionKey, Headers = headers};
        }


        public async Task<List<RowValue>> MapExcelDataToPropertyValues(List<ConversionMap> conversionMaps, string SessionKey)
        {

            var fileBytes = GetFileFromSession(SessionKey);

            if (fileBytes == null || fileBytes.Length <= 0)
            {
                throw new Exception("File Not Setup");
            }

            //before got to this point data must be mapped correctly and we need to set the Replacable Value (like lookups)
            //so to do that we need to check the log propers in the mapping if it is have a lookup prop or similar prop so we need to add it in ReplacableValues
            var data = new List<RowValue>();

            using (var stream = new MemoryStream(fileBytes))
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming you want the first worksheet
                    var firstRow = worksheet.Row(1); // Assuming headers are in the first row
                    var columnMapping = new Dictionary<string, int>();

                    // Map header names to column indices
                    foreach (var cell in firstRow.CellsUsed())
                    {
                        var headerName = cell.Value.ToString().Trim();
                        var conversionMap = conversionMaps.FirstOrDefault(cm => cm.ExcelHeaderName.Equals(headerName, StringComparison.OrdinalIgnoreCase));
                        if (conversionMap != null)
                        {
                            columnMapping[conversionMap.PropertyKey] = cell.Address.ColumnNumber;
                        }
                    }

                    // If no mappings found, throw an exception
                    if (!columnMapping.Any())
                    {
                        throw new Exception("No matching headers found in the Excel file.");
                    }
                    var properties = new List<PropertyValues>();
                    // Iterate through the rows
                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip the header row
                    {
                        var rowData = new List<PropertyValues>();

                        foreach (var conversionMap in conversionMaps)
                        {
                            if (columnMapping.TryGetValue(conversionMap.PropertyKey, out int columnIndex))
                            {
                                var cell = row.Cell(columnIndex);
                                var cellValue = cell.Value.ToString().Trim();

                                // Replace values if necessary
                                var replacedValue = conversionMap.ReplacedValues?
                                    .FirstOrDefault(rv => rv.ExcelValue.Equals(cellValue, StringComparison.OrdinalIgnoreCase))?.SystemValue ?? cellValue;

                                rowData.Add(new PropertyValues
                                {
                                    Key = conversionMap.PropertyKey,
                                    Value = string.IsNullOrEmpty(replacedValue) ? conversionMap.DefaultValue : replacedValue
                                });
                            }
                        }

                        properties.AddRange(rowData);
                    }
                    data.Add(new RowValue { Properties = properties });
                }
            }
            return data;
        }

        public async Task<List<string>> GetExcelHeaderValues(string headerName, string SessionKey)
        {
            var fileBytes = GetFileFromSession(SessionKey);

            if (fileBytes == null || fileBytes.Length <= 0)
            {
                throw new Exception("File Not Setup");
            }
            var values = new List<string>();
            //we use this to Adjust ReplacedValues in ConversionMap Class (to make user set what the value should be replaced for what).
            using (var stream = new MemoryStream(fileBytes))
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming you want the first worksheet
                    var firstRow = worksheet.Row(1); // Assuming headers are in the first row
                    int headerColumnIndex = -1;

                    // Find the column index of the header name
                    foreach (var cell in firstRow.CellsUsed())
                    {
                        if (cell.Value.ToString().Equals(headerName, StringComparison.OrdinalIgnoreCase))
                        {
                            headerColumnIndex = cell.Address.ColumnNumber;
                            break;
                        }
                    }

                    if (headerColumnIndex == -1)
                    {
                        throw new Exception($"Header '{headerName}' not found.");
                    }

                    // Get all values in the column with the found index
                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip the header row
                    {
                        var cell = row.Cell(headerColumnIndex);
                        if (!cell.IsEmpty())
                        {
                            values.Add(cell.Value.ToString());
                        }
                    }
                }
            }
                return values.Distinct().ToList();
        }
    }
}
