using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;
using Communication_API.Repository.Interfaces.Email;
using Communication_API.Services.Interfaces.Email;
using OfficeOpenXml;
using System.Text;

namespace Communication_API.Services.Implementations.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request)
        {
            return await _emailRepository.ConfigureEmail(request);
        }

        public async Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request)
        {
            return await _emailRepository.SendNewEmail(request);
        }

        public async Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request)
        {
            return await _emailRepository.GetEmailReports(request);
        }

        public async Task<ServiceResponse<string>> SendEmailToStudents(SendEmailStudentRequest request)
        {
            try
            { 
                // Convert SMSDate from string to DateTime
                DateTime EmailDate = DateTime.ParseExact(request.EmailDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var student in request.StudentEmail)
                { 
                    await _emailRepository.InsertEmailForStudent(request.GroupID, request.InstituteID, student.StudentID, student.EmailSubject, student.EmailBody, EmailDate, 1); // Assuming SMSStatusID is 1

                }

                return new ServiceResponse<string>(true, "Email sent successfully to students.", "Email sent successfully", 200); 
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send email", ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<string>> SendEmailToEmployees(SendEmailEmployeeRequest request)
        {
            try
            { 
                // Convert SMSDate from string to DateTime
                DateTime EmailDate = DateTime.ParseExact(request.EmailDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var employee in request.EmployeeEmail)
                {
                    await _emailRepository.InsertEmailForEmployee(request.GroupID, request.InstituteID, employee.EmployeeID, employee.EmailSubject, employee.EmailBody, EmailDate, 1); // Assuming SMSStatusID is 1

                }

                return new ServiceResponse<string>(true, "Email sent successfully to Employees.", "Email sent successfully", 200);
                 
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send email", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmailStudentStatus(UpdateEmailStudentStatusRequest request)
        {
            try
            {
                // Update the email status for the student
                await _emailRepository.UpdateEmailStatusForStudent(request.GroupID, request.InstituteID, request.StudentID, request.EmailStatusID);

                return new ServiceResponse<string>(true, "Email status updated successfully.", "Email status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update email status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmailEmployeeStatus(UpdateEmailEmployeeStatusRequest request)
        {
            try
            {
                // Update the email status for the employee
                await _emailRepository.UpdateEmailStatusForEmployee(request.GroupID, request.InstituteID, request.EmployeeID, request.EmailStatusID);

                return new ServiceResponse<string>(true, "Email status updated successfully.", "Email status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update email status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<List<EmailStudentReportsResponse>>> GetEmailStudentReport(GetEmailStudentReportRequest request)
        {
            return await _emailRepository.GetEmailStudentReport(request);
        }

        public async Task<ServiceResponse<string>> GetEmailStudentReportExport(EmailStudentReportExportRequest request)
        {
            // Fetch the SMS Student report data
            var reportData = await _emailRepository.GetEmailStudentReportData(request);

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

        private string GenerateExcelFile(List<EmailStudentReportExportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Email Report");
                worksheet.Cells[1, 1].Value = "Admission Number";
                worksheet.Cells[1, 2].Value = "Student Name";
                worksheet.Cells[1, 3].Value = "Class Section";
                worksheet.Cells[1, 4].Value = "Date Time";
                worksheet.Cells[1, 5].Value = "Email Subject";
                worksheet.Cells[1, 6].Value = "Email ID";  
                worksheet.Cells[1, 7].Value = "Status";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.AdmissionNumber;
                    worksheet.Cells[row, 2].Value = item.StudentName;
                    worksheet.Cells[row, 3].Value = item.ClassSection;
                    worksheet.Cells[row, 4].Value = item.DateTime;
                    worksheet.Cells[row, 5].Value = item.EmailSubject;
                    worksheet.Cells[row, 6].Value = item.EmailID;
                    worksheet.Cells[row, 7].Value = item.Status;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<EmailStudentReportExportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Admission Number, Student Name, Class Section, Date Time, Email Subject, Email ID, Status");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.AdmissionNumber}, {item.StudentName}, {item.ClassSection}, {item.DateTime}, {item.EmailSubject}, {item.EmailID}, {item.Status}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }

        public async Task<ServiceResponse<List<EmailEmployeeReportsResponse>>> GetEmailEmployeeReport(GetEmailEmployeeReportRequest request)
        {
            return await _emailRepository.GetEmailEmployeeReport(request);
        }
         
        
      //public async Task<ServiceResponse<string>> GetEmailEmployeeReportExport(EmailEmployeeReportExportRequest request)
      //{
      //    string filePath = await _emailRepository.GetEmailEmployeeReportExport(request);

      //    if (string.IsNullOrEmpty(filePath))
      //    {
      //        return new ServiceResponse<string>(false, "Failed to generate report", null, 400);
      //    }

      //    return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
      //}


    }
}
