﻿namespace Attendance_SE_API.DTOs.Requests
{
    public class ClassAttendanceAnalysisRequest
    {
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; } // For pagination
        public int PageSize { get; set; }   // For pagination
    }
}
