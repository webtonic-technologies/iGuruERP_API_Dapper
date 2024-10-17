//namespace TimeTable_API.DTOs.Responses
//{
//    public class TimeTableResponse
//    {
//        public int TimeTableID { get; set; }
//        public string AcademicYearCode { get; set; }
//        public int ClassID { get; set; }
//        public int SectionID { get; set; }
//        public string DayName { get; set; }
//        public bool IsBreak { get; set; }
//        public int SessionID { get; set; }
//        public int BreaksID { get; set; }
//        public int SubjectID { get; set; }
//        public int EmployeeID { get; set; }
//        public int InstituteID { get; set; }
//    }
//}

namespace TimeTable_API.DTOs.Responses
{
    public class TimeTableResponse
    {
        public int DayID { get; set; } // DayID from tblTimeTableDaySetup
        public string DayType { get; set; } // DayType from tblTimeTableDayMaster
        public List<SessionResponse> Sessions { get; set; } // List of Sessions for the Day
        public List<BreakResponse> Breaks { get; set; } // List of Breaks for the Day

        public TimeTableResponse()
        {
            Sessions = new List<SessionResponse>();
            Breaks = new List<BreakResponse>();
        }
    }

    public class SessionResponse
    {
        public int SessionID { get; set; } // SessionID from tblTimeTableSessions
        public string SessionName { get; set; } // SessionName from tblTimeTableSessions
        public string SessionTime { get; set; } // Formatted session time (08.00am - 08.45am)

        public List<EmployeeSubjectResponse> EmployeeSubjects { get; set; } // List of Employee-Subject Mappings for the Session

        public SessionResponse()
        {
            EmployeeSubjects = new List<EmployeeSubjectResponse>();
        }
    }

    public class EmployeeSubjectResponse
    {
        public int SubjectID { get; set; } // SubjectId from tbl_Subjects
        public string SubjectName { get; set; } // SubjectName from tbl_Subjects
        public int EmployeeID { get; set; } // Employee_id from tbl_EmployeeProfileMaster
        public string EmployeeName { get; set; } // Full name from tbl_EmployeeProfileMaster
    }

    public class BreakResponse
    {
        public int BreakID { get; set; } // BreakID from tblTimeTableBreaks
        public string BreakName { get; set; } // BreakName from tblTimeTableBreaks
        public string BreakTime { get; set; } // Formatted break time (08.00am - 08.45am)

    }


    public class DayResponse
    {
        public int DayID { get; set; } // DayID from tblTimeTableDaySetup
        public string DayType { get; set; } // DayType from tblTimeTableDayMaster
    }
}

