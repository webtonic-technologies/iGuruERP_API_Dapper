using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
namespace Employee_API.Services.Interfaces
{
    public interface IEmployeeDashboardServices
    {
        Task<ServiceResponse<EmployeeGenderStatsResponse>> GetEmployeeGenderStats(int instituteId);
        Task<ServiceResponse<AgeGroupStatsResponse>> GetEmployeeAgeGroupStats(int instituteId);
        Task<ServiceResponse<ExperienceStatsResponse>> GetEmployeeExperienceStats(int instituteId);
        Task<ServiceResponse<DepartmentEmployeeResponse>> GetEmployeeDepartmentCounts(int instituteId);
        Task<ServiceResponse<EmployeeEventsResponse>> GetEmployeeEvents(DateTime date, int instituteId);
        Task<ServiceResponse<List<GenderCountByDesignation>>> GetGenderCountByDesignation(int instituteId);
        Task<ServiceResponse<ActiveInactiveEmployeesResponse>> GetActiveInactiveEmployees(int instituteId);
    }
}
