using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class PaymentChecklistService : IPaymentChecklistService
    {
        private readonly IPaymentChecklistRepository _paymentChecklistRepository;

        public PaymentChecklistService(IPaymentChecklistRepository paymentChecklistRepository)
        {
            _paymentChecklistRepository = paymentChecklistRepository;
        }

        public async Task<ServiceResponse<int>> SetPaymentChecklist(List<PaymentChecklistSetRequest> requests)
        {
            var rowsAffected = await _paymentChecklistRepository.SetPaymentChecklist(requests);
            if (rowsAffected > 0)
            {
                return new ServiceResponse<int>(true, "Payment checklist set successfully", rowsAffected, 200);
            }
            return new ServiceResponse<int>(false, "Failed to set payment checklist", rowsAffected, 400);
        }

        public ServiceResponse<List<PaymentChecklistGetResponse>> GetPaymentChecklist(PaymentChecklistGetRequest request)
        {
            return _paymentChecklistRepository.GetPaymentChecklist(request);
        }
    }
}
