namespace Employee_API.DTOs
{
    public class GetAllEmployeeListRequest
    {
       public int InstituteId {  get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
    }
}
