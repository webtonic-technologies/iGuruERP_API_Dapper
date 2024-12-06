namespace Communication_API.DTOs.Requests.PushNotification
{
    public class PushNotificationStudentsRequest
    {
        public int InstituteID { get; set; }
        //public int GroupID { get; set; }
        public List<int> GroupIDs { get; set; }  // List of GroupIDs

        public int UserTypeStatus { get; set; }  // 1: Active, 2: Inactive, 3: All
    }
}
