using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Implementations;
using Employee_API.Repository.Interfaces;
using Employee_API.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Employee_API.Services.Implementations
{
    public class EmployeeDashboardServices : IEmployeeDashboardServices
    {
        private readonly IEmployeeDashboardRepository _employeeDashboardRepository;

        public EmployeeDashboardServices(IEmployeeDashboardRepository employeeDashboardRepository)
        {
            _employeeDashboardRepository = employeeDashboardRepository;
        }
        public async Task<ServiceResponse<ActiveInactiveEmployeesResponse>> GetActiveInactiveEmployees(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetActiveInactiveEmployees(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ActiveInactiveEmployeesResponse>(false, ex.Message, new ActiveInactiveEmployeesResponse(), 500);
            }
        }

        public async Task<ServiceResponse<AgeGroupStatsResponse>> GetEmployeeAgeGroupStats(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetEmployeeAgeGroupStats(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AgeGroupStatsResponse>(false, ex.Message, new AgeGroupStatsResponse(), 500);
            }
        }

        public async Task<ServiceResponse<DepartmentEmployeeResponse>> GetEmployeeDepartmentCounts(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetEmployeeDepartmentCounts(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DepartmentEmployeeResponse>(false, ex.Message, new DepartmentEmployeeResponse(), 500);
            }
        }

        public async Task<ServiceResponse<EmployeeEventsResponse>> GetEmployeeEvents(DateTime date, int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetEmployeeEvents(date, instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeEventsResponse>(false, ex.Message, new EmployeeEventsResponse(), 500);
            }
        }

        public async Task<ServiceResponse<ExperienceStatsResponse>> GetEmployeeExperienceStats(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetEmployeeExperienceStats(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ExperienceStatsResponse>(false, ex.Message, new ExperienceStatsResponse(), 500);
            }
        }

        public async Task<ServiceResponse<EmployeeGenderStatsResponse>> GetEmployeeGenderStats(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetEmployeeGenderStats(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeGenderStatsResponse>(false, ex.Message, new EmployeeGenderStatsResponse(), 500);
            }
        }

        public async Task<ServiceResponse<List<GenderCountByDesignation>>> GetGenderCountByDesignation(int instituteId)
        {
            try
            {
                return await _employeeDashboardRepository.GetGenderCountByDesignation(instituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GenderCountByDesignation>>(false, ex.Message, [], 500);
            }
        }
    }
}
