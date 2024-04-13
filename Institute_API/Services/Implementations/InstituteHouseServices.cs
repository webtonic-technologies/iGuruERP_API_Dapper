using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Implementations;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class InstituteHouseServices : IInstituteHouseServices
    {

        private readonly IInstituteHouseRepository _instituteHouseRepository;

        public InstituteHouseServices(IInstituteHouseRepository instituteHouseRepository)
        {
            _instituteHouseRepository = instituteHouseRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateInstituteHouse(InstituteHouseDTO request)
        {
            try
            {
                return await _instituteHouseRepository.AddUpdateInstituteHouse(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int Id)
        {
            try
            {
                return await _instituteHouseRepository.GetInstituteHouseById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id)
        {
            try
            {
                return await _instituteHouseRepository.GetInstituteHouseLogoById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
    }
}
