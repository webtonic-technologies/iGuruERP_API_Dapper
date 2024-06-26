﻿namespace Attendance_API.Models
{
    public class ShiftTimingMaster
    {
        public int Shift_Timing_id { get; set; }
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Clock_In { get; set; } = string.Empty;
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Clock_Out { get; set; } = string.Empty;
        [TimeFormat(ErrorMessage = "Invalid time format. Please use HH:MM format.")]
        public string Late_Coming { get; set; } = string.Empty;
        public DateTime Applicable_Date { get; set; }
    }
}
