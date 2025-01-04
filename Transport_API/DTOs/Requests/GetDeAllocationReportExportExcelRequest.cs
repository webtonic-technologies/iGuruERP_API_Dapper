namespace Transport_API.DTOs.Requests
{
    public class GetDeAllocationReportExportExcelRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Date in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // Date in 'DD-MM-YYYY' format
        public int UserTypeID { get; set; }    // 1 for Employee, 2 for Student
    }
}
