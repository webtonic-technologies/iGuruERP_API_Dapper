using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
namespace Attendance_API.Services.Interfaces
{
    public interface IIMEIRegistrationService
    {
        Task<ServiceResponse<string>> AddIMEIRegistration(IMEIRegistrationModel imeiRegistrationDto);
        Task<ServiceResponse<List<IMEIRegistrationDto>>> GetAllIMEIRegistrations();
    }
}
