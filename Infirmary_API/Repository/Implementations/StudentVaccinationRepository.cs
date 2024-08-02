using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class StudentVaccinationRepository : IStudentVaccinationRepository
    {
        private readonly IDbConnection _connection;

        public StudentVaccinationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string query = request.StudentVaccinationID == 0
                            ? @"INSERT INTO tblStudentVaccination (AcademicYear, ClassID, SectionID, StudentID, VaccinationID, DateOfVaccination, InstituteID, IsActive) 
                               VALUES (@AcademicYear, @ClassID, @SectionID, @StudentID, @VaccinationID, @DateOfVaccination, @InstituteID, @IsActive);
                               SELECT CAST(SCOPE_IDENTITY() as int);"
                            : @"UPDATE tblStudentVaccination SET AcademicYear = @AcademicYear, ClassID = @ClassID, SectionID = @SectionID, StudentID = @StudentID, VaccinationID = @VaccinationID, 
                               DateOfVaccination = @DateOfVaccination, InstituteID = @InstituteID, IsActive = @IsActive 
                               WHERE StudentVaccinationID = @StudentVaccinationID;
                               SELECT @StudentVaccinationID;";

                        var studentVaccinationID = await _connection.ExecuteScalarAsync<int>(query, request, transaction);

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Operation Successful", "Student vaccination updated successfully", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblStudentVaccination WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    sv.StudentVaccinationID,
                    sv.AcademicYear,
                    sv.ClassID,
                    c.class_name AS ClassName,
                    sv.SectionID,
                    sec.section_name AS SectionName,
                    sv.StudentID,
                    s.First_Name + ' ' + s.Last_Name AS StudentName,
                    sv.VaccinationID,
                    sv.DateOfVaccination,
                    sv.InstituteID,
                    sv.IsActive
                FROM 
                    tblStudentVaccination sv
                JOIN 
                    tbl_StudentMaster s ON sv.StudentID = s.student_id
                JOIN 
                    tbl_Class c ON sv.ClassID = c.class_id
                JOIN 
                    tbl_Section sec ON sv.SectionID = sec.section_id
                WHERE
                    sv.IsActive = 1 AND sv.InstituteID = @InstituteID
                ORDER BY 
                    sv.StudentVaccinationID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<StudentVaccinationResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<List<StudentVaccinationResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentVaccinationResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<StudentVaccination>> GetStudentVaccinationById(int id)
        {
            try
            {
                string query = @"
                SELECT 
                    sv.StudentVaccinationID,
                    sv.AcademicYear,
                    sv.ClassID,
                    sv.SectionID,
                    sv.StudentID,
                    sv.VaccinationID,
                    sv.DateOfVaccination,
                    sv.InstituteID,
                    sv.IsActive,
                    s.First_Name + ' ' + s.Last_Name AS StudentName,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName
                FROM 
                    tblStudentVaccination sv
                JOIN 
                    tbl_StudentMaster s ON sv.StudentID = s.student_id
                JOIN 
                    tbl_Class c ON sv.ClassID = c.class_id
                JOIN 
                    tbl_Section sec ON sv.SectionID = sec.section_id
                WHERE 
                    sv.StudentVaccinationID = @Id AND sv.IsActive = 1";

                var result = await _connection.QueryFirstOrDefaultAsync<StudentVaccination>(query, new { Id = id });

                if (result != null)
                    return new ServiceResponse<StudentVaccination>(true, "Record found", result, 200);
                else
                    return new ServiceResponse<StudentVaccination>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentVaccination>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStudentVaccination(int id)
        {
            try
            {
                string query = "UPDATE tblStudentVaccination SET IsActive = 0 WHERE StudentVaccinationID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "Student vaccination deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Operation failed", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
