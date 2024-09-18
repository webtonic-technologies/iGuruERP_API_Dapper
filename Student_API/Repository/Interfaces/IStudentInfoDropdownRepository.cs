using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentInfoDropdownRepository
    {
        Task<ServiceResponse<List<Gender>>> GetAllGenders();
        Task<ServiceResponse<List<Sections>>> GetAllSections(int Class_Id);
        Task<ServiceResponse<List<Religion>>> GetAllReligions();
        Task<ServiceResponse<List<Nationality>>> GetAllNationalities();
        Task<ServiceResponse<List<MotherTongue>>> GetAllMotherTongues();
        Task<ServiceResponse<List<BloodGroup>>> GetAllBloodGroups();
        Task<ServiceResponse<List<StudentType>>> GetAllStudentTypes();
        Task<ServiceResponse<List<StudentGroup>>> GetAllStudentGroups();
        Task<ServiceResponse<List<Occupation>>> GetAllOccupations();
        Task<ServiceResponse<List<ParentType>>> GetAllParentTypes();
        Task<ServiceResponse<List<Class>>> GetAllClass(int institute_id);
        Task<ServiceResponse<List<City>>> GetAllCities(int stateId);
        Task<ServiceResponse<List<State>>> GetAllStates();
        Task<ServiceResponse<List<Academic>>> GetAllAcademic();
        Task<ServiceResponse<List<ClassWithSections>>> GetAllClassesWithSections(int institute_id);
    }
}
