namespace Communication_API.DTOs.Requests.PushNotification
{
    public class TriggerNotificationRequest
    {
        public int PushNotificationID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string NotificationMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        //public bool Status { get; set; }
        public bool ScheduleNow { get; set; } 
        public string ScheduleDate { get; set; }  // Change to string
        public string ScheduleTime { get; set; }  // Change to string

        public string AcademicYearCode { get; set; }  // Add AcademicYearCode as string
        public int InstituteID { get; set; }  // Add InstituteID as integer



        // Lists for student and employee IDs
        public List<int>? StudentIDs { get; set; }
        public List<int>? EmployeeIDs { get; set; }

        public int SentBy { get; set; }
    }


}
