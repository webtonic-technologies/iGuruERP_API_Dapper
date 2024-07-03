﻿using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Student_API.Repository.Implementations
{
    public class StudentPromotionRepository : IStudentPromotionRepository
    {
        private readonly IDbConnection _connection;

        public StudentPromotionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, string sortField, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // Validate sortField and sortDirection
                var allowedSortFields = new List<string> { "Student_Name", "Class_Section" };
                var allowedSortDirections = new List<string> { "ASC", "DESC" };

                // Default to "Student_Name" if sortField is invalid
                if (!allowedSortFields.Contains(sortField))
                {
                    sortField = "Student_Name";
                }

                // Default to "ASC" if sortDirection is invalid
                sortDirection = sortDirection.ToUpper();
                if (!allowedSortDirections.Contains(sortDirection))
                {
                    sortDirection = "ASC";
                }

                // Map sortField to SQL column names
                string sortColumn;
                switch (sortField)
                {
                    case "Class_Section":
                        sortColumn = "Class_Section";
                        break;
                    case "Student_Name":
                    default:
                        sortColumn = "Student_Name";
                        break;
                }

                string query = $@"
        IF OBJECT_ID('tempdb..#TempStudentDetails') IS NOT NULL
    DROP TABLE #TempStudentDetails;

        SELECT 
            student_id, 
            CONCAT(first_name, ' ', last_name) AS Student_Name, 
            CONCAT(tbl_Class.class_name, ' - ', tbl_Section.Section_name) AS Class_Section,
            tbl_StudentMaster.class_id,
            tbl_StudentMaster.Section_Id
        INTO 
            #TempStudentDetails
        FROM 
            tbl_StudentMaster
        LEFT JOIN 
            tbl_Class ON tbl_Class.Class_id = tbl_StudentMaster.class_id
        LEFT JOIN 
            tbl_Section ON tbl_Section.section_id = tbl_StudentMaster.section_id
        WHERE 
            tbl_StudentMaster.class_id = @ClassId OR @ClassId= 0;

       
        SELECT 
            COUNT(*) 
        FROM 
            #TempStudentDetails;

      
        SELECT 
            student_id, 
            Student_Name, 
            Class_Section,
            class_id,
            Section_Id
        FROM 
            #TempStudentDetails
        ORDER BY 
            {sortColumn} {sortDirection}, student_id
        OFFSET 
            @Offset ROWS
        FETCH NEXT 
            @PageSize ROWS ONLY;

       IF OBJECT_ID('tempdb..#TempStudentDetails') IS NOT NULL
    DROP TABLE #TempStudentDetails;

        ";

                List<StudentPromotionDTO> students;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    using (var multi = await _connection.QueryMultipleAsync(query, new { ClassId = classId, Offset = offset, PageSize = pageSize }))
                    {
                        totalRecords = multi.ReadSingle<int>();
                        students = multi.Read<StudentPromotionDTO>().ToList();
                    }

                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "Students retrieved successfully", students, 200, totalRecords);
                }
                else
                {
                    using (var multi = await _connection.QueryMultipleAsync(query, new { ClassId = classId, Offset = 0, PageSize = int.MaxValue }))
                    {
                        totalRecords = multi.ReadSingle<int>();
                        students = multi.Read<StudentPromotionDTO>().ToList();
                    }

                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "All students retrieved successfully", students, 200, totalRecords);
                }
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
                string query = @"
                UPDATE tbl_StudentMaster
                SET class_id = @NextClassId , section_id= @sectionId
                WHERE student_id IN @StudentIds";

                await _connection.ExecuteAsync(query, new { NextClassId = nextClassId, StudentIds = studentIds, sectionId = sectionId });

                return new ServiceResponse<bool>(true, "Students promoted successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<bool>> PromoteClasses(ClassPromotionDTO classPromotionDTO)
        {


            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string tempTableQuery = @"
            IF OBJECT_ID('tempdb..#TempPromotion') IS NOT NULL
            DROP TABLE #TempPromotion;

            CREATE TABLE #TempPromotion (
                StudentId INT,
                NewClassId INT,
                NewSectionId INT
            )";

                    await _connection.ExecuteAsync(tempTableQuery, transaction: transaction);

                    // Insert intermediate promotions into the temp table
                    foreach (var classSection in classPromotionDTO.ClassSections)
                    {
                        string insertTempQuery = @"
                INSERT INTO #TempPromotion (StudentId, NewClassId, NewSectionId)
                SELECT student_id, @NewClassId, @NewSectionId
                FROM tbl_StudentMaster
                WHERE class_id = @OldClassId AND section_id = @OldSectionId";

                        await _connection.ExecuteAsync(insertTempQuery, new
                        {
                            NewClassId = classSection.NewClassId,
                            NewSectionId = classSection.NewSectionId,
                            OldClassId = classSection.OldClassId,
                            OldSectionId = classSection.OldSectionId
                        }, transaction: transaction);
                    }

                    // Update the actual student master table
                    string updateQuery = @"
            UPDATE SM
            SET SM.class_id = TP.NewClassId, SM.section_id = TP.NewSectionId
            FROM tbl_StudentMaster SM
            INNER JOIN #TempPromotion TP ON SM.student_id = TP.StudentId";

                    await _connection.ExecuteAsync(updateQuery, transaction: transaction);


                    string logQuery = @"
                    INSERT INTO tbl_ClassPromotionLog ( UserId, IPAddress, PromotionDateTime,institute_id)
                    VALUES ( @UserId, @IPAddress, GETDATE(),@institute_id)";

                    await _connection.ExecuteAsync(logQuery, new
                    {
                        UserId = classPromotionDTO.UserId,
                        IPAddress = classPromotionDTO.IPAddress,
                        institute_id = classPromotionDTO.institute_id
                    }, transaction: transaction);
                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Classes promoted successfully", true, 200);
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, ex.Message, false, 500);
                }
            }
        }
        public async Task<ServiceResponse<List<ClassPromotionLogDTO>>> GetClassPromotionLog(GetClassPromotionLogParam obj)
        {
            try
            {
                var allowedSortFields = new List<string> { "LogId", "UserId", "IPAddress" };
                var allowedSortDirections = new List<string> { "ASC", "DESC" };

                if (!allowedSortFields.Contains(obj.sortField))
                {
                    obj.sortField = "PromotionDateTime";
                }

                obj.sortDirection = obj.sortDirection?.ToUpper() ?? "DESC";
                if (!allowedSortDirections.Contains(obj.sortDirection))
                {
                    obj.sortDirection = "DESC";
                }
                int offset = (obj.pageNumber.HasValue && obj.pageNumber.Value > 0) ? (obj.pageNumber.Value - 1) * obj.pageSize.Value : 0;
                int pageSize = obj.pageSize ?? 10;

                string query = $@"
        SELECT COUNT(*) 
        FROM tbl_ClassPromotionLog 
        WHERE institute_id = @institute_id;

        SELECT LogId, UserId, IPAddress, PromotionDateTime 
        FROM tbl_ClassPromotionLog 
        WHERE institute_id = @institute_id
        ORDER BY {obj.sortField} {obj.sortDirection}
        OFFSET @Offset ROWS 
        FETCH NEXT @PageSize ROWS ONLY;";
                using (var multi = await _connection.QueryMultipleAsync(query, new { institute_id = obj.institute_id, Offset = offset, PageSize = pageSize }))
                {
                    int totalRecords = multi.ReadSingle<int>();
                    var logs = multi.Read<ClassPromotionLogDTO>().ToList();

                    return new ServiceResponse<List<ClassPromotionLogDTO>>(true, "Logs retrieved successfully", logs, 200, totalRecords);
                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassPromotionLogDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}