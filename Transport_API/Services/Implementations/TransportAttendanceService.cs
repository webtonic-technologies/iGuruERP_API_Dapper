using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class TransportAttendanceService : ITransportAttendanceService
    {
        private readonly ITransportAttendanceRepository _transportAttendanceRepository;

        public TransportAttendanceService(ITransportAttendanceRepository transportAttendanceRepository)
        {
            _transportAttendanceRepository = transportAttendanceRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateTransportAttendance(TransportAttendance transportAttendance)
        {
            return await _transportAttendanceRepository.AddUpdateTransportAttendance(transportAttendance);
        }

        public async Task<ServiceResponse<IEnumerable<TransportAttendance>>> GetAllTransportAttendance(GetTransportAttendanceRequest request)
        {
            return await _transportAttendanceRepository.GetAllTransportAttendance(request);
        }

        public async Task<ServiceResponse<TransportAttendance>> GetTransportAttendanceById(int transportAttendanceId)
        {
            return await _transportAttendanceRepository.GetTransportAttendanceById(transportAttendanceId);
        }

        public async Task<ServiceResponse<bool>> UpdateTransportAttendanceStatus(int transportAttendanceId)
        {
            return await _transportAttendanceRepository.UpdateTransportAttendanceStatus(transportAttendanceId);
        }
    }
}
