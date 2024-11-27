using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.Requests;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeeStructureRepository
    {
        FeeStructureResponse GetFeeStructure(FeeStructureRequest request);
        Task<byte[]> GetFeeStructureExcel(FeeStructureRequest request);

    }
}
