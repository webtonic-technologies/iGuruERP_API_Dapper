﻿namespace Institute_API.DTOs
{
    public class HolidayDTO
    {
        public int Holiday_id { get; set; }
        public string HolidayName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public int Institute_id { get; set; }
        public int Academic_year_id { get; set; }
        public string YearName { get; set; }
        public List<HolidayClassSessionMappingDTO> ClassSessionMappings { get; set; }
    }

    public class HolidayClassSessionMappingDTO
    {
        public int HolidayClassSessionMapping_id { get; set; }
        public int Holiday_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
        public string? section_name {  get; set; }    
        public string? class_name {  get; set; }   
    }
}
