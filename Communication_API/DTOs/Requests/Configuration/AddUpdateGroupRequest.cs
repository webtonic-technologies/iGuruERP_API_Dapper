namespace Communication_API.DTOs.Requests.Configuration
{
    public class AddUpdateGroupRequest
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int AcadamicYearID { get; set; }
        public int TypeID { get; set; }
    }
}
