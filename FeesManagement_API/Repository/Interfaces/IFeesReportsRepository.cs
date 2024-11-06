using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;

public interface IFeesReportsRepository
{
    Task<ServiceResponse<List<DailyPaymentSummaryResponse>>> GetDailyPaymentSummaryAsync(DailyPaymentSummaryRequest request);
    Task<List<FeePaymentSummaryResponse>> GetFeePaymentSummaryAsync(FeePaymentSummaryRequest request);
    Task<List<PaidFeeResponse>> GetPaidFeeReportAsync(PaidFeeRequest request);
    Task<List<ConcessionTypeResponse>> GetConcessionTypeReportAsync(ConcessionTypeRequest request);
    Task<List<ClassWiseConcessionResponse>> GetClassWiseConcessionReportAsync(ClassWiseConcessionRequest request);
    Task<List<DiscountSummaryResponse>> GetDiscountSummaryAsync(DiscountSummaryRequest request);
    Task<List<WaiverSummaryResponse>> GetWaiverSummaryReportAsync(WaiverSummaryRequest request);


}
