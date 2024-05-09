using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Implementations;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentInfoDropdownService : IStudentInfoDropdownService
    {
        private readonly IStudentInfoDropdownRepository _studentInfoDropdownRepository;

        public StudentInfoDropdownService(IStudentInfoDropdownRepository studentInfoDropdownRepository)
        {
            _studentInfoDropdownRepository = studentInfoDropdownRepository;
        }
        public async Task<ServiceResponse<List<Gender>>> GetAllGenders()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllGenders();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Gender>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Sections>>> GetAllSections()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllSections();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Sections>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Religion>>> GetAllReligions()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllReligions();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Religion>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Nationality>>> GetAllNationalities()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllNationalities();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Nationality>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<MotherTongue>>> GetAllMotherTongues()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllMotherTongues();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MotherTongue>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<BloodGroup>>> GetAllBloodGroups()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllBloodGroups();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<BloodGroup>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentType>>> GetAllStudentTypes()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllStudentTypes();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentType>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentGroup>>> GetAllStudentGroups()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllStudentGroups();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentGroup>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Occupation>>> GetAllOccupations()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllOccupations();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Occupation>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<ParentType>>> GetAllParentTypes()
        {
            try
            {
                var data = await _studentInfoDropdownRepository.GetAllParentTypes();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ParentType>>(false, ex.Message, null, 500);
            }
        }
    }
}
