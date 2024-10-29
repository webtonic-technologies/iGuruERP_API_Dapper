using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Services.Interfaces
{
    public interface IRefundService
    {
        string AddRefund(AddRefundRequest request);
        List<GetRefundResponse> GetRefund(GetRefundRequest request);
        ServiceResponse<string> DeleteRefund(int refundID);

    }
}
