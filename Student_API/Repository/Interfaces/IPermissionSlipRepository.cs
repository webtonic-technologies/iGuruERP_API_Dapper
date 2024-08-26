using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IPermissionSlipRepository
    {
        Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int Institute_id,int classId, int sectionId, int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved);
        Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int Institute_id,int classId, int sectionId, string startDate, string endDate, bool isApproved, int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<string>> AddPermissionSlip(PermissionSlip permissionSlipDto);
        Task<ServiceResponse<SinglePermissionSlipDTO>> GetPermissionSlipById(int permissionSlipId);
    }
}
