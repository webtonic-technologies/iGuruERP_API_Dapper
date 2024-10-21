namespace Student_API.DTOs.RequestDTO
{
    public class GetClassPromotionLogParam : PagedListModel
    {
        public int institute_id { get; set; }
    }

    public class ExportClassPromotionLogParam
    {
        public int institute_id { get; set; }
        public int exportFormat { get; set; }
    }
}
