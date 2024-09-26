namespace EventGallery_API.DTOs.Requests
{

    public class GetAllEventsRequest
    {
        public int AcademicYearID { get; set; }
        public int InstituteID { get; set; }  // Ensure there is only one definition
        public string Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class EventRequest
    {
        public int? EventID { get; set; }
        public string EventName { get; set; }
        public string FromDate { get; set; }  // Accepting string for DD-MM-YYYY format
        public string ToDate { get; set; }    // Accepting string for DD-MM-YYYY format
        public string Description { get; set; }
        public string Location { get; set; }
        public string ScheduleDate { get; set; }  // Accepting string for DD-MM-YYYY format
        public string ScheduleTime { get; set; }  // Accepting string for hh:mm AM/PM format
        public int AcademicYearID { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }

        // Students mapping
        public StudentsMapping Students { get; set; }

        // Employee mapping
        public EmployeeMapping Employee { get; set; }

        // Attachment
        public string Attachment { get; set; }  // Base64 string for attachment
    }

    public class StudentsMapping
    {
        public bool All { get; set; }
        public List<ClassSection>? ClassSection { get; set; }
    }

    public class ClassSection
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class EmployeeMapping
    {
        public bool All { get; set; }
        public List<int>? EmployeeID { get; set; }  // EmployeeID list if All = false
    }
}
