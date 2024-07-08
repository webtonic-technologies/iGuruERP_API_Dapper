using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Services.Interfaces
{
    public interface IAdminDesignationServices
    {
        Task<ServiceResponse<string>> AddUpdateAdminDesignation(AdminDesignation request);
        Task<ServiceResponse<string>> DeleteAdminDesignation(int Designationid);
        Task<ServiceResponse<AdminDesignation>> GetAdminDesignationById(int Designationid);
        Task<ServiceResponse<List<AdminDesignation>>> GetAdminDesignationList(GetListRequest request);
    }
}
