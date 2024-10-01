namespace TimeTable_API.DTOs.Requests
{
    public class GetAllDaySetupsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
