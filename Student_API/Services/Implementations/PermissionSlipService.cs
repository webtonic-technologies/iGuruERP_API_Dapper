using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class PermissionSlipService : IPermissionSlipService
    {
        private readonly IPermissionSlipRepository _repository;

        public PermissionSlipService(IPermissionSlipRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetAllPermissionSlips(classId, sectionId,pageNumber,pageSize);
        }
        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int classId, int sectionId, DateTime? startDate, DateTime? endDate, bool isApproved, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetPermissionSlips(classId, sectionId, startDate, endDate, isApproved,pageNumber, pageSize);
        }
        public async Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved)
        {
            return await _repository.UpdatePermissionSlipStatus(permissionSlipId, isApproved);
        }
    }

}
