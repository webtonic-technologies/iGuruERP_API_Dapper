namespace Institute_API.DTOs
{
    public class CommonRequestDTO
    {
        public int Institute_id { get; set; }
        public int Academic_year_id { get; set; }
        public int Status { get; set; }
        public string sortColumn { get; set; }
        public string sortDirection { get; set; }
        public int? pageSize { get; set; } = null;
        public int? pageNumber { get; set; } = null;
        //public bool isExcel {  get; set; }  
    }

    public class CommonExportRequest
    {
        public int Institute_id { get; set; }
        public int Academic_year_id { get; set; }
        public int Status { get; set; }
    }
}
