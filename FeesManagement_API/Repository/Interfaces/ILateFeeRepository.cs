// Repository/Interfaces/ILateFeeRepository.cs
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


public interface ILateFeeRepository
{
    Task<int> AddUpdateLateFee(AddUpdateLateFeeRequest request);
    Task<ServiceResponse<IEnumerable<LateFeeResponse>>> GetAllLateFee(GetAllLateFeeRequest request);
    Task<LateFeeResponse> GetLateFeeById(int lateFeeID);
    Task<int> UpdateLateFeeStatus(int lateFeeID);
    Task<IEnumerable<FeeTenureResponse>> GetFeeTenureDDL(GetFeeTenureDDLRequest request);

}
 
