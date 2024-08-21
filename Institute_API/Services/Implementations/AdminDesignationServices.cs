using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Implementations;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class AdminDesignationServices : IAdminDesignationServices
    {
        private readonly IAdminDesignationRepository _adminDesignationRepository;

        public AdminDesignationServices(IAdminDesignationRepository adminDesignationRepository)
        {
            _adminDesignationRepository = adminDesignationRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateAdminDesignation(AdminDesignation request)
        {
            try
            {
                return await _adminDesignationRepository.AddUpdateAdminDesignation(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAdminDesignation(int Designationid)
        {
            try
            {
                return await _adminDesignationRepository.DeleteAdminDesignation(Designationid);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AdminDesignationResponse>> GetAdminDesignationById(int Designationid)
        {
            try
            {
                return await _adminDesignationRepository.GetAdminDesignationById(Designationid);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AdminDesignationResponse>(false, ex.Message, new AdminDesignationResponse(), 500);
            }
        }

        public async Task<ServiceResponse<List<AdminDesignationResponse>>> GetAdminDesignationList(GetListRequest request)
        {
            try
            {
                return await _adminDesignationRepository.GetAdminDesignationList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDesignationResponse>>(false, ex.Message, [], 500);
            }
        }
    }
}
