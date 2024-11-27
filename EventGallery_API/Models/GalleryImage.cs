namespace EventGallery_API.Models
{
    public class GalleryImage
    {
        public int GalleryID { get; set; }
        public int EventID { get; set; }
        public int InstituteID { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; }
        public string AcademicYearCode { get; set; }

    }
}
