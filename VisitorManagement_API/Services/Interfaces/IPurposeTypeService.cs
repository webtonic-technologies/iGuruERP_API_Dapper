using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface IPurposeTypeService
    {
        Task<ServiceResponse<string>> AddUpdatePurposeType(PurposeType purposeType);
        Task<ServiceResponse<IEnumerable<PurposeType>>> GetAllPurposeTypes(GetAllPurposeTypeRequest request);
        Task<ServiceResponse<PurposeType>> GetPurposeTypeById(int purposeTypeId);
        Task<ServiceResponse<bool>> UpdatePurposeTypeStatus(int purposeTypeId);
    }
}
