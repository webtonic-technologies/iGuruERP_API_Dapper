using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Implementations;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class AdminDepartmentServices : IAdminDepartmentServices
    {
        private readonly IAdminDepartmentRepository _adminDepartmentRepository;

        public AdminDepartmentServices(IAdminDepartmentRepository adminDepartmentRepository)
        {
            _adminDepartmentRepository = adminDepartmentRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateAdminDept(AdminDepartment request)
        {
            try
            {
                return await _adminDepartmentRepository.AddUpdateAdminDept(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAdminDepartment(int Department_id)
        {
            try
            {
                return await _adminDepartmentRepository.DeleteAdminDepartment(Department_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AdminDepartment>> GetAdminDepartmentById(int Department_id)
        {
            try
            {
                return await _adminDepartmentRepository.GetAdminDepartmentById(Department_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AdminDepartment>(false, ex.Message, new AdminDepartment(), 500);
            }
        }

        public async Task<ServiceResponse<List<AdminDepartment>>> GetAdminDepartmentList(int Institute_id)
        {
            try
            {
                return await _adminDepartmentRepository.GetAdminDepartmentList(Institute_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDepartment>>(false, ex.Message, [], 500);
            }
        }
    }
}
