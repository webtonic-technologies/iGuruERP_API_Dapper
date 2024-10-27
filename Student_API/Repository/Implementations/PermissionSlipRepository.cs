using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Helper;
using Student_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;
using static Student_API.Models.Enums.Enums;

namespace Student_API.Repository.Implementations
{
    public class PermissionSlipRepository : IPermissionSlipRepository
    {
        private readonly IDbConnection _dbConnection;

        public PermissionSlipRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int Institute_id, int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
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
                c.class_name AS ClassName,
                sec.Section_name AS SectionName,
                g.Gender_Type AS GenderName,
                p.first_name + ' ' + p.last_name AS ParentName,
                FORMAT(ps.RequestedDateTime, 'dd-MM-yyyy hh:mm tt') AS RequestedDateTime ,
                ps.Reason,
                ps.Status,
               FORMAT(ps.ModifiedDate, 'dd-MM-yyyy hh:mm tt') AS ModifiedDate
            INTO #PermissionSlipTempTable
            FROM tbl_PermissionSlip ps
            INNER JOIN tbl_StudentMaster s ON ps.Student_Id = s.student_id
           LEFT JOIN tbl_StudentParentsInfo p ON ps.Student_Parent_Info_id = p.student_parent_info_id
            LEFT JOIN tbl_Class c ON s.class_id = c.class_id
            LEFT JOIN tbl_section sec ON s.section_id = sec.section_id
            LEFT JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            WHERE ps.Institute_id = @Institute_id AND (s.class_id = @ClassId OR @ClassId =0)
              AND (s.section_id = @SectionId OR @SectionId = 0)AND Status = 1;

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
                using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { ClassId = classId, SectionId = sectionId, Offset = offset, PageSize = actualPageSize, Institute_id = Institute_id }))
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
                int status = (int)Permission_Status.Pending;
                status = isApproved ? (int)Permission_Status.Approved : (int)Permission_Status.Rejected;
                string query = @"
            UPDATE tbl_PermissionSlip 
            SET Status = @Status , ModifiedDate = GETDATE()
            WHERE PermissionSlip_Id = @PermissionSlipId";

                await _dbConnection.ExecuteAsync(query, new { PermissionSlipId = permissionSlipId, Status = status });

                return new ServiceResponse<string>(true, "Permission slip status updated successfully", null, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int Institute_id, int classId, int sectionId, string startDate, string endDate, bool isApproved, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                int status = (int)Permission_Status.Pending;
                status = isApproved ? (int)Permission_Status.Approved : (int)Permission_Status.Rejected;
                // Define the maximum page size if pagination is not specified
                const int MaxPageSize = int.MaxValue;

                // Set default pagination values if not provided
                int actualPageSize = pageSize ?? MaxPageSize;
                int actualPageNumber = pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                startDate = DateTimeHelper.ConvertToDateTime(startDate, "dd-MM-yyyy").ToString("yyyy-MM-dd");
                endDate = DateTimeHelper.ConvertToDateTime(endDate, "dd-MM-yyyy").ToString("yyyy-MM-dd");


                // SQL query with filters and pagination
                string sql = @"
            IF OBJECT_ID('tempdb..#PermissionSlipTempTable') IS NOT NULL DROP TABLE #PermissionSlipTempTable;

            SELECT 
                ps.PermissionSlip_Id,
                s.student_id AS Student_Id,
                s.admission_number AS Admission_Number,
                s.first_name + ' ' + s.last_name AS StudentName,
                c.class_name AS ClassName,
                sec.Section_name AS SectionName,    
                FORMAT(ps.RequestedDateTime, 'dd-MM-yyyy hh:mm tt')  AS RequestedDateTime,
                pt.parent_type AS ParentType,
                p.first_name + ' ' + p.last_name AS ParentName,
                ps.Reason AS Remark,
                Status,
                ps.Reason,
                g.Gender_Type AS GenderName,
   FORMAT(ps.ModifiedDate, 'dd-MM-yyyy hh:mm tt') AS ModifiedDate
            INTO #PermissionSlipTempTable
            FROM tbl_PermissionSlip ps
            JOIN tbl_StudentMaster s ON ps.Student_Id = s.student_id
            JOIN tbl_StudentParentsInfo p ON ps.Student_Parent_Info_id = p.student_parent_info_id
            JOIN tbl_Class c ON s.class_id = c.class_id
            JOIN tbl_section sec ON s.section_id = sec.section_id
            JOIN tbl_ParentType pt ON p.parent_type_id = pt.parent_type_id
            LEFT JOIN tbl_Gender g ON s.gender_id = g.Gender_Id
            WHERE ps.Institute_id = @Institute_id
              AND (s.class_id = @ClassId OR  @ClassId=0)
              AND (s.section_id = @SectionId OR  @SectionId =0)
              AND ps.Status = @Status
              AND (@StartDate IS NULL OR CAST(ps.RequestedDateTime AS DATE) >= CAST(@StartDate AS DATE))
AND (@EndDate IS NULL OR CAST(ps.RequestedDateTime AS DATE) <= CAST(@EndDate AS DATE));


            SELECT * 
            FROM #PermissionSlipTempTable
            ORDER BY RequestedDateTime DESC
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
                    Status = status,
                    Offset = offset,
                    PageSize = actualPageSize,
                    Institute_id = Institute_id
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

        public async Task<ServiceResponse<SinglePermissionSlipDTO>> GetPermissionSlipById(int permissionSlipId, int Institute_id)
        {
            try
            {
                string sql = @"
                SELECT 
                    ps.PermissionSlip_Id,
                    s.student_id AS Student_Id,
                    s.first_name + ' ' + s.last_name AS StudentName,
                    s.admission_number AS Admission_Number,
                    c.class_name AS ClassName,
                    sec.Section_name AS SectionName,
                    FORMAT(ps.RequestedDateTime, 'dd-MM-yyyy hh:mm tt') AS RequestedDateTime,
                    ps.Reason,
                    ps.Status,
                     FORMAT(ps.ModifiedDate, 'dd-MM-yyyy hh:mm tt') AS ModifiedDate,
                    ps.Qr_Code,
                    p.first_name + ' ' + p.last_name AS ParentName,
                    g.Gender_Type AS GenderName,
                    p.File_Name AS Parent_File
                FROM tbl_PermissionSlip ps
                JOIN tbl_StudentMaster s ON ps.Student_Id = s.student_id
                JOIN tbl_StudentParentsInfo p ON ps.Student_Parent_Info_id = p.student_parent_info_id
                JOIN tbl_Class c ON s.class_id = c.class_id
                JOIN tbl_section sec ON s.section_id = sec.section_id
                JOIN tbl_Gender g ON s.Gender_id = g.Gender_id
                WHERE ps.PermissionSlip_Id = @PermissionSlipId;
";

                var permissionSlip = await _dbConnection.QuerySingleOrDefaultAsync<SinglePermissionSlipDTO>(sql, new { PermissionSlipId = permissionSlipId });


                if (permissionSlip != null)
                {
                    return new ServiceResponse<SinglePermissionSlipDTO>(true, "Permission slip retrieved successfully", permissionSlip, 200);
                }
                else
                {
                    return new ServiceResponse<SinglePermissionSlipDTO>(false, "Permission slip not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SinglePermissionSlipDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddPermissionSlip(PermissionSlip permissionSlipDto)
        {
            try
            {
                string query = @"
                INSERT INTO tbl_PermissionSlip (Student_Id, Student_Parent_Info_id, Institute_id, RequestedDateTime, Reason, Status,Qr_Code)
                VALUES (@Student_Id, @Student_Parent_Info_id, @Institute_id, GETDATE(), @Reason, @Status,@Qr_Code);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                var newPermissionSlipId = await _dbConnection.ExecuteScalarAsync<int>(query, new
                {
                    permissionSlipDto.Student_Id,
                    permissionSlipDto.Student_Parent_Info_id,
                    permissionSlipDto.Institute_id,
                    permissionSlipDto.Reason,
                    permissionSlipDto.Qr_Code,

                    Status = (int)Permission_Status.Pending
                });

                return new ServiceResponse<string>(true, "Permission slip added successfully", newPermissionSlipId.ToString(), 201);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
