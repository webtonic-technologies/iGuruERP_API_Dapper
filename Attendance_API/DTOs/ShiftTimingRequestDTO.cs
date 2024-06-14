namespace Attendance_API.DTOs
{
    public class ShiftTimingRequestDTO
    {
        public int Shift_Timing_id { get; set; }
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Clock_In { get; set; } = string.Empty;
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Clock_Out { get; set; } = string.Empty;
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Late_Coming { get; set; } = string.Empty;
        public DateTime Applicable_Date { get; set; }
        public List<int> Designations { get; set; }
    }

    public class ShiftTimingFilterDTO 
    {
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
    }
}
