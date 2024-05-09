using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentInfoDropdownRepository
    {
        Task<ServiceResponse<List<Gender>>> GetAllGenders();
        Task<ServiceResponse<List<Sections>>> GetAllSections();
        Task<ServiceResponse<List<Religion>>> GetAllReligions();
        Task<ServiceResponse<List<Nationality>>> GetAllNationalities();
        Task<ServiceResponse<List<MotherTongue>>> GetAllMotherTongues();
        Task<ServiceResponse<List<BloodGroup>>> GetAllBloodGroups();
        Task<ServiceResponse<List<StudentType>>> GetAllStudentTypes();
        Task<ServiceResponse<List<StudentGroup>>> GetAllStudentGroups();
        Task<ServiceResponse<List<Occupation>>> GetAllOccupations();
        Task<ServiceResponse<List<ParentType>>> GetAllParentTypes();
    }
}
