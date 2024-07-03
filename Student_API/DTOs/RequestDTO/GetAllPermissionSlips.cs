namespace Student_API.DTOs.RequestDTO
{
    public class GetAllPermissionSlips : PagedListModel
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
    }
    public class GetAllPermissionSlipsByStatus : PagedListModel
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
