namespace UserRoleManagement_API.DTOs.Requests
{
    public class GetUserRolesRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; }
        public int PageNumber { get; set; }  // New paging parameter
        public int PageSize { get; set; }    // New paging parameter

    }
}
