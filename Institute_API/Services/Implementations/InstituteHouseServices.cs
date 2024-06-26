﻿using Institute_API.DTOs;
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

        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id)
        {
            try
            {
                return await _instituteHouseRepository.GetInstituteHouseList(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }
    }
}
