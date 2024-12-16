namespace Attendance_SE_API.DTOs.Requests
{
    public class SubjectAttendanceAnalysisRequest
    {
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public List<int> SectionIDs { get; set; } // Modified to accept multiple sections
        public int InstituteID { get; set; }
        public List<int> SubjectIDs { get; set; } // Modified to accept multiple subjects
    }

    //public class SubjectAttendanceAnalysisRequest1
    //{
    //    public string AcademicYearCode { get; set; }
    //    public int ClassID { get; set; }
    //    public int SectionID { get; set; }
    //    public int InstituteID { get; set; } 
    //}

    public class SubjectAttendanceAnalysisRequest1
    {
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        //public int SectionID { get; set; }
        public List<int> SectionIDs { get; set; }
        public int InstituteID { get; set; }
        public List<int> SubjectIDs { get; set; }
        public int PageNumber { get; set; } // For pagination
        public int PageSize { get; set; }   // For pagination
    }

}
