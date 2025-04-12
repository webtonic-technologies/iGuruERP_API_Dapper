using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IPaymentChecklistService
    {
        Task<ServiceResponse<int>> SetPaymentChecklist(List<PaymentChecklistSetRequest> requests);
        ServiceResponse<List<PaymentChecklistGetResponse>> GetPaymentChecklist(PaymentChecklistGetRequest request);

    }
}
