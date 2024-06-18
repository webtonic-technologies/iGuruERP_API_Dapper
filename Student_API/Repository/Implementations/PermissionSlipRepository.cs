using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;
using System.Drawing.Printing;

namespace Student_API.Repository.Implementations
{
    public class PermissionSlipRepository : IPermissionSlipRepository
    {
        private readonly IDbConnection _dbConnection;

        public PermissionSlipRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                const int MaxPageSize = int.MaxValue;

                // Set default values if pageSize or pageNumber are not provided
                int actualPageSize = pageSize ?? MaxPageSize;
                int actualPageNumber = pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                string sql = @"
            -- Drop the temporary table if it exists
            IF OBJECT_ID('tempdb..#PermissionSlipTempTable') IS NOT NULL DROP TABLE #PermissionSlipTempTable;

            -- Create and populate the temporary table in one step
            SELECT 
                ps.PermissionSlip_Id,
                s.student_id AS Student_Id,
                s.first_name + ' ' + s.last_name AS StudentName,
                s.admission_number AS Admission_Number,
                c.ClassName,
                sec.SectionName,
                g.GenderName,
                p.first_name + ' ' + p.last_name AS ParentName,
                ps.RequestedDateTime,
                ps.Reason,
                ps.IsApproved,
                ps.ModifiedDate
            INTO #PermissionSlipTempTable
            FROM tbl_PermissionSlip ps
            JOIN tbl_StudentMaster s ON ps.Student_Id = s.student_id
            JOIN tbl_StudentParentsInfo p ON ps.Student_Parent_Info_id = p.student_parent_info_id
            JOIN tbl_CourseClass c ON s.class_id = c.CourseClass_id
            JOIN tbl_CourseClassSection sec ON s.section_id = sec.CourseClassSection_id
            JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            WHERE s.class_id = @ClassId
              AND s.section_id = @SectionId;

            -- Select paginated data from the temporary table
            SELECT * 
            FROM #PermissionSlipTempTable
            ORDER BY ModifiedDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            -- Get the total count of records
            SELECT COUNT(1) 
            FROM #PermissionSlipTempTable;

            -- Drop the temporary table
            DROP TABLE IF EXISTS #PermissionSlipTempTable;";

                // Execute the query with pagination
                using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { ClassId = classId, SectionId = sectionId, Offset = offset, PageSize = actualPageSize }))
                {
                    var permissionSlips = multi.Read<PermissionSlipDTO>().ToList();
                    int? totalRecords = (pageSize.HasValue && pageNumber.HasValue) ? multi.ReadSingle<int>() : null;

                    if (permissionSlips.Any())
                    {
                        return new ServiceResponse<List<PermissionSlipDTO>>(true, "Data retrieved successfully", permissionSlips, 200, totalRecords);
                    }
                    else
                    {
                        return new ServiceResponse<List<PermissionSlipDTO>>(false, "No permission slips found", null, 404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PermissionSlipDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved)
        {
            try
            {
                string query = @"
            UPDATE tbl_PermissionSlip 
            SET IsApproved = @IsApproved , ModifiedDate = GETDATE()
            WHERE PermissionSlip_Id = @PermissionSlipId";

                await _dbConnection.ExecuteAsync(query, new { PermissionSlipId = permissionSlipId, IsApproved = isApproved });

                return new ServiceResponse<string>(true, "Permission slip status updated successfully", null, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int classId,int sectionId,DateTime? startDate,DateTime? endDate,bool isApproved,int? pageNumber = null,int? pageSize = null)
        {
            try
            {
                // Define the maximum page size if pagination is not specified
                const int MaxPageSize = int.MaxValue;

                // Set default pagination values if not provided
                int actualPageSize = pageSize ?? MaxPageSize;
                int actualPageNumber = pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                // SQL query with filters and pagination
                string sql = @"
            IF OBJECT_ID('tempdb..#PermissionSlipTempTable') IS NOT NULL DROP TABLE #PermissionSlipTempTable;

            SELECT 
                ps.PermissionSlip_Id,
                s.student_id AS Student_Id,
                s.admission_number AS Admission_Number,
                s.first_name + ' ' + s.last_name AS StudentName,
                c.class_course  AS ClassName,
                sec.SectionName,
                ps.RequestedDateTime AS ApprovalDate,
                pt.ParentTypeName AS ParentType,
                p.first_name + ' ' + p.last_name AS ParentName,
                ps.Reason AS Remark
            INTO #PermissionSlipTempTable
            FROM tbl_PermissionSlip ps
            JOIN tbl_StudentMaster s ON ps.Student_Id = s.student_id
            JOIN tbl_Student_Parents_Info p ON ps.Student_Parent_Info_id = p.student_parent_info_id
            JOIN tbl_CourseClass c ON s.class_id = c.CourseClass_id
            JOIN tbl_CourseClassSection sec ON s.section_id = sec.CourseClassSection_id
            JOIN tbl_ParentType pt ON p.parent_type_id = pt.ParentTypeId
            WHERE s.class_id = @ClassId
              AND s.section_id = @SectionId
              AND ps.IsApproved = @IsApproved
              AND (@StartDate IS NULL OR ps.ModifiedDate >= @StartDate)
              AND (@EndDate IS NULL OR ps.ModifiedDate <= @EndDate);

            SELECT * 
            FROM #PermissionSlipTempTable
            ORDER BY ApprovalDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(1) 
            FROM #PermissionSlipTempTable;

            DROP TABLE IF EXISTS #PermissionSlipTempTable;";

                // Execute the query with pagination
                using (var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    ClassId = classId,
                    SectionId = sectionId,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsApproved = isApproved,
                    Offset = offset,
                    PageSize = actualPageSize
                }))
                {
                    var permissionSlips = multi.Read<PermissionSlipDTO>().ToList();
                    int? totalRecords = (pageSize.HasValue && pageNumber.HasValue) ? multi.ReadSingle<int>() : null;

                    if (permissionSlips.Any())
                    {
                        return new ServiceResponse<List<PermissionSlipDTO>>(true, "Data retrieved successfully", permissionSlips, 200, totalRecords);
                    }
                    else
                    {
                        return new ServiceResponse<List<PermissionSlipDTO>>(false, "No permission slips found", null, 404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PermissionSlipDTO>>(false, ex.Message, null, 500);
            }
        }

    }
}

