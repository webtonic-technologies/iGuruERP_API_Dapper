namespace Employee_API.DTOs
{
    public class GetAllEmployeeListRequest
    {
       public int InstituteId {  get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageNumber {  get; set; }
    }
}
