// Repository/Interfaces/ILateFeeRepository.cs
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

public interface ILateFeeRepository
{
    Task<int> AddUpdateLateFee(AddUpdateLateFeeRequest request);
    Task<IEnumerable<LateFeeResponse>> GetAllLateFee(GetAllLateFeeRequest request);
    Task<LateFeeResponse> GetLateFeeById(int lateFeeID);
    Task<int> UpdateLateFeeStatus(int lateFeeID);
}
