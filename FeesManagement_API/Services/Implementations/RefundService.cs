using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Utilities;


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
        public async Task<ServiceResponse<IEnumerable<GetRefundResponse>>> GetRefund(GetRefundRequest request)
        {
            return await _refundRepository.GetRefund(request);
        }

        //public List<GetRefundResponse> GetRefund(GetRefundRequest request)
        //{
        //    return _refundRepository.GetRefund(request);
        //}

        public ServiceResponse<string> DeleteRefund(int refundID)
        {
            var result = _refundRepository.DeleteRefund(refundID);
            return new ServiceResponse<string>(true, "Refund deleted successfully.", result, 200);
        }

        public byte[] GetRefundExport(GetRefundExportRequest request)
        {
            var dataTable = _refundRepository.GetRefundExportData(request);
            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Helper method that converts DataTable to Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // Helper method that converts DataTable to CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }

        public IEnumerable<GetStudentListResponse> GetStudentList(GetStudentListRequest request)
        {
            return _refundRepository.GetStudentList(request);
        }

        public IEnumerable<GetRefundPaymentModeResponse> GetRefundPaymentMode()
        {
            return _refundRepository.GetRefundPaymentMode();
        }
    }
}


