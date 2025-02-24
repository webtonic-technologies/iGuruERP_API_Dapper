namespace StudentManagement_API.DTOs.Requests
{
    public class ChangePermissionSlipStatusRequest
    {
        public int InstituteID { get; set; }
        public int PermissionSlipID { get; set; }
        // Use 1 for Approve, 0 for Reject.
        public int Status { get; set; }
        public int StatusUpdatedBy { get; set; }

    }
}
