﻿using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteDetailsRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request);
        Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id);
        Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request);
        Task<ServiceResponse<List<InstituteDetailsResponseDTO>>> GetAllInstituteDetailsList(int AcademicYear);
        Task<ServiceResponse<List<Country>>> GetCountriesAsync();
        Task<ServiceResponse<List<State>>> GetStatesByCountryIdAsync(int countryId);
        Task<ServiceResponse<List<City>>> GetCitiesByDistrictIdAsync(int districtId);
        Task<ServiceResponse<List<District>>> GetDistrictsByStateIdAsync(int stateId);
        Task<ServiceResponse<List<AcademicYearMaster>>> GetAcademicYearList(int InstituteId);
        Task<ServiceResponse<bool>> ActiveAcademicYear(string AcaInfoYearCode, int InstituteId);
        Task<ServiceResponse<string>> AddOrUpdateAcademicYear(AcademicInfo request);
    }
}
