using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;
using Communication_API.Repository.Interfaces.WhatsApp;
using Communication_API.Services.Interfaces.WhatsApp;
using OfficeOpenXml;
using System.Text;

namespace Communication_API.Services.Implementations.WhatsApp
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IWhatsAppRepository _whatsAppRepository;

        public WhatsAppService(IWhatsAppRepository whatsAppRepository)
        {
            _whatsAppRepository = whatsAppRepository;
        }

        public async Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request)
        {
            return await _whatsAppRepository.Setup(request);
        }

        public async Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID)
        {
            return await _whatsAppRepository.GetBalance(VendorID);
        }

        public async Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateWhatsAppTemplateRequest request)
        {
            return await _whatsAppRepository.AddUpdateTemplate(request);
        }

        public async Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppTemplate(request);
        }

        public async Task<ServiceResponse<List<GetWhatsAppTemplateExportResponse>>> GetWhatsAppTemplateExport(GetWhatsAppTemplateExportRequest request)
        {
            // Fetch data from repository
            var templates = await _whatsAppRepository.GetWhatsAppTemplateExport(request.InstituteID);

            var response = new ServiceResponse<List<GetWhatsAppTemplateExportResponse>>(true, "Export Data Retrieved", templates, 200);
            return response;
        }

        public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        {
            return await _whatsAppRepository.Send(request);
        }

        public async Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppReport(request);
        }

        public async Task<ServiceResponse<string>> SendWhatsAppToStudents(SendWhatsAppStudentRequest request)
        {
            try
            {
                //// Parse WhatsAppDate from string to DateTime
                //DateTime whatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                //// Step 1: Insert the WhatsApp message into tblWhatsAppStudent
                //foreach (var studentID in request.StudentIDs)
                //{
                //    await _whatsAppRepository.InsertWhatsAppForStudent(request.GroupID, request.InstituteID, studentID, request.WhatsAppMessage, whatsAppDate, 0); // Assuming WhatsAppStatusID is 0 (Pending)
                //}

                //return new ServiceResponse<string>(true, "WhatsApp message sent successfully to students.", "WhatsApp message added/updated successfully", 200);

                // Convert SMSDate from string to DateTime
                DateTime whatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var student in request.StudentMessages)
                {
                    await _whatsAppRepository.InsertWhatsAppForStudent(request.GroupID, request.InstituteID, student.StudentID, student.WhatsAppMessage, whatsAppDate, 1, request.SentBy); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "WhatsApp sent successfully to students.", "WhatsApp sent successfully", 200);

            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send WhatsApp message", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> SendWhatsAppToEmployees(SendWhatsAppEmployeeRequest request)
        {
            try
            {
                // Convert SMSDate from string to DateTime
                DateTime WhatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                // Iterate over each employeeMessage and insert SMS data into the table
                foreach (var employee in request.EmployeeMessages)
                {
                    await _whatsAppRepository.InsertWhatsAppForEmployee(request.GroupID, request.InstituteID, employee.EmployeeID, employee.Message, WhatsAppDate, 1, request.SentBy); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "WhatsApp sent successfully to employees.", "WhatsApp sent successfully", 200);

                //// Parse WhatsAppDate from string to DateTime
                //DateTime whatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                //// Step 1: Insert the WhatsApp message into tblWhatsAppEmployee
                //foreach (var employeeID in request.EmployeeIDs)
                //{
                //    await _whatsAppRepository.InsertWhatsAppForEmployee(request.GroupID, request.InstituteID, employeeID, request.WhatsAppMessage, whatsAppDate, 0); // Assuming WhatsAppStatusID is 0 (Pending)
                //}

                //return new ServiceResponse<string>(true, "WhatsApp message sent successfully to employees.", "WhatsApp message added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send WhatsApp message", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateWhatsAppStudentStatus(UpdateWhatsAppStudentStatusRequest request)
        {
            try
            {
                // Step 1: Update the WhatsApp status for the student
                await _whatsAppRepository.UpdateWhatsAppStatusForStudent(request.GroupID, request.InstituteID, request.StudentID, request.WhatsAppStatusID);

                return new ServiceResponse<string>(true, "WhatsApp status updated successfully for student.", "WhatsApp status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update WhatsApp status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateWhatsAppEmployeeStatus(UpdateWhatsAppEmployeeStatusRequest request)
        {
            try
            {
                // Step 1: Update the WhatsApp status for the employee
                await _whatsAppRepository.UpdateWhatsAppStatusForEmployee(request.GroupID, request.InstituteID, request.EmployeeID, request.WhatsAppStatusID);

                return new ServiceResponse<string>(true, "WhatsApp status updated successfully for employee.", "WhatsApp status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update WhatsApp status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<List<WhatsAppTemplateDDLResponse>>> GetWhatsAppTemplateDDL(WhatsAppTemplateDDLRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppTemplateDDL(request.InstituteID);
        }

        public async Task<ServiceResponse<List<WhatsAppStudentReportsResponse>>> GetWhatsAppStudentReport(GetWhatsAppStudentReportRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppStudentReport(request);
        }


        public async Task<ServiceResponse<string>> GetWhatsAppStudentReportExport(WhatsAppStudentReportExportRequest request)
        {
            // Fetch the SMS Student report data
            var reportData = await _whatsAppRepository.GetWhatsAppStudentReportData(request);

            if (reportData == null || !reportData.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            // Check the ExportType and return the corresponding file
            if (request.ExportType == 1)
            {
                // Generate Excel file
                var file = GenerateExcelFile(reportData);
                return new ServiceResponse<string>(true, "Excel file generated", file, 200);
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV file
                var file = GenerateCsvFile(reportData);
                return new ServiceResponse<string>(true, "CSV file generated", file, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }
        }

        private string GenerateExcelFile(List<WhatsAppStudentReportExportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("WhatsApp Report");
                worksheet.Cells[1, 1].Value = "Admission Number";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Class Section";
                worksheet.Cells[1, 4].Value = "Date Time";
                worksheet.Cells[1, 5].Value = "Message";
                worksheet.Cells[1, 6].Value = "Status";
                worksheet.Cells[1, 7].Value = "Sent By";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.AdmissionNumber;
                    worksheet.Cells[row, 2].Value = item.StudentName;
                    worksheet.Cells[row, 3].Value = item.ClassSection;
                    worksheet.Cells[row, 4].Value = item.DateTime;
                    worksheet.Cells[row, 5].Value = item.Message;
                    worksheet.Cells[row, 6].Value = item.Status;
                    worksheet.Cells[row, 7].Value = item.SentBy;

                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WhatsAppReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<WhatsAppStudentReportExportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Admission Number, Student Name, Class Section, Date Time, Message, Status, Sent By");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.AdmissionNumber}, {item.StudentName}, {item.ClassSection}, {item.DateTime.Replace(",","")}, {item.Message.Replace(",", "")}, {item.Status}, {item.SentBy}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WhatsAppReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }

        public async Task<ServiceResponse<List<WhatsAppEmployeeReportsResponse>>> GetWhatsAppEmployeeReport(GetWhatsAppEmployeeReportRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppEmployeeReport(request);
        }

        public async Task<ServiceResponse<string>> GetWhatsAppEmployeeReportExport(WhatsAppEmployeeReportExportRequest request)
        {
            string filePath = await _whatsAppRepository.GetWhatsAppEmployeeReportExport(request);

            if (string.IsNullOrEmpty(filePath))
            {
                return new ServiceResponse<string>(false, "Failed to generate report", null, 400);
            }

            return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
        }

        public async Task<ServiceResponse<WhatsAppPlanResponse>> GetWhatsAppPlan(int WhatsAppVendorID)
        {
            return await _whatsAppRepository.GetWhatsAppPlan(WhatsAppVendorID);
        }


        public async Task<ServiceResponse<List<GetWhatsAppTopUpHistoryResponse>>> GetWhatsAppTopUpHistory(GetWhatsAppTopUpHistoryRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppTopUpHistory(request.InstituteID);
        }

        public async Task<ServiceResponse<List<GetWhatsAppTopUpHistoryExportResponse>>> GetWhatsAppTopUpHistoryExport(GetWhatsAppTopUpHistoryExportRequest request)
        {
            var exportData = await _whatsAppRepository.GetWhatsAppTopUpHistoryExport(request.InstituteID);

            var response = new ServiceResponse<List<GetWhatsAppTopUpHistoryExportResponse>>(true, "Export Data Retrieved", exportData, 200);
            return response;
        }
    }
}
