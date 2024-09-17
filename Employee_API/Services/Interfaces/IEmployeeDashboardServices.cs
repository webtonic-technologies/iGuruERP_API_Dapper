using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
namespace Employee_API.Services.Interfaces
{
    public interface IEmployeeDashboardServices
    {
        Task<ServiceResponse<EmployeeGenderStatsResponse>> GetEmployeeGenderStats(int instituteId);
        Task<ServiceResponse<List<AgeGroupStatsResponse>>> GetEmployeeAgeGroupStats(int instituteId);
        Task<ServiceResponse<List<ExperienceRangeResponse>>> GetEmployeeExperienceStats(int instituteId);
        Task<ServiceResponse<DepartmentEmployeeResponse>> GetEmployeeDepartmentCounts(int instituteId);
        Task<ServiceResponse<EmployeeEventsResponse>> GetEmployeeEvents(DateTime date, int instituteId);
        Task<ServiceResponse<List<GenderCountByDesignation>>> GetGenderCountByDesignation(int instituteId);
        Task<ServiceResponse<ActiveInactiveEmployeesResponse>> GetActiveInactiveEmployees(int instituteId);
        Task<ServiceResponse<AppUserNonAppUserResponse>> GetAppUsersNonAppUsersEmployees(int instituteId);
    }
}
