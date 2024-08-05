namespace Student_API.DTOs
{
    public class UpdatePermissionSlipStatusRequest
    {
        public int PermissionSlipId { get; set; }
        public bool IsApproved { get; set; }
    }
}
