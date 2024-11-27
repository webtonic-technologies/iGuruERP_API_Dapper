using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Services.Implementations
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;

        public RefundService(IRefundRepository refundRepository)
        {
            _refundRepository = refundRepository;
        }

        public string AddRefund(AddRefundRequest request)
        {
            return _refundRepository.AddRefund(request);
        }

        public List<GetRefundResponse> GetRefund(GetRefundRequest request)
        {
            return _refundRepository.GetRefund(request);
        }

        public ServiceResponse<string> DeleteRefund(int refundID)
        {
            var result = _refundRepository.DeleteRefund(refundID);
            return new ServiceResponse<string>(true, "Refund deleted successfully.", result, 200);
        }
    }
}
