using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;
using System.IO;
using OfficeOpenXml;
using System.Text;


namespace Transport_API.Services.Implementations
{
    public class RoutePlanService : IRoutePlanService
    {
        private readonly IRoutePlanRepository _routePlanRepository;

        public RoutePlanService(IRoutePlanRepository routePlanRepository)
        {
            _routePlanRepository = routePlanRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlanRequestDTO routePlan)
        {
            return await _routePlanRepository.AddUpdateRoutePlan(routePlan);
        }

        public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            return await _routePlanRepository.GetAllRoutePlans(request);
        }

        public async Task<ServiceResponse<GetAllRoutePlanExportResponse>> ExportRoutePlansData(GetAllRoutePlanExportRequest request)
        {
            var routePlansResponse = await _routePlanRepository.FetchRoutePlansForExport(request);

            if (!routePlansResponse.Success)
            {
                return new ServiceResponse<GetAllRoutePlanExportResponse>(false, "No data found", null, 204);
            }

            var routePlans = routePlansResponse.Data.ToList();
            byte[] fileData = null;
            string fileName = $"RoutePlanExport_{DateTime.Now:yyyyMMddHHmmss}";

            if (request.ExportType == 1) // Export to Excel
            {
                fileData = GenerateExcel(routePlans);
                fileName += ".xlsx";
            }
            else if (request.ExportType == 2) // Export to CSV
            {
                fileData = GenerateCsv(routePlans);
                fileName += ".csv";
            }

            return new ServiceResponse<GetAllRoutePlanExportResponse>(true, "File Generated", new GetAllRoutePlanExportResponse
            {
                FileData = fileData,
                FileName = fileName
            }, 200);
        }

        private byte[] GenerateExcel(IEnumerable<RoutePlanResponseDTOExport> routePlans)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("RoutePlans");
                worksheet.Cells[1, 1].Value = "RouteName";
                worksheet.Cells[1, 2].Value = "VehicleNumber";
                worksheet.Cells[1, 3].Value = "NoOfStops";
                worksheet.Cells[1, 4].Value = "PickUpTime";
                worksheet.Cells[1, 5].Value = "DropTime";
                worksheet.Cells[1, 6].Value = "DriverName";

                int row = 2;
                foreach (var plan in routePlans)
                {
                    worksheet.Cells[row, 1].Value = plan.RouteName;
                    worksheet.Cells[row, 2].Value = plan.VehicleNumber;
                    worksheet.Cells[row, 3].Value = plan.NoOfStops;
                    worksheet.Cells[row, 4].Value = plan.PickUpTime;
                    worksheet.Cells[row, 5].Value = plan.DropTime;
                    worksheet.Cells[row, 6].Value = plan.DriverName;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateCsv(IEnumerable<RoutePlanResponseDTOExport> routePlans)
        {
            var csv = new StringBuilder();
            csv.AppendLine("RouteName,VehicleNumber,NoOfStops,PickUpTime,DropTime,DriverName");

            foreach (var plan in routePlans)
            {
                csv.AppendLine($"{plan.RouteName},{plan.VehicleNumber},{plan.NoOfStops},{plan.PickUpTime},{plan.DropTime},{plan.DriverName}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }


        public async Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int routePlanId)
        {
            return await _routePlanRepository.GetRoutePlanById(routePlanId);
        }

        public async Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int routePlanId)
        {
            return await _routePlanRepository.UpdateRoutePlanStatus(routePlanId);
        }
        public async Task<ServiceResponse<RouteDetailsResponseDTO>> GetRouteDetails(GetRouteDetailsRequest request)
        {
            return await _routePlanRepository.GetRouteDetails(request);
        }
        public async Task<ServiceResponse<byte[]>> GetRouteDetailsExportExcel(GetRouteDetailsRequest request)
        {
            return await _routePlanRepository.GetRouteDetailsExportExcel(request);
        }
         
        public async Task<ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>> GetRoutePlanVehicles(int instituteID)
        {
            return await _routePlanRepository.GetRoutePlanVehicles(instituteID);
        }


    }
}
