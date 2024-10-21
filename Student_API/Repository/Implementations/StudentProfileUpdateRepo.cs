using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class StudentProfileUpdateRepo : IStudentProfileUpdateRepo
    {
        private readonly IDbConnection _connection;

        public StudentProfileUpdateRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddProfileUpdateRequest(int studentId, int status)
        {
            try
            {
                string sql = @"INSERT INTO tbl_ProfileUpdateRequest (Student_Id, Status, CreatedDateTime)
                       VALUES (@StudentId, @Status, @CreatedDateTime)";

                var parameters = new
                {
                    StudentId = studentId,
                    Status = status,
                    CreatedDateTime = DateTime.Now
                };

                int rowsAffected = await _connection.ExecuteAsync(sql, parameters);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Profile update request added successfully", null, 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to add profile update request", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<string>> UpdateProfileUpdateRequest(int requestId, int newStatus)
        {
            try
            {
                string sql = @"UPDATE tbl_ProfileUpdateRequest
                       SET Status = @NewStatus, UpdatedDateTime = @UpdatedDateTime
                       WHERE Id = @RequestId";

                var parameters = new
                {
                    NewStatus = newStatus,
                    UpdatedDateTime = DateTime.Now,
                    RequestId = requestId
                };

                int rowsAffected = await _connection.ExecuteAsync(sql, parameters);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Profile update request updated successfully", null, 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to update profile update request", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<ProfileUpdateRequestDTO>>> GetProfileUpdateRequests(GetStudentProfileRequestModel obj)
        {
            try
            {
                const int MaxPageSize = int.MaxValue;
                int actualPageSize = obj.pageSize ?? MaxPageSize;
                int actualPageNumber = obj.pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                string sql = @"
        SELECT 
            pur.Id, 
            pur.Student_Id, 
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS Student_Name, 
            c.Class_Name AS Class, 
            sec.Section_Name AS Section, 
            sm.Roll_Number, 
            g.Gender_Type AS Gender, 
            FORMAT(sm.Date_of_Birth, 'dd-MM-yyyy') AS DOB, 
            r.Religion_Type AS Religion, 
             CONCAT(sp.First_Name, ' ', sp.Last_Name) AS Father_Name, 
            pur.Status,
            pur.CreatedDateTime,
            pur.UpdatedDateTime
        FROM 
            tbl_ProfileUpdateRequest pur
        INNER JOIN 
            tbl_StudentMaster sm ON pur.Student_Id = sm.Student_Id
        LEFT JOIN 
            tbl_Class c ON sm.Class_Id = c.Class_Id
        LEFT JOIN 
            tbl_Section sec ON sm.Section_Id = sec.Section_Id
        LEFT JOIN 
            tbl_Gender g ON sm.Gender_Id = g.Gender_Id
        LEFT JOIN 
            tbl_Religion r ON sm.Religion_Id = r.Religion_Id
        LEFT JOIN 
            tbl_StudentParentsInfo sp ON sm.Student_Id = sp.Student_Id AND sp.Parent_Type_Id = 1
        WHERE 
            (sm.Class_Id = @ClassId OR @ClassId = 0)";

                // Add search functionality based on Keyword
                if (!string.IsNullOrEmpty(obj.Keyword))
                {
                    sql += " AND (CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE '%' + @Keyword + '%' OR sm.Roll_Number LIKE '%' + @Keyword + '%')";
                }

                sql += @"
        ORDER BY 
            Student_Name
        OFFSET 
            @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var parameters = new { ClassId = obj.class_id, Offset = offset, PageSize = actualPageSize, Keyword = obj.Keyword };

                var profileUpdateList = (await _connection.QueryAsync<ProfileUpdateRequestDTO>(sql, parameters)).ToList();

                if (profileUpdateList.Any())
                {
                    return new ServiceResponse<List<ProfileUpdateRequestDTO>>(true, "Data fetched successfully", profileUpdateList, 200);
                }
                else
                {
                    return new ServiceResponse<List<ProfileUpdateRequestDTO>>(false, "No data found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ProfileUpdateRequestDTO>>(false, ex.Message, null, 500);
            }
        }

    }
}
