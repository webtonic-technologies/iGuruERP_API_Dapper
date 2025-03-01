namespace StudentManagement_API.DTOs.Requests
{
    public class GetLoginCredentialsExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Search { get; set; }
        /// <summary>
        /// 1 = Excel, 2 = CSV
        /// </summary>
        public int ExportType { get; set; }
    }
}
