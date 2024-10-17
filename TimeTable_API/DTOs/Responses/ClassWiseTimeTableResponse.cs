namespace TimeTable_API.DTOs.Responses
{
    public class ClassWiseTimeTableResponse
    {
        public string ClassName { get; set; } // Class name from tbl_Class
        public string SectionName { get; set; } // Section name from tbl_Section
        public Dictionary<string, string> Subjects { get; set; } // Mapping for subjects and number of periods per week

        public List<ClassWiseDayResponse> Days { get; set; } // List of days, each with its sessions and breaks

        public ClassWiseTimeTableResponse()
        {
            Subjects = new Dictionary<string, string>();
            Days = new List<ClassWiseDayResponse>(); // Initialize Days here
        }
    }

    public class ClassWiseDayResponse
    {
        public int DayID { get; set; } // DayID from tblTimeTableDaySetup
        public string DayType { get; set; } // DayType from tblTimeTableDayMaster

        public List<ClassWiseSessionResponse> Sessions { get; set; } // List of Sessions for the Day
        public List<ClassWiseBreakResponse> Breaks { get; set; } // List of Breaks for the Day

        public ClassWiseDayResponse()
        {
            Sessions = new List<ClassWiseSessionResponse>(); // Initialize Sessions
            Breaks = new List<ClassWiseBreakResponse>(); // Initialize Breaks
        }
    }

    public class ClassWiseSessionResponse
    {
        public int SessionID { get; set; } // SessionID from tblTimeTableSessions
        public string SessionName { get; set; } // SessionName from tblTimeTableSessions
        public string SessionTime { get; set; } // Formatted session time (e.g., 08:00am - 08:45am)

        public List<ClassWiseEmployeeSubjectResponse> EmployeeSubjects { get; set; } // List of Employee-Subject Mappings for the Session

        public ClassWiseSessionResponse()
        {
            EmployeeSubjects = new List<ClassWiseEmployeeSubjectResponse>();
        }
    }

    public class ClassWiseEmployeeSubjectResponse
    {
        public int SubjectID { get; set; } // SubjectID from tbl_Subjects
        public string SubjectName { get; set; } // SubjectName from tbl_Subjects
        public int EmployeeID { get; set; } // EmployeeID from tbl_EmployeeProfileMaster
        public string EmployeeName { get; set; } // Full name from tbl_EmployeeProfileMaster
    }

    public class ClassWiseBreakResponse
    {
        public int BreakID { get; set; } // BreakID from tblTimeTableBreaks
        public string BreakName { get; set; } // BreakName from tblTimeTableBreaks
        public string BreakTime { get; set; } // Formatted break time (e.g., 09:30am - 10:00am)
    }
}



//namespace TimeTable_API.DTOs.Responses
//{
//    public class ClassWiseTimeTableResponse
//    {
//        public string ClassName { get; set; } // Class name from tbl_Class
//        public string SectionName { get; set; } // Section name from tbl_Section
//        public Dictionary<string, string> Subjects { get; set; } // Mapping for subjects and number of periods per week

//        public int DayID { get; set; } // DayID from tblTimeTableDaySetup
//        public string DayType { get; set; } // DayType from tblTimeTableDayMaster
//        public List<ClassWiseSessionResponse> Sessions { get; set; } // List of Sessions for the Day
//        public List<ClassWiseBreakResponse> Breaks { get; set; } // List of Breaks for the Day

//        public ClassWiseTimeTableResponse()
//        {
//            Subjects = new Dictionary<string, string>();
//            Sessions = new List<ClassWiseSessionResponse>();
//            Breaks = new List<ClassWiseBreakResponse>();
//        }
//    }

//    public class ClassWiseSessionResponse
//    {
//        public int SessionID { get; set; } // SessionID from tblTimeTableSessions
//        public string SessionName { get; set; } // SessionName from tblTimeTableSessions
//        public string SessionTime { get; set; } // Formatted session time (e.g., 08:00am - 08:45am)

//        public List<ClassWiseEmployeeSubjectResponse> EmployeeSubjects { get; set; } // List of Employee-Subject Mappings for the Session

//        public ClassWiseSessionResponse()
//        {
//            EmployeeSubjects = new List<ClassWiseEmployeeSubjectResponse>();
//        }
//    }

//    public class ClassWiseEmployeeSubjectResponse
//    {
//        public int SubjectID { get; set; } // SubjectID from tbl_Subjects
//        public string SubjectName { get; set; } // SubjectName from tbl_Subjects
//        public int EmployeeID { get; set; } // EmployeeID from tbl_EmployeeProfileMaster
//        public string EmployeeName { get; set; } // Full name from tbl_EmployeeProfileMaster
//    }

//    public class ClassWiseBreakResponse
//    {
//        public int BreakID { get; set; } // BreakID from tblTimeTableBreaks
//        public string BreakName { get; set; } // BreakName from tblTimeTableBreaks
//        public string BreakTime { get; set; } // Formatted break time (e.g., 09:30am - 10:00am)
//    }

//    public class ClassWiseDayResponse
//    {
//        public int DayID { get; set; } // DayID from tblTimeTableDaySetup
//        public string DayType { get; set; } // DayType from tblTimeTableDayMaster
//    }
//}
