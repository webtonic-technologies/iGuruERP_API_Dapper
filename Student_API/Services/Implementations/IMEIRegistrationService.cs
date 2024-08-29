using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using Student_API.DTOs.ServiceResponse;
using Student_API.DTOs.RequestDTO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Student_API.DTOs;

namespace Student_API.Services.Implementations
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
