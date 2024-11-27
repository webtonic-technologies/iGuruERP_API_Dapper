using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeDiscountService
    {
        ServiceResponse<bool> ApplyDiscount(SubmitFeeDiscountRequest request);
    }
}
