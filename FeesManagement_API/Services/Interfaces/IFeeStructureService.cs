using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeStructureService
    {
        FeeStructureResponse GetFeeStructure(FeeStructureRequest request);
    }
}
