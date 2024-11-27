using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeeCollectionRepository
    {
        ServiceResponse<GetFeeResponse> GetFeesCollection(GetFeeRequest request);
        ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request);

    }
}
