namespace LibraryManagement_API.DTOs.Requests
{
    public class GetAllLibraryInchargeRequest
    {
        public int InstituteID { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
    }
}
