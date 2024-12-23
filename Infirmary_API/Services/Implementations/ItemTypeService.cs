using CsvHelper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class ItemTypeService : IItemTypeService
    {
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemTypeService(IItemTypeRepository itemTypeRepository)
        {
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateItemType(AddUpdateItemTypeRequest request)
        {
            return await _itemTypeRepository.AddUpdateItemType(request);
        }

        public async Task<ServiceResponse<List<ItemTypeResponse>>> GetAllItemTypes(GetAllItemTypesRequest request)
        {
            return await _itemTypeRepository.GetAllItemTypes(request);
        }

        public async Task<ServiceResponse<ItemType>> GetItemTypeById(int id)
        {
            return await _itemTypeRepository.GetItemTypeById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteItemType(int id)
        {
            return await _itemTypeRepository.DeleteItemType(id);
        }

        public async Task<ServiceResponse<byte[]>> ExportItemTypesData(GetItemTypesExportRequest request)
        {
            var data = await _itemTypeRepository.GetItemTypesData(request.InstituteID);

            if (data == null || data.Count == 0)
            {
                return new ServiceResponse<byte[]>(false, "No data found", null, 404);
            }

            byte[] exportData = null;

            if (request.ExportType == 1) // Excel export
            {
                exportData = GenerateExcel(data);
            }
            else if (request.ExportType == 2) // CSV export
            {
                exportData = GenerateCsv(data);
            }
            else
            {
                return new ServiceResponse<byte[]>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<byte[]>(true, "Export completed successfully", exportData, 200);
        }

        private byte[] GenerateExcel(List<GetItemTypesExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Item Types");
                worksheet.Cells["A1"].Value = "Item Type Name";
                worksheet.Cells["B1"].Value = "Description";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.ItemTypeName;
                    worksheet.Cells[row, 2].Value = item.Description;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(List<GetItemTypesExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetItemTypesExportResponse>();
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
