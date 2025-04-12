using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeeCollectionRepository
    {
        ServiceResponse<GetFeeResponse> GetFeesCollection(GetFeeRequest request);
        ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request);
        ServiceResponse<bool> SubmitFeeWaiver(SubmitFeeWaiverRequest request);
        ServiceResponse<bool> ApplyDiscount(SubmitFeeDiscountRequest request);
        ServiceResponse<GetWaiverSummaryResponse> GetWaiverSummary(GetWaiverSummaryRequest request);
        ServiceResponse<GetDiscountSummaryResponse> GetDiscountSummary(GetDiscountSummaryRequest request);
        ServiceResponse<GetCollectFeeResponse> GetCollectFee(GetCollectFeeRequest request);

    }
}
