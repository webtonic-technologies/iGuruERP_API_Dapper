using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Response
{
    public class StudentAttendanceResponse
    {
        public int student_id { get; set; } // Ensure this is mapped to the correct field
        public string Admission_Number { get; set; } // New property for Admission Number
        public string Roll_Number { get; set; } // New property for Roll Number
        public string StudentName { get; set; }
        public int StatusID { get; set; } // Attendance Status
        public string Remarks { get; set; }
    }
}
