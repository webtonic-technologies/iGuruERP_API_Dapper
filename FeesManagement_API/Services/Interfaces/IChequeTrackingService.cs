using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IChequeTrackingService
    {
        ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request);
        ServiceResponse<List<GetChequeTrackingBouncedResponse>> GetChequeTrackingBounced(GetChequeTrackingBouncedRequest request);
        ServiceResponse<List<GetChequeTrackingClearedResponse>> GetChequeTrackingCleared(GetChequeTrackingClearedRequest request);

        ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus();
        byte[] GetChequeTrackingExport(ChequeTrackingExportRequest request); // Add this
        byte[] GetChequeTrackingBouncedExport(ChequeTrackingExportBouncedRequest request); // Add this
        byte[] GetChequeTrackingClearedExport(ChequeTrackingExportClearedRequest request); // Add this 
        ServiceResponse<bool> AddChequeBounce(SubmitChequeBounceRequest request);
        ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request);

    }
}
