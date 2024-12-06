namespace Communication_API.DTOs.Requests.PushNotification
{
    public class PushNotificationEmployeesRequest
    {
        public int InstituteID { get; set; }
        public List<int> GroupIDs { get; set; }
        public int UserTypeStatus { get; set; }  // 1: Active, 2: Inactive, 3: All
    }
}
