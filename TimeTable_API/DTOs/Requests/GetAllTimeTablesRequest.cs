﻿namespace TimeTable_API.DTOs.Requests
{
    public class GetAllTimeTablesRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

}