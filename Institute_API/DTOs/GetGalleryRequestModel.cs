namespace Institute_API.DTOs
{
    public class GetGalleryRequestModel
    {
        public int Institute_id { get; set; }
        public int Status { get; set; }

        public int? pageSize { get; set; } = null;
        public int? pageNumber { get; set; } = null;
    }
}
