using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Communication_API.Services.Interfaces.PushNotification;
using CsvHelper;
using OfficeOpenXml;
using System.Text;

namespace Communication_API.Services.Implementations.PushNotification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request)
        {
            return await _notificationRepository.TriggerNotification(request);
        }
        public async Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request)
        {
            return await _notificationRepository.GetPushNotificationStudent(request);
        }
        public async Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request)
        {
            return await _notificationRepository.GetPushNotificationEmployee(request);
        }

        //public async Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request)
        //{
        //    return await _notificationRepository.GetNotificationReport(request);
        //}


        public async Task<ServiceResponse<List<GetNotificationStudentReportResponse>>> GetNotificationStudentReport(GetNotificationStudentReportRequest request)
        {
            return await _notificationRepository.GetNotificationStudentReport(request);
        }

        public async Task<ServiceResponse<string>> GetNotificationStudentReportExport(GetNotificationStudentReportExportRequest request)
        {
            // Fetch the SMS Student report data
            var reportData = await _notificationRepository.GetNotificationStudentReportExport(request);

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

        private string GenerateExcelFile(List<GetNotificationStudentReportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Push Notification Report");
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
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PushNotificationReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<GetNotificationStudentReportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Admission Number, Student Name, Class Section, Date Time, Message, Status, Sent By");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.AdmissionNumber}, {item.StudentName}, {item.ClassSection}, {item.DateTime.Replace(",", "")}, {item.Message.Replace(",", "")}, {item.Status}, {item.SentBy}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PushNotificationReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }
         
        public async Task<ServiceResponse<List<GetNotificationEmployeeReportResponse>>> GetNotificationEmployeeReport(GetNotificationEmployeeReportRequest request)
        {
            return await _notificationRepository.GetNotificationEmployeeReport(request);
        }






        public async Task<ServiceResponse<string>> GetNotificationEmployeeReportExport(GetNotificationEmployeeReportExportRequest request)
        {
            // Fetch the SMS Student report data
            var reportData = await _notificationRepository.GetNotificationEmployeeReportExport(request);

            if (reportData == null || !reportData.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            // Check the ExportType and return the corresponding file
            if (request.ExportType == 1)
            {
                // Generate Excel file
                var file = GenerateExcelFile_Employee(reportData);
                return new ServiceResponse<string>(true, "Excel file generated", file, 200);
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV file
                var file = GenerateCsvFile_Employee(reportData);
                return new ServiceResponse<string>(true, "CSV file generated", file, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }
        }

        private string GenerateExcelFile_Employee(List<GetNotificationEmployeeReportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Push Notification Report");
                worksheet.Cells[1, 1].Value = "Employee Name";
                worksheet.Cells[1, 2].Value = "Department Designation";
                worksheet.Cells[1, 3].Value = "DateTime";
                worksheet.Cells[1, 4].Value = "Message";
                worksheet.Cells[1, 5].Value = "Status";
                worksheet.Cells[1, 6].Value = "Sent By";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.EmployeeName;
                    worksheet.Cells[row, 2].Value = item.DepartmentDesignation;
                    worksheet.Cells[row, 3].Value = item.DateTime;
                    worksheet.Cells[row, 4].Value = item.Message;
                    worksheet.Cells[row, 5].Value = item.Status;
                    worksheet.Cells[row, 6].Value = item.SentBy;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PushNotificationReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile_Employee(List<GetNotificationEmployeeReportResponse> data)
        {
            var csvBuilder = new StringBuilder(); 

            csvBuilder.AppendLine("Employee Name,Department Designation,DateTime,Message,Status,Sent By");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.EmployeeName},{item.DepartmentDesignation},{item.DateTime.Replace(",", "")},{item.Message.Replace(",", "")},{item.Status}, {item.SentBy}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PushNotificationReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }









        public async Task<ServiceResponse<string>> SendPushNotificationStudent(SendPushNotificationStudentRequest request)
        {

            try
            {
                // Convert SMSDate from string to DateTime
                DateTime NotificationDate = DateTime.ParseExact(request.NotificationDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var student in request.StudentNotification)
                {
                    await _notificationRepository.InsertPushNotificationForStudent(request.GroupID, request.InstituteID, student.StudentID, student.Message, NotificationDate, 1, request.SentBy); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "Push Notification Sent Successfully", "Notifications have been scheduled for students", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }  
        }

        public async Task<ServiceResponse<string>> SendPushNotificationEmployee(SendPushNotificationEmployeeRequest request)
        {

            try
            {
                // Convert SMSDate from string to DateTime
                DateTime NotificationDate = DateTime.ParseExact(request.NotificationDate, "dd-MM-yyyy", null);

                // Iterate over each studentMessage and insert SMS data into the table
                foreach (var student in request.EmployeeNotification)
                {
                    await _notificationRepository.InsertPushNotificationForEmployee(request.GroupID, request.InstituteID, student.EmployeeID, student.Message, NotificationDate, 1, request.SentBy); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "Push Notification Sent Successfully", "Notifications have been scheduled for employees", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }


            //var pushNotificationStatusID = 0; // Pending status initially
            //var pushNotificationDate = request.PushNotificationDate;
            //string pushNotificationMessage = request.PushNotificationMessage;

            //// Insert the push notification details for each employee
            //foreach (var employeeID in request.EmployeeIDs)
            //{
            //    await _notificationRepository.InsertPushNotificationForEmployee(request.GroupID, request.InstituteID, employeeID, pushNotificationMessage, pushNotificationDate, pushNotificationStatusID);
            //}

            //return new ServiceResponse<string>(true, "Push Notification Sent Successfully", "Notifications have been scheduled for employees", StatusCodes.Status200OK);
        }

        public async Task<ServiceResponse<string>> UpdatePushNotificationStudentStatus(UpdatePushNotificationStudentStatusRequest request)
        {
            await _notificationRepository.UpdatePushNotificationStudentStatus(request.GroupID, request.InstituteID, request.StudentID, request.PushNotificationStatusID);

            return new ServiceResponse<string>(true, "Push Notification Status Updated Successfully", "The status has been successfully updated", StatusCodes.Status200OK);
        }

        public async Task<ServiceResponse<string>> UpdatePushNotificationEmployeeStatus(UpdatePushNotificationEmployeeStatusRequest request)
        {
            await _notificationRepository.UpdatePushNotificationEmployeeStatus(request.GroupID, request.InstituteID, request.EmployeeID, request.PushNotificationStatusID);

            return new ServiceResponse<string>(true, "Push Notification Status Updated Successfully", "The status has been successfully updated", StatusCodes.Status200OK);
        }
    }
}
