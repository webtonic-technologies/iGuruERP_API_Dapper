using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Repository.Interfaces
{
    public interface IAdminDesignationRepository
    {
        Task<ServiceResponse<string>> AddUpdateAdminDesignation(AdminDesignation request);
        Task<ServiceResponse<string>> DeleteAdminDesignation(int Designationid);
        Task<ServiceResponse<AdminDesignation>> GetAdminDesignationById(int Designationid);
        Task<ServiceResponse<List<AdminDesignation>>> GetAdminDesignationList(int Institute_id);
    }
}
