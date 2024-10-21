using Student_API.Helper;

namespace Student_API.DTOs.RequestDTO
{
    public class GetAllPermissionSlips 
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
        public int? pageNumber { get; set; } = null;
        public int? pageSize { get; set; } = null;
    }
    public class GetAllPermissionSlipsByStatus
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;

        [ValidDateString("dd-MM-yyyy")]
        public string startDate { get; set; }

        [ValidDateString("dd-MM-yyyy")]
        public string endDate { get; set; }
        public int? pageNumber { get; set; } = null;
        public int? pageSize { get; set; } = null;
    }
    public class GetAllPermissionSlip
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
        public int exportFormat {  get; set; }  
    }
    public class GetAllPermissionSlipsByStatusExport
    {
        public int Institute_id { get; set; }
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;

        [ValidDateString("dd-MM-yyyy")]
        public string startDate { get; set; }

        [ValidDateString("dd-MM-yyyy")]
        public string endDate { get; set; }
        public int exportFormat { get; set; }
    }

    public class GetSinglePermissionSlip
    {
        public int permissionSlipId { get; set; }   
        public int Institute_id { get; set; }
        
    }
}
