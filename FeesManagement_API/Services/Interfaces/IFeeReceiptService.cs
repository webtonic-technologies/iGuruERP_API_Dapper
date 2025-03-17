using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeReceiptService
    { 
        Task<ServiceResponse<IEnumerable<GetReceiptComponentResponse>>> GetReceiptComponents(GetReceiptComponentRequest request);
        Task<ServiceResponse<IEnumerable<GetReceiptPropertyResponse>>> GetReceiptProperties(GetReceiptPropertyRequest request);
        Task<ServiceResponse<IEnumerable<GetReceiptLayoutResponse>>> GetReceiptLayouts();
        Task<ServiceResponse<IEnumerable<GetReceiptTypeResponse>>> GetReceiptTypes();
        Task<ServiceResponse<bool>> ConfigureReceipt(ConfigureReceiptRequest request);

    }
}
