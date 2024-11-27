using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IRefundRepository
    {
        string AddRefund(AddRefundRequest request);
        List<GetRefundResponse> GetRefund(GetRefundRequest request);
        string DeleteRefund(int refundID);

    }
}
