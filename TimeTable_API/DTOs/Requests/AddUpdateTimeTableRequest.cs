//namespace TimeTable_API.DTOs.Requests
//{
//    public class AddUpdateTimeTableRequest
//    {
//        public int TimeTableID { get; set; }
//        public int InstituteID { get; set; }
//        public string AcademicYearCode { get; set; }
//        public int ClassID { get; set; }
//        public int SectionID { get; set; }
//        public List<DayRequest> Days { get; set; } // List of days

//        public class DayRequest
//        {
//            public int DayID { get; set; }
//            public bool IsBreak { get; set; }
//            public List<Break> Breaks { get; set; } // Breaks for that day
//            public List<Session> Sessions { get; set; } // Sessions for that day

//            public class Break
//            {
//                public int BreaksID { get; set; }
//            }

//            public class Session
//            {
//                public int SessionID { get; set; }
//                public int SubjectID { get; set; }
//                public int EmployeeID { get; set; }
//            }
//        }
//    }
//}

namespace TimeTable_API.DTOs.Requests
{
    public class AddUpdateTimeTableRequest
    {
        public int TimeTableID { get; set; } // TimeTableID in tblTimeTableMaster
        public int InstituteID { get; set; } // InstituteID in tblTimeTableMaster
        public string AcademicYearCode { get; set; } // AcademicYearCode in tblTimeTableMaster
        public int ClassID { get; set; } // ClassID in tblTimeTableMaster
        public int SectionID { get; set; } // SectionID in tblTimeTableMaster
        public int GroupID { get; set; } // GroupID to be fetched from tblTimeTableClassSession and added to tblTimeTableMaster
        public List<DayRequest> Days { get; set; } // Days associated with the TimeTable

        public AddUpdateTimeTableRequest()
        {
            Days = new List<DayRequest>();
        }
    }

    public class DayRequest
    {
        public int DayID { get; set; } // DayID from tblTimeTableDayMaster
        public List<SessionMappingRequest> Sessions { get; set; } // Sessions for the Day
        public List<BreakMappingRequest> Breaks { get; set; } // Breaks for the Day

        public DayRequest()
        {
            Sessions = new List<SessionMappingRequest>();
            Breaks = new List<BreakMappingRequest>();
        }
    }

    public class SessionMappingRequest
    {
        public int TTSessionID { get; set; } // TTSessionID in tblTimeTableSessionMapping
        public int SessionID { get; set; } // SessionID in tblTimeTableSessions
        public int GroupID { get; set; } // GroupID fetched from tblTimeTableClassSession and added to tblTimeTableSessionMapping
        public List<EmployeeSubjectRequest> EmployeeSubjects { get; set; } // Employee-Subject mappings

        public SessionMappingRequest()
        {
            EmployeeSubjects = new List<EmployeeSubjectRequest>();
        }
    }

    public class EmployeeSubjectRequest
    {
        public int TSSEID { get; set; } // TSSEID in tblTimeTableSessionSubjectEmployee
        public int TTSessionID { get; set; } // TTSessionID from tblTimeTableSessionMapping
        public int SubjectID { get; set; } // SubjectID in tblTimeTableSessionSubjectEmployee
        public int EmployeeID { get; set; } // EmployeeID in tblTimeTableSessionSubjectEmployee
    }

    public class BreakMappingRequest
    {
        public int TTBreakID { get; set; } // TTBreakID in tblTimeTableBreakMapping
        public int BreaksID { get; set; } // BreaksID in tblTimeTableBreaks
        public int GroupID { get; set; } // GroupID fetched from tblTimeTableClassSession and added to tblTimeTableBreakMapping
    }
}

