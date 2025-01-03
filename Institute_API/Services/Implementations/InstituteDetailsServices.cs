﻿using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class InstituteDetailsServices : IInstituteDetailsServices
    {
        private readonly IInstituteDetailsRepository _instituteDetailsRepository;

        public InstituteDetailsServices(IInstituteDetailsRepository instituteDetailsRepository)
        {
            _instituteDetailsRepository = instituteDetailsRepository;
        }

        public async Task<ServiceResponse<bool>> ActiveAcademicYear(string AcaInfoYearCode, int InstituteId)
        {
            return await _instituteDetailsRepository.ActiveAcademicYear(AcaInfoYearCode, InstituteId);
        }

        public async Task<ServiceResponse<string>> AddOrUpdateAcademicYear(AcademicInfo request)
        {
            return await _instituteDetailsRepository.AddOrUpdateAcademicYear(request);
        }

        public async Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request)
        {
            try
            {
                return await _instituteDetailsRepository.AddUpdateInstititeDetails(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request)
        {
            try
            {
                return await _instituteDetailsRepository.DeleteImage(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<AcademicYearMaster>>> GetAcademicYearList(int InstituteId)
        {
            return await _instituteDetailsRepository.GetAcademicYearList(InstituteId);
        }

        public async Task<ServiceResponse<List<InstituteDetailsResponseDTO>>> GetAllInstituteDetailsList(int AcademicYear)
        {
            try
            {
                return await _instituteDetailsRepository.GetAllInstituteDetailsList(AcademicYear);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InstituteDetailsResponseDTO>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<City>>> GetCitiesByDistrictIdAsync(int districtId)
        {
            try
            {
                return await _instituteDetailsRepository.GetCitiesByDistrictIdAsync(districtId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<City>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<Country>>> GetCountriesAsync()
        {

            try
            {
                return await _instituteDetailsRepository.GetCountriesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Country>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<District>>> GetDistrictsByStateIdAsync(int stateId)
        {
            try
            {
                return await _instituteDetailsRepository.GetDistrictsByStateIdAsync(stateId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<District>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id)
        {
            try
            {
                return await _instituteDetailsRepository.GetInstituteDetailsById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteDetailsResponseDTO>(false, ex.Message, new InstituteDetailsResponseDTO(), 500);
            }
        }

        public async Task<ServiceResponse<List<State>>> GetStatesByCountryIdAsync(int countryId)
        {
            try
            {
                return await _instituteDetailsRepository.GetStatesByCountryIdAsync(countryId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<State>>(false, ex.Message, [], 500);
            }
        }
    }
}
