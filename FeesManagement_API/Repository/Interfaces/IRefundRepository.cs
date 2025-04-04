using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Data;


namespace FeesManagement_API.Repository.Interfaces
{
    public interface IRefundRepository
    {
        string AddRefund(AddRefundRequest request);

        Task<ServiceResponse<IEnumerable<GetRefundResponse>>> GetRefund(GetRefundRequest request);

        //List<GetRefundResponse> GetRefund(GetRefundRequest request);
        string DeleteRefund(int refundID);
        DataTable GetRefundExportData(GetRefundExportRequest request);
        IEnumerable<GetStudentListResponse> GetStudentList(GetStudentListRequest request);
        IEnumerable<GetRefundPaymentModeResponse> GetRefundPaymentMode();

    }
}
