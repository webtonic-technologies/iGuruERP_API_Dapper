namespace TimeTable_API.DTOs.Requests
{
    public class GetSubstituteEmployeeListRequest
    {
        public int InstituteID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
