using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing.Printing;

namespace Student_API.Services.Implementations
{
    public class StudentPromotionService : IStudentPromotionService
    {
        private readonly IStudentPromotionRepository _studentPromotionRepository;
        public StudentPromotionService(IStudentPromotionRepository studentPromotionRepository)
        {
            _studentPromotionRepository = studentPromotionRepository;
        }
        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(GetStudentsForPromotionParam obj)
        {
            try
            {
                var data = await _studentPromotionRepository.GetStudentsForPromotion(obj.classId, obj.sortField, obj.sortDirection, obj.pageSize, obj.pageNumber);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentPromotionDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId, int sectionId)
        {
            try
            {
                var data = await _studentPromotionRepository.PromoteStudents(studentIds, nextClassId,sectionId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> PromoteClasses(ClassPromotionDTO classPromotionDTO)
        {
            try
            {
                var data = await _studentPromotionRepository.PromoteClasses(classPromotionDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<ClassPromotionLogDTO>>> GetClassPromotionLog(GetClassPromotionLogParam obj)
        {
            try
            {
                var data = await _studentPromotionRepository.GetClassPromotionLog(obj);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassPromotionLogDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}
