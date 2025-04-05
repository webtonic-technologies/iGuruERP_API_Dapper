using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Data;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IChequeTrackingRepository
    {
        ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request);
        ServiceResponse<List<GetChequeTrackingBouncedResponse>> GetChequeTrackingBounced(GetChequeTrackingBouncedRequest request);
        ServiceResponse<List<GetChequeTrackingClearedResponse>> GetChequeTrackingCleared(GetChequeTrackingClearedRequest request);

        ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus();
        DataTable GetChequeTrackingExportData(ChequeTrackingExportRequest request); // Add this
        DataTable GetChequeTrackingBouncedExportData(ChequeTrackingExportBouncedRequest request); // Add this
        DataTable GetChequeTrackingClearedExportData(ChequeTrackingExportClearedRequest request); // Add this

        ServiceResponse<bool> AddChequeBounce(SubmitChequeBounceRequest request);
        ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request);

    }
}
