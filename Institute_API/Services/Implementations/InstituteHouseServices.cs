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

        public async Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request)
        {
            try
            {
                return await _instituteHouseRepository.AddUpdateInstituteHouse(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId)
        {
            try
            {
                return await _instituteHouseRepository.GetInstituteHouseById(instituteHouseId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }

        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id, string searchText)
        {
            try
            {
                return await _instituteHouseRepository.GetInstituteHouseList(Id, searchText);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }

        public async Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId)
        {
            try
            {
                return await _instituteHouseRepository.SoftDeleteInstituteHouse(instituteHouseId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
