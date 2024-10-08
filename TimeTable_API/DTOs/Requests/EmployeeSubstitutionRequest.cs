namespace TimeTable_API.DTOs.Requests
{
    public class EmployeeSubstitutionRequest
    {
        public string AcademicYearCode { get; set; }
        public string Date { get; set; } // Date in DD-MM-YYYY format
        public int EmployeeID { get; set; }
    }



    public class EmployeeSubstitutionRequest_Update
    {
        public string AcademicYearCode { get; set; }
        public string SubstitutesDate { get; set; } // Date in 'DD-MM-YYYY' format
        public int SubjectID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SessionID { get; set; }
        public int EmployeeID { get; set; }
        public int SubstitutesEmployeeID { get; set; }
        public int InstituteID { get; set; }
    }
}
