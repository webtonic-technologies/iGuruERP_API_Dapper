using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Services.Interfaces
{
    public interface IRefundService
    {
        string AddRefund(AddRefundRequest request);
        Task<ServiceResponse<IEnumerable<GetRefundResponse>>> GetRefund(GetRefundRequest request);

        //List<GetRefundResponse> GetRefund(GetRefundRequest request);
        ServiceResponse<string> DeleteRefund(int refundID);
        byte[] GetRefundExport(GetRefundExportRequest request);
        IEnumerable<GetStudentListResponse> GetStudentList(GetStudentListRequest request);
        IEnumerable<GetRefundPaymentModeResponse> GetRefundPaymentMode();

    }
}
