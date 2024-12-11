using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IChequeTrackingService
    {
        ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request);
        ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus();
        byte[] GetChequeTrackingExport(ChequeTrackingExportRequest request); // Add this

    }
}
