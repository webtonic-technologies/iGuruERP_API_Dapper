using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Communication_API.Services.Interfaces.SMS;
using OfficeOpenXml;
using System.Text;

namespace Communication_API.Services.Implementations.SMS
{
    public class SMSService : ISMSService
    {
        private readonly ISMSRepository _smsRepository;

        public SMSService(ISMSRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        public async Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request)
        {
            return await _smsRepository.SetupSMSConfiguration(request);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            return await _smsRepository.GetSMSBalance(VendorID);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            return await _smsRepository.CreateSMSTemplate(request);
        }

        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            return await _smsRepository.GetAllSMSTemplate(request);
        }

        public async Task<ServiceResponse<List<SMSTemplateExportResponse>>> GetAllSMSTemplateExport(SMSTemplateExportRequest request)
        {
            // Fetch data from repository
            var templates = await _smsRepository.GetAllSMSTemplateExport(request.InstituteID);

            var response = new ServiceResponse<List<SMSTemplateExportResponse>>(true, "Export Data Retrieved", templates, 200);
            return response;
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            return await _smsRepository.SendNewSMS(request);
        }

        public async Task<ServiceResponse<List<SMSStudentReportsResponse>>> GetSMSStudentReport(GetSMSStudentReportRequest request)
        {
            return await _smsRepository.GetSMSStudentReport(request);
        }

        public async Task<ServiceResponse<string>> GetSMSStudentReportExport(SMSStudentReportExportRequest request)
        {
            // Fetch the SMS Student report data
            var reportData = await _smsRepository.GetSMSStudentReportData(request);

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

        private string GenerateExcelFile(List<SMSStudentReportExportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SMS Report");
                worksheet.Cells[1, 1].Value = "Admission Number";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Class Section";
                worksheet.Cells[1, 4].Value = "Date Time";
                worksheet.Cells[1, 5].Value = "Message";
                worksheet.Cells[1, 6].Value = "Status";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.AdmissionNumber;
                    worksheet.Cells[row, 2].Value = item.StudentName;
                    worksheet.Cells[row, 3].Value = item.ClassSection;
                    worksheet.Cells[row, 4].Value = item.DateTime;
                    worksheet.Cells[row, 5].Value = item.Message;
                    worksheet.Cells[row, 6].Value = item.Status;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "SMSReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<SMSStudentReportExportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Admission Number, Student Name, Class Section, Date Time, Message, Status");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.AdmissionNumber}, {item.StudentName}, {item.ClassSection}, {item.DateTime}, {item.Message}, {item.Status}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "SMSReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }

        public async Task<ServiceResponse<List<SMSEmployeeReportsResponse>>> GetSMSEmployeeReport(GetSMSEmployeeReportRequest request)
        {
            return await _smsRepository.GetSMSEmployeeReport(request);
        }

        public async Task<ServiceResponse<string>> GetSMSEmployeeReportExport(SMSEmployeeReportExportRequest request)
        {
            string filePath = await _smsRepository.GetSMSEmployeeReportExport(request);

            if (string.IsNullOrEmpty(filePath))
            {
                return new ServiceResponse<string>(false, "Failed to generate report", null, 400);
            }

            return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
        }


        //public async Task<ServiceResponse<string>> SendSMSStudent(SendSMSStudentRequest request)
        //{
        //    try
        //    {
        //        // Convert SMSDate from string to DateTime
        //        DateTime smsDate = DateTime.ParseExact(request.SMSDate, "dd-MM-yyyy", null);

        //        // Iterate over each student ID and insert SMS data into the table
        //        foreach (var studentID in request.StudentIDs)
        //        {
        //            await _smsRepository.InsertSMSForStudent(request.GroupID, request.InstituteID, studentID, request.SMSMessage, smsDate, 1); // Assuming SMSStatusID is 1
        //        }

        //        return new ServiceResponse<string>(true, "SMS sent successfully to students.", "SMS sent successfully", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
        //    }
        //}

        public async Task<ServiceResponse<string>> SendSMSStudent(SendSMSStudentRequest request)
        {
            try
            {
                // Convert SMSDate from string to DateTime
                DateTime smsDate = DateTime.ParseExact(request.SMSDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var student in request.StudentMessages)
                {
                    await _smsRepository.InsertSMSForStudent(request.GroupID, request.InstituteID, student.StudentID, student.Message, smsDate, 1); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "SMS sent successfully to students.", "SMS sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }
        }


        public async Task<ServiceResponse<string>> SendSMSEmployee(SendSMSEmployeeRequest request)
        {
            try
            {
                // Convert SMSDate from string to DateTime
                DateTime smsDate = DateTime.ParseExact(request.SMSDate, "dd-MM-yyyy", null);

                // Iterate over each employeeMessage and insert SMS data into the table
                foreach (var employee in request.EmployeeMessages)
                {
                    await _smsRepository.InsertSMSForEmployee(request.GroupID, request.InstituteID, employee.EmployeeID, employee.Message, smsDate, 1); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "SMS sent successfully to employees.", "SMS sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }
        }



        public async Task<ServiceResponse<string>> UpdateSMSStudentStatus(UpdateSMSStudentStatusRequest request)
        {
            try
            {
                // Call the repository method to update the SMS status in the database
                await _smsRepository.UpdateSMSStudentStatus(request.GroupID, request.InstituteID, request.StudentID, request.SMSStatusID);

                return new ServiceResponse<string>(true, "SMS status updated successfully.", "SMS status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update SMS status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateSMSEmployeeStatus(UpdateSMSEmployeeStatusRequest request)
        {
            try
            {
                // Call the repository method to update the SMS status in the database
                await _smsRepository.UpdateSMSEmployeeStatus(request.GroupID, request.InstituteID, request.EmployeeID, request.SMSStatusID);

                return new ServiceResponse<string>(true, "SMS status updated successfully.", "SMS status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update SMS status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<List<SMSTemplateDDLResponse>>> GetSMSTemplateDDL(SMSTemplateDDLRequest request)
        {
            return await _smsRepository.GetSMSTemplateDDL(request.InstituteID);
        }
    }
}
