using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Response
{
    //public class EmployeeAttendanceResponse
    //{
    //    //public bool Success { get; set; }
    //    //public string Message { get; set; }
    //    //public List<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    //    //public int StatusCode { get; set; }

    //    //public class AttendanceRecord
    //    //{
    //    //    public int EmployeeID { get; set; }
    //    //    public int StatusID { get; set; }
    //    //    public string Remarks { get; set; }
    //    //}

    //    public int Employee_id { get; set; }  
    //    public string Employee_code_id { get; set; }   
    //    public string EmployeeName { get; set; }
    //    public int StatusID { get; set; } // Attendance Status
    //    public string Remarks { get; set; }
    //}

    public class EmployeeAttendanceResponse
    {
        public int Employee_id { get; set; }
        public string Employee_code_id { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string StatusID { get; set; } // Optional to include the status
        public string Remarks { get; set; } // Optional, if needed
    }
}
