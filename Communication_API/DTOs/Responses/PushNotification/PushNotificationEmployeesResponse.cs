namespace Communication_API.DTOs.Responses.PushNotification
{
    public class PushNotificationEmployeesResponse
    {
        public int GroupID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public bool IsActive { get; set; }
    }
}
