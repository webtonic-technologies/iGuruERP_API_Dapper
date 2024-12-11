using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeStructureService
    {
        FeeStructureResponse GetFeeStructure(FeeStructureRequest request);
        Task<ServiceResponse<byte[]>> GetFeeStructureExcel(FeeStructureRequest request);
        Task<ServiceResponse<byte[]>> GetFeeStructureCSV(FeeStructureRequest request);

    }
}
