using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class PurposeTypeService : IPurposeTypeService
    {
        private readonly IPurposeTypeRepository _purposeTypeRepository;

        public PurposeTypeService(IPurposeTypeRepository purposeTypeRepository)
        {
            _purposeTypeRepository = purposeTypeRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdatePurposeType(PurposeType purposeType)
        {
            return await _purposeTypeRepository.AddUpdatePurposeType(purposeType);
        }

        public async Task<ServiceResponse<IEnumerable<PurposeType>>> GetAllPurposeTypes(GetAllPurposeTypeRequest request)
        {
            return await _purposeTypeRepository.GetAllPurposeTypes(request);
        }

        public async Task<ServiceResponse<PurposeType>> GetPurposeTypeById(int purposeTypeId)
        {
            return await _purposeTypeRepository.GetPurposeTypeById(purposeTypeId);
        }

        public async Task<ServiceResponse<bool>> UpdatePurposeTypeStatus(int purposeTypeId)
        {
            return await _purposeTypeRepository.UpdatePurposeTypeStatus(purposeTypeId);
        }
    }
}
