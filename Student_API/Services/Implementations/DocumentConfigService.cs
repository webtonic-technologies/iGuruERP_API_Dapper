using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class DocumentConfigService : IDocumentConfigService
    {
        private readonly IDocumentConfigRepository _studentDocumentRepository;
        public DocumentConfigService(IDocumentConfigRepository documentConfigRepository)
        {
            _studentDocumentRepository = documentConfigRepository;
        }
        public async Task<ServiceResponse<int>> AddUpdateStudentDocument(List<StudentDocumentConfig> studentDocumentDto)
        {
            try
            {
                var data = await _studentDocumentRepository.AddUpdateStudentDocument(studentDocumentDto);
                return new ServiceResponse<int>(true, "Student document added/updated successfully", data.Data, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<StudentDocumentConfigDTO>> GetStudentDocumentConfigById(int documentConfigId)
        {
            try
            {
                var data = await _studentDocumentRepository.GetStudentDocumentConfigById(documentConfigId);
                if (data != null)
                    return new ServiceResponse<StudentDocumentConfigDTO>(true, "Student document config retrieved successfully", data.Data, 200);
                else
                    return new ServiceResponse<StudentDocumentConfigDTO>(false, "Student document config not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentDocumentConfigDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStudentDocument(int studentDocumentId)
        {
            try
            {
                var success = await _studentDocumentRepository.DeleteStudentDocument(studentDocumentId);
                if (success.Data)
                    return new ServiceResponse<bool>(true, "Student document deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Student document not found or couldn't be deleted", false, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentDocumentConfigDTO>>> GetAllStudentDocuments(int Institute_id ,string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                var data = await _studentDocumentRepository.GetAllStudentDocuments(Institute_id,sortColumn, sortDirection,pageSize, pageNumber);
                return new ServiceResponse<List<StudentDocumentConfigDTO>>(true, "Student documents retrieved successfully", data.Data, 200, data.TotalCount);

                //if (pageSize.HasValue && pageNumber.HasValue)
                //{
                //    return new ServiceResponse<List<StudentDocumentConfigDTO>>(true, "Student documents retrieved successfully", data.Data, 200, data.TotalCount);
                //}
                //else
                //{
                //    return new ServiceResponse<List<StudentDocumentConfigDTO>>(true, "Student documents retrieved successfully", data.Data, 200);

                //}
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDocumentConfigDTO>>(false, ex.Message, null, 500);
            }
        }

    }
}
