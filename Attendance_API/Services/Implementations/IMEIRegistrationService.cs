using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs.ServiceResponse;

using System.Threading.Tasks;
using System.Collections.Generic;
using Attendance_API.DTOs;

namespace Student_API.Attendance_API.Implementations
{
    public class IMEIRegistrationService : IIMEIRegistrationService
    {
        private readonly IIMEIRegistrationRepo _iIMEIRegistrationRepo;

        public IMEIRegistrationService(IIMEIRegistrationRepo iIMEIRegistrationRepo)
        {
            _iIMEIRegistrationRepo = iIMEIRegistrationRepo;
        }

        public async Task<ServiceResponse<string>> AddIMEIRegistration(IMEIRegistrationModel imeiRegistrationDto)
        {
            return await _iIMEIRegistrationRepo.AddIMEIRegistration(imeiRegistrationDto);
        }

        public async Task<ServiceResponse<List<IMEIRegistrationDto>>> GetAllIMEIRegistrations()
        {
            return await _iIMEIRegistrationRepo.GetAllIMEIRegistrations();
        }
    }
}
