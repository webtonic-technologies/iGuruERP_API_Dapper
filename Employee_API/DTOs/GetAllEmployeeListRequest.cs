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
    public class EmployeeLoginRequest
    {
        public int InstituteId { set; get; }
        public int DepartmentId {  set; get; }
        public int DesignationId {  set; get; }
        public string SearchText { set; get; } = string.Empty;
        public int PageNumber {  set; get; }
        public int PageSize {  set; get; }
    }
}
