using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;

public interface IFeesReportsService
{
    Task<ServiceResponse<List<DailyPaymentSummaryResponse>>> GetDailyPaymentSummaryAsync(DailyPaymentSummaryRequest request);
    Task<ServiceResponse<List<FeePaymentSummaryResponse>>> GetFeePaymentSummaryAsync(FeePaymentSummaryRequest request);
    Task<ServiceResponse<List<PaidFeeResponse>>> GetPaidFeeReportAsync(PaidFeeRequest request);
    Task<ServiceResponse<List<ConcessionTypeResponse>>> GetConcessionTypeReportAsync(ConcessionTypeRequest request);
    Task<ServiceResponse<List<ClassWiseConcessionResponse>>> GetClassWiseConcessionReportAsync(ClassWiseConcessionRequest request);
    Task<ServiceResponse<List<DiscountSummaryResponse>>> GetDiscountSummaryReportAsync(DiscountSummaryRequest request);
    Task<List<WaiverSummaryResponse>> GetWaiverSummaryReportAsync(WaiverSummaryRequest request);

}
