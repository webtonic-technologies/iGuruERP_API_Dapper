namespace Communication_API.DTOs.Requests.DigitalDiary
{
    public class GetAllDiaryRequest
    {
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; } 
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; } // Changed to string - Format 'DD-MM-YYYY'
        public string EndDate { get; set; }   // Changed to string - Format 'DD-MM-YYYY'
        public string Search { get; set; }
    }

}
