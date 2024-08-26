using Student_API.Helper;

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

        [ValidDateString("dd-MM-yyyy")]
        public string startDate { get; set; }

        [ValidDateString("dd-MM-yyyy")]
        public string endDate { get; set; }
    }
}
