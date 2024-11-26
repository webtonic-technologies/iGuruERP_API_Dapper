namespace EventGallery_API.DTOs.Requests
{
    public class GalleryImageRequest
    {
        public int GalleryID { get; set; }
        public int EventID { get; set; }
        public int InstituteID { get; set; }
        public string FileName { get; set; } // Base64 encoded image
        public string AcademicYearCode { get; set; }
    }

    public class GalleryImageRequest_Get
    {
        public int EventID { get; set; }
        public int InstituteID { get; set; }
    } 
}
