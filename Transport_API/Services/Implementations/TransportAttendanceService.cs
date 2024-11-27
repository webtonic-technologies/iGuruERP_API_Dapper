using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
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

        public async Task<ServiceResponse<string>> AddUpdateTransportAttendance(IEnumerable<TransportAttendanceRequest> requests)
        {
            foreach (var request in requests)
            {
                var response = await _transportAttendanceRepository.AddUpdateTransportAttendance(request); // Call the repository for each record
                if (!response.Success)  // Check if the repository method was successful
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating attendance", 400);
                }
            }

            return new ServiceResponse<string>(true, "Operation Successful", "Attendance records added/updated successfully", 200);
        }


        //public async Task<ServiceResponse<string>> AddUpdateTransportAttendance(TransportAttendanceRequest request)
        //{
        //    return await _transportAttendanceRepository.AddUpdateTransportAttendance(request);
        //}

        public async Task<ServiceResponse<IEnumerable<TransportAttendanceResponse>>> GetAllTransportAttendance(GetTransportAttendanceRequest request)
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
