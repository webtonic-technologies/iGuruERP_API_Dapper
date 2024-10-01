using System;
using System.Collections.Generic;

namespace TimeTable_API.DTOs.Requests
{
    public class AddUpdateGroupRequest
    {
        public int? GroupID { get; set; } // If null, create a new group, else update
        public string GroupName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; } = true;
        public List<SessionRequest> Sessions { get; set; } = new();
        public List<BreakRequest> Breaks { get; set; } = new();
        public List<ClassSectionRequest> ClassSections { get; set; } = new();
    }

    public class SessionRequest
    {
        public int? SessionID { get; set; }
        public string SessionName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class BreakRequest
    {
        public int? BreaksID { get; set; }
        public string BreakName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class ClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
