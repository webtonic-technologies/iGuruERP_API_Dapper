using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Repository.Interfaces
{
    public interface ITransportAttendanceRepository
    {
        Task<ServiceResponse<string>> AddUpdateTransportAttendance(TransportAttendanceRequest request);
        Task<ServiceResponse<IEnumerable<TransportAttendanceResponse>>> GetAllTransportAttendance(GetTransportAttendanceRequest request);
        Task<ServiceResponse<TransportAttendance>> GetTransportAttendanceById(int transportAttendanceId);
        Task<ServiceResponse<bool>> UpdateTransportAttendanceStatus(int transportAttendanceId);
    }
}
