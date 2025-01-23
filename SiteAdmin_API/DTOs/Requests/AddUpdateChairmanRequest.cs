namespace SiteAdmin_API.DTOs.Requests
{
    public class AddUpdateChairmanRequest
    {
        public int ChairmanID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<InstituteRequest> Institutes { get; set; }  // List of institutes associated with the chairman
    }

    public class InstituteRequest
    {
        public int InstituteID { get; set; }  // InstituteID to be inserted into tblInstituteChairmanAssociation
    }
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
      //  public string EmailID { get; set; }
    }

}
