using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.Utilities;
using FeesManagement_API.Repository.Implementations;

namespace FeesManagement_API.Services.Implementations
{
    public class ChequeTrackingService : IChequeTrackingService
    {
        private readonly IChequeTrackingRepository _chequeTrackingRepository;

        public ChequeTrackingService(IChequeTrackingRepository chequeTrackingRepository)
        {
            _chequeTrackingRepository = chequeTrackingRepository;
        }

        public ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request)
        {
            return _chequeTrackingRepository.GetChequeTracking(request);
        }
        public ServiceResponse<List<GetChequeTrackingBouncedResponse>> GetChequeTrackingBounced(GetChequeTrackingBouncedRequest request)
        {
            return _chequeTrackingRepository.GetChequeTrackingBounced(request);
        }

        public ServiceResponse<List<GetChequeTrackingClearedResponse>> GetChequeTrackingCleared(GetChequeTrackingClearedRequest request)
        {
            return _chequeTrackingRepository.GetChequeTrackingCleared(request);
        }


        public ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus()
        {
            return _chequeTrackingRepository.GetChequeTrackingStatus();
        }

        public byte[] GetChequeTrackingExport(ChequeTrackingExportRequest request)
        {
            var dataTable = _chequeTrackingRepository.GetChequeTrackingExportData(request);

            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }
        public byte[] GetChequeTrackingBouncedExport(ChequeTrackingExportBouncedRequest request)
        {
            var dataTable = _chequeTrackingRepository.GetChequeTrackingBouncedExportData(request);

            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }
        public byte[] GetChequeTrackingClearedExport(ChequeTrackingExportClearedRequest request)
        {
            var dataTable = _chequeTrackingRepository.GetChequeTrackingClearedExportData(request);

            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }

        public ServiceResponse<bool> AddChequeBounce(SubmitChequeBounceRequest request)
        {
            return _chequeTrackingRepository.AddChequeBounce(request);
        }
        public ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request)
        {
            return _chequeTrackingRepository.AddChequeClearance(request);
        }
    }
}
