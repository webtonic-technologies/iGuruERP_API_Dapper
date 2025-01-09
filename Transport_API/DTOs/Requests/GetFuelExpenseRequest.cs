namespace Transport_API.DTOs.Requests
{
    public class GetFuelExpenseRequest
    {
        public string StartDate { get; set; }  // String format: DD-MM-YYYY
        public string EndDate { get; set; }    // String format: DD-MM-YYYY
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
