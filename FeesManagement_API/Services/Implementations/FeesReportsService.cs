using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;

public class FeesReportsService : IFeesReportsService
{
    private readonly IFeesReportsRepository _feesReportsRepository;

    public FeesReportsService(IFeesReportsRepository feesReportsRepository)
    {
        _feesReportsRepository = feesReportsRepository;
    }

    public async Task<ServiceResponse<List<DailyPaymentSummaryResponse>>> GetDailyPaymentSummaryAsync(DailyPaymentSummaryRequest request)
    {
        var response = await _feesReportsRepository.GetDailyPaymentSummaryAsync(request);

        return response;  // This should now return ServiceResponse<List<DailyPaymentSummaryResponse>>
    }

    public async Task<ServiceResponse<List<FeePaymentSummaryResponse>>> GetFeePaymentSummaryAsync(FeePaymentSummaryRequest request)
    {
        try
        {
            var data = await _feesReportsRepository.GetFeePaymentSummaryAsync(request);
            return new ServiceResponse<List<FeePaymentSummaryResponse>>(true, "Data retrieved successfully.", data, 200);
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<FeePaymentSummaryResponse>>(false, ex.Message, new List<FeePaymentSummaryResponse>(), 500);
        }
    }

    public async Task<ServiceResponse<List<PaidFeeResponse>>> GetPaidFeeReportAsync(PaidFeeRequest request)
    {
        try
        {
            var data = await _feesReportsRepository.GetPaidFeeReportAsync(request);
            return new ServiceResponse<List<PaidFeeResponse>>(true, "Data retrieved successfully.", data, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<PaidFeeResponse>>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse<List<ConcessionTypeResponse>>> GetConcessionTypeReportAsync(ConcessionTypeRequest request)
    {
        try
        {
            var data = await _feesReportsRepository.GetConcessionTypeReportAsync(request);
            return new ServiceResponse<List<ConcessionTypeResponse>>(true, "Data retrieved successfully.", data, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<ConcessionTypeResponse>>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse<List<ClassWiseConcessionResponse>>> GetClassWiseConcessionReportAsync(ClassWiseConcessionRequest request)
    {
        try
        {
            var data = await _feesReportsRepository.GetClassWiseConcessionReportAsync(request);
            return new ServiceResponse<List<ClassWiseConcessionResponse>>(true, "Data retrieved successfully.", data, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<ClassWiseConcessionResponse>>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse<List<DiscountSummaryResponse>>> GetDiscountSummaryReportAsync(DiscountSummaryRequest request)
    {
        var data = await _feesReportsRepository.GetDiscountSummaryAsync(request);
        return new ServiceResponse<List<DiscountSummaryResponse>>(true, "Data retrieved successfully.", data, 200);
    }

    public async Task<List<WaiverSummaryResponse>> GetWaiverSummaryReportAsync(WaiverSummaryRequest request)
    {
        return await _feesReportsRepository.GetWaiverSummaryReportAsync(request);
    }
}
