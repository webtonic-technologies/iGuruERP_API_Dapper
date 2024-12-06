using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Communication_API.Services.Interfaces.PushNotification;

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

        public async Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request)
        {
            return await _notificationRepository.GetNotificationReport(request);
        }
        public async Task<ServiceResponse<string>> SendPushNotificationStudent(SendPushNotificationStudentRequest request)
        {
            // Step 1: Save push notification details in the table
            var pushNotificationStatusID = 0; // Pending status initially
            var pushNotificationDate = request.PushNotificationDate;
            string pushNotificationMessage = request.PushNotificationMessage;

            // Insert the push notification details for each student
            foreach (var studentID in request.StudentIDs)
            {
                await _notificationRepository.InsertPushNotificationForStudent(request.GroupID, request.InstituteID, studentID, pushNotificationMessage, pushNotificationDate, pushNotificationStatusID);
            }

            return new ServiceResponse<string>(true, "Push Notification Sent Successfully", "Notifications have been scheduled for students", StatusCodes.Status200OK);
        }

        public async Task<ServiceResponse<string>> SendPushNotificationEmployee(SendPushNotificationEmployeeRequest request)
        {
            var pushNotificationStatusID = 0; // Pending status initially
            var pushNotificationDate = request.PushNotificationDate;
            string pushNotificationMessage = request.PushNotificationMessage;

            // Insert the push notification details for each employee
            foreach (var employeeID in request.EmployeeIDs)
            {
                await _notificationRepository.InsertPushNotificationForEmployee(request.GroupID, request.InstituteID, employeeID, pushNotificationMessage, pushNotificationDate, pushNotificationStatusID);
            }

            return new ServiceResponse<string>(true, "Push Notification Sent Successfully", "Notifications have been scheduled for employees", StatusCodes.Status200OK);
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
