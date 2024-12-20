using CsvHelper;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Formats.Asn1;
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
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _visitorLogRepository;

        public VisitorLogService(IVisitorLogRepository visitorLogRepository)
        {
            _visitorLogRepository = visitorLogRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorRequestDTO visitorLog)
        {
            return await _visitorLogRepository.AddUpdateVisitorLog(visitorLog);
        }

        public async Task<ServiceResponse<IEnumerable<Visitorlogresponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            return await _visitorLogRepository.GetAllVisitorLogs(request);
        }

        public async Task<ServiceResponse<Visitorlogresponse>> GetVisitorLogById(int visitorId)
        {
            return await _visitorLogRepository.GetVisitorLogById(visitorId);
        }

        public async Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId)
        {
            return await _visitorLogRepository.UpdateVisitorLogStatus(visitorId);
        }

        public async Task<ServiceResponse<IEnumerable<GetSourcesResponse>>> GetSources(GetSourcesRequest request)
        {
            return await _visitorLogRepository.GetSources(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetPurposeResponse>>> GetPurpose(GetPurposeRequest request)
        {
            return await _visitorLogRepository.GetPurpose(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetIDProofResponse>>> GetIDProof()
        {
            return await _visitorLogRepository.GetIDProof();
        }
        public async Task<ServiceResponse<IEnumerable<GetApprovalTypeResponse>>> GetApprovalType()
        {
            return await _visitorLogRepository.GetApprovalType();
        }

        public async Task<ServiceResponse<IEnumerable<GetEmployeeResponse>>> GetEmployee(GetEmployeeRequest request)
        {
            return await _visitorLogRepository.GetEmployee(request);
        }
        public async Task<ServiceResponse<GetVisitorSlipResponse>> GetVisitorSlip(GetVisitorSlipRequest request)
        {
            return await _visitorLogRepository.GetVisitorSlip(request);
        }

        public async Task<ServiceResponse<string>> ChangeApprovalStatus(ChangeApprovalStatusRequest request)
        {
            try
            {
                // Call repository to change approval status
                var result = await _visitorLogRepository.UpdateApprovalStatus(request.VisitorID, request.ApprovalTypeID, request.InstituteID);

                if (result)
                {
                    return new ServiceResponse<string>(true, "Approval status updated successfully.", null, 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to update approval status.", null, 400);
                }
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        //public async Task<ServiceResponse<byte[]>> GetVisitorLogsExport(GetVisitorLogsExportRequest request)
        //{
        //    // Get visitor logs based on filters
        //    var visitorLogs = await _visitorLogRepository.GetVisitorLogs(request);

        //    // Check the export type
        //    if (request.ExportType == 1) // Excel
        //    {
        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Visitor Logs");
        //            worksheet.Cells[1, 1].Value = "VisitorCodeID";
        //            worksheet.Cells[1, 2].Value = "VisitorName";
        //            worksheet.Cells[1, 3].Value = "SourceName";
        //            worksheet.Cells[1, 4].Value = "PurposeName";
        //            worksheet.Cells[1, 5].Value = "MobileNo";
        //            worksheet.Cells[1, 6].Value = "EmailID";
        //            worksheet.Cells[1, 7].Value = "Address";
        //            worksheet.Cells[1, 8].Value = "OrganizationName";
        //            worksheet.Cells[1, 9].Value = "EmployeeFullName";
        //            worksheet.Cells[1, 10].Value = "NoOfVisitor";
        //            worksheet.Cells[1, 11].Value = "AccompaniedBy";
        //            worksheet.Cells[1, 12].Value = "CheckInTime";
        //            worksheet.Cells[1, 13].Value = "CheckOutTime";
        //            worksheet.Cells[1, 14].Value = "Remarks";
        //            worksheet.Cells[1, 15].Value = "ApprovalTypeName";

        //            int row = 2; // Start from the second row

        //            foreach (var log in visitorLogs)
        //            {
        //                worksheet.Cells[row, 1].Value = log.VisitorCodeID;
        //                worksheet.Cells[row, 2].Value = log.VisitorName;
        //                worksheet.Cells[row, 3].Value = log.Sourcename;
        //                worksheet.Cells[row, 4].Value = log.Purposename;
        //                worksheet.Cells[row, 5].Value = log.MobileNo;
        //                worksheet.Cells[row, 6].Value = log.EmailID;
        //                worksheet.Cells[row, 7].Value = log.Address;
        //                worksheet.Cells[row, 8].Value = log.OrganizationName;
        //                worksheet.Cells[row, 9].Value = log.EmployeeFullName;
        //                worksheet.Cells[row, 10].Value = log.NoOfVisitor;
        //                worksheet.Cells[row, 11].Value = log.AccompaniedBy;
        //                worksheet.Cells[row, 12].Value = log.CheckInTime;
        //                worksheet.Cells[row, 13].Value = log.CheckOutTime;
        //                worksheet.Cells[row, 14].Value = log.Remarks;
        //                worksheet.Cells[row, 15].Value = log.ApprovalTypeName;

        //                row++;
        //            }

        //            // Return the Excel file as a byte array
        //            return new ServiceResponse<byte[]>(true, "Exported to Excel successfully", package.GetAsByteArray(), 200);
        //        }
        //    }
        //    else if (request.ExportType == 2) // CSV
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            using (var writer = new StreamWriter(memoryStream))
        //            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //            {
        //                // Write header
        //                csv.WriteHeader<GetVisitorLogsExportResponse>();
        //                csv.NextRecord();

        //                // Write records
        //                await csv.WriteRecordsAsync(visitorLogs);

        //                writer.Flush();
        //                return new ServiceResponse<byte[]>(true, "Exported to CSV successfully", memoryStream.ToArray(), 200);
        //            }
        //        }
        //    }

        //    return new ServiceResponse<byte[]>(false, "Invalid Export Type", null, 400);
        //}




        public async Task<ServiceResponse<byte[]>> GetVisitorLogsExport(GetVisitorLogsExportRequest request)
        {
            try
            {
                var visitorLogs = await _visitorLogRepository.GetVisitorLogs(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(visitorLogs); // Generate Excel byte array
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(visitorLogs); // Generate CSV byte array
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


        // Helper method to generate CSV
        private byte[] GenerateCsv(IEnumerable<GetVisitorLogsExportResponse> visitorLogs)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteHeader<GetVisitorLogsExportResponse>();
                csv.NextRecord();

                // Write data
                foreach (var log in visitorLogs)
                {
                    csv.WriteRecord(log);
                    csv.NextRecord();
                }

                // Flush the data to memory stream
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        // Helper method to generate Excel
        private byte[] GenerateExcel(IEnumerable<GetVisitorLogsExportResponse> visitorLogs)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Visitor Logs");

                // Write header
                worksheet.Cells[1, 1].Value = "VisitorCodeID";
                worksheet.Cells[1, 2].Value = "VisitorName";
                worksheet.Cells[1, 3].Value = "Sourcename";
                worksheet.Cells[1, 4].Value = "Purposename";
                worksheet.Cells[1, 5].Value = "MobileNo";
                worksheet.Cells[1, 6].Value = "EmailID";
                worksheet.Cells[1, 7].Value = "Address";
                worksheet.Cells[1, 8].Value = "OrganizationName";
                worksheet.Cells[1, 9].Value = "EmployeeFullName";
                worksheet.Cells[1, 10].Value = "NoOfVisitor";
                worksheet.Cells[1, 11].Value = "AccompaniedBy";
                worksheet.Cells[1, 12].Value = "CheckInTime";
                worksheet.Cells[1, 13].Value = "CheckOutTime";
                worksheet.Cells[1, 14].Value = "Remarks";
                worksheet.Cells[1, 15].Value = "ApprovalTypeName";

                int row = 2;
                foreach (var log in visitorLogs)
                {
                    worksheet.Cells[row, 1].Value = log.VisitorCodeID;
                    worksheet.Cells[row, 2].Value = log.VisitorName;
                    worksheet.Cells[row, 3].Value = log.Sourcename;
                    worksheet.Cells[row, 4].Value = log.Purposename;
                    worksheet.Cells[row, 5].Value = log.MobileNo;
                    worksheet.Cells[row, 6].Value = log.EmailID;
                    worksheet.Cells[row, 7].Value = log.Address;
                    worksheet.Cells[row, 8].Value = log.OrganizationName;
                    worksheet.Cells[row, 9].Value = log.EmployeeFullName;
                    worksheet.Cells[row, 10].Value = log.NoOfVisitor;
                    worksheet.Cells[row, 11].Value = log.AccompaniedBy;
                    worksheet.Cells[row, 12].Value = log.CheckInTime;
                    worksheet.Cells[row, 13].Value = log.CheckOutTime;
                    worksheet.Cells[row, 14].Value = log.Remarks;
                    worksheet.Cells[row, 15].Value = log.ApprovalTypeName;

                    row++;
                }

                // Save to memory stream
                package.Save();
                return memoryStream.ToArray();
            }
        }
    }
}
