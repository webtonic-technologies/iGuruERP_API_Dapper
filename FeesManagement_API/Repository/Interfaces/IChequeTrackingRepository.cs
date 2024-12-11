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
        ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus();
        DataTable GetChequeTrackingExportData(ChequeTrackingExportRequest request); // Add this

    }
}
