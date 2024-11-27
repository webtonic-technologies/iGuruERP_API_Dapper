using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly IDbConnection _connection;

        public TemplateRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<int>> AddUpdateTemplate(TemplateDTO templateDto)
        {
            try
            {
                string query;
                
                if (templateDto.Template_Type_Id > 0)
                {
                    query = @"
                    UPDATE [dbo].[tbl_TemplateType]
                    SET Template_Name = @Template_Name,
                        UserId = @UserId,
                        CreatedDate = GETDATE()
                    WHERE Template_Type_Id = @Template_Type_Id";
                }
                else
                {
                    query = @"
                    INSERT INTO [dbo].[tbl_TemplateType] (Template_Name, UserId)
                    VALUES (@Template_Name, @UserId);
                    SELECT SCOPE_IDENTITY();";
                }

                int id = await _connection.ExecuteScalarAsync<int>(query, templateDto);
                return new ServiceResponse<int>(true, "Template saved successfully", id, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<TemplateDTO>> GetTemplateById(int templateId)
        {
            try
            {
                string query = "SELECT * FROM [dbo].[tbl_TemplateType] WHERE Template_Type_Id = @Template_Type_Id AND isDelete = 0";
                var template = await _connection.QueryFirstOrDefaultAsync<TemplateDTO>(query, new { Template_Type_Id = templateId });

                if (template != null)
                    return new ServiceResponse<TemplateDTO>(true, "Template retrieved successfully", template, 200);
                else
                    return new ServiceResponse<TemplateDTO>(false, "Template not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TemplateDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTemplate(int templateId)
        {
            try
            {
                string query = "update  [dbo].[tbl_TemplateType] SET isDelete = 1 WHERE Template_Type_Id = @Template_Type_Id";
                int rowsAffected = await _connection.ExecuteAsync(query, new { Template_Type_Id = templateId });

                if (rowsAffected > 0)
                    return new ServiceResponse<bool>(true, "Template deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Template not found or couldn't be deleted", false, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<TemplateResponseDTO>>> GetAllTemplates(int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                string queryAll = "SELECT Template_Type_Id,Template_Name,UserId,FORMAT([CreatedDate], 'dd-MM-yyyy hh:mm tt') AS CreatedDate FROM [dbo].[tbl_TemplateType] WHERE isDelete = 0";
                string queryCount = "SELECT COUNT(*) FROM [dbo].[tbl_TemplateType] WHERE isDelete = 0";

                List<TemplateResponseDTO> templates;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)   
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;
                    string queryPaginated = $@"
                        {queryAll}
                        ORDER BY Template_Type_Id
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;

                        {queryCount}";

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize }))
                    {
                        templates = multi.Read<TemplateResponseDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }

                    return new ServiceResponse<List<TemplateResponseDTO>>(true, "Templates retrieved successfully", templates, 200, totalRecords);
                }
                else
                {
                    templates = (await _connection.QueryAsync<TemplateResponseDTO>(queryAll)).ToList();
                    return new ServiceResponse<List<TemplateResponseDTO>>(true, "All templates retrieved successfully", templates, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TemplateResponseDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}
