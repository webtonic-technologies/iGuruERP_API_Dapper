namespace StudentManagement_API.DTOs.Requests
{
    public class GetStudentInformationExportRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public string IPAddress { get; set; } 
        public int UserID { get; set; } 
        public int ClassID { get; set; }
        public int SectionID { get; set; }

        /// <summary>
        /// Status = 1 => isActive = 1  
        /// Status = 2 => isActive = 0  
        /// Status = 3 => isActive in (1,0)
        /// </summary>
        public int Status { get; set; }

        public int StudentTypeID { get; set; }
        public string Search { get; set; }

        /// <summary>
        /// ExportType = 1 for Excel and 2 for CSV
        /// </summary>
        public int ExportType { get; set; }
    }
}
