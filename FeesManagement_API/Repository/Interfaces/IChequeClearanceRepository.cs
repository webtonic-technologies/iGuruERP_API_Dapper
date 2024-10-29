using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IChequeClearanceRepository
    {
        ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request);
    }
}
