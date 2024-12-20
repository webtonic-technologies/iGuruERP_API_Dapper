using CsvHelper;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class EmployeeGatePassService : IEmployeeGatePassService
    {
        private readonly IEmployeeGatePassRepository _employeeGatePassRepository;

        public EmployeeGatePassService(IEmployeeGatePassRepository employeeGatePassRepository)
        {
            _employeeGatePassRepository = employeeGatePassRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass)
        {
            return await _employeeGatePassRepository.AddUpdateEmployeeGatePass(employeeGatePass);
        }

        public async Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            return await _employeeGatePassRepository.GetAllEmployeeGatePass(request);
        }

        public async Task<ServiceResponse<List<Visitedfor>>> GetAllVisitedForReason()
        {
            return await _employeeGatePassRepository.GetAllVisitedForReason();
        }

        public async Task<ServiceResponse<EmployeeGatepassResponse>> GetEmployeeGatePassById(int gatePassId)
        {
            return await _employeeGatePassRepository.GetEmployeeGatePassById(gatePassId);
        }

        public async Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            return await _employeeGatePassRepository.UpdateEmployeeGatePassStatus(gatePassId);
        }
        public async Task<List<GetVisitorForDDLResponse>> GetVisitorForDDL()
        {
            return await _employeeGatePassRepository.GetVisitorForDDL();
        }

        public async Task<GetGatePassSlipResponse> GetGatePassSlip(int gatePassID, int instituteID)
        {
            return await _employeeGatePassRepository.GetGatePassSlip(gatePassID, instituteID);
        }

        //public ServiceResponse<string> GetEmployeeGatePassExport(GetEmployeeGatePassExportRequest request)
        //{
        //    // Fetch data from the database
        //    var data = _employeeGatePassRepository.GetEmployeeGatePassExport(request.InstituteId, request.EmployeeId, request.StartDate, request.EndDate, request.SearchText);

        //    // Process export based on type
        //    if (request.ExportType == 1)  // Excel
        //    {
        //        return ExportToExcel(data);
        //    }
        //    else if (request.ExportType == 2)  // CSV
        //    {
        //        return ExportToCSV(data);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<string>(false, "Invalid Export Type", null, 400);
        //    }
        //}

        public async Task<ServiceResponse<byte[]>> GetEmployeeGatePassExport(GetEmployeeGatePassExportRequest request)
        {
            try
            {
                var GatePass = await _employeeGatePassRepository.GetEmployeeGatePassExport(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(GatePass); // Generate Excel byte array
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(GatePass); // Generate CSV byte array
                }

                return new ServiceResponse<byte[]>(
                    true,
                    "Export completed successfully",
                    exportData,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }



        private byte[] GenerateExcel(IEnumerable<GetEmployeeGatePassExportResponse> GatePass)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("EmployeeGatePass");
                worksheet.Cells["A1"].Value = "Employee Name";
                worksheet.Cells["B1"].Value = "Department Name";
                worksheet.Cells["C1"].Value = "Designation Name";
                worksheet.Cells["D1"].Value = "Pass No";
                worksheet.Cells["E1"].Value = "Visitor For Name";
                worksheet.Cells["F1"].Value = "Check In Time";
                worksheet.Cells["G1"].Value = "Check Out Time";
                worksheet.Cells["H1"].Value = "Purpose";
                worksheet.Cells["I1"].Value = "Plan Of Visit";
                worksheet.Cells["J1"].Value = "Remarks";
                worksheet.Cells["K1"].Value = "Status Name";

                int row = 2;
                foreach (var log in GatePass)
                { 
                    worksheet.Cells[row, 1].Value = log.EmployeeName;
                    worksheet.Cells[row, 2].Value = log.DepartmentName;
                    worksheet.Cells[row, 3].Value = log.DesignationName;
                    worksheet.Cells[row, 4].Value = log.PassNo;
                    worksheet.Cells[row, 5].Value = log.VisitorForName;
                    worksheet.Cells[row, 6].Value = log.CheckInTime;
                    worksheet.Cells[row, 7].Value = log.CheckOutTime;
                    worksheet.Cells[row, 8].Value = log.Purpose;
                    worksheet.Cells[row, 9].Value = log.PlanOfVisit;
                    worksheet.Cells[row, 10].Value = log.Remarks;
                    worksheet.Cells[row, 11].Value = log.StatusName;
                    row++;
                }

                // Save to memory stream
                package.Save();
                return memoryStream.ToArray();
            }
        }


        private byte[] GenerateCsv(IEnumerable<GetEmployeeGatePassExportResponse> GatePass)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteHeader<GetVisitorLogsExportResponse>();
                csv.NextRecord();

                // Write data
                foreach (var log in GatePass)
                {
                    csv.WriteRecord(log);
                    csv.NextRecord();
                }

                // Flush the data to memory stream
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
         
    }
}
