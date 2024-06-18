using Student_API.DTOs;
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
        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                var data = await _studentPromotionRepository.GetStudentsForPromotion(classId, pageSize, pageNumber);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentPromotionDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId)
        {
            try
            {
                var data = await _studentPromotionRepository.PromoteStudents(studentIds, nextClassId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
