namespace EventGallery_API.DTOs.Requests
{
    public class GalleryApprovalRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; }
        public int ApprovalID { get; set; }
        public int Status { get; set; }
    }
}
