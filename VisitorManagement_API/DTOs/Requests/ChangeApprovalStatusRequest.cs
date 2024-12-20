namespace VisitorManagement_API.DTOs.Requests
{
    public class ChangeApprovalStatusRequest
    {
        public int VisitorID { get; set; }
        public int ApprovalTypeID { get; set; }
        public int InstituteID { get; set; }
    }
}
