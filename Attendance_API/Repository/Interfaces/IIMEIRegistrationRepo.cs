using  System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.DTOs;


namespace Attendance_API.Repository.Interfaces
{
    public interface IIMEIRegistrationRepo
    {
        Task<ServiceResponse<string>> AddIMEIRegistration(IMEIRegistrationModel imeiRegistrationDto);
        Task<ServiceResponse<List<IMEIRegistrationDto>>> GetAllIMEIRegistrations();
    }
}
