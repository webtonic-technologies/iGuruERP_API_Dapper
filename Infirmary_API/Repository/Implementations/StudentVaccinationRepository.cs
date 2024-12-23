using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        //public async Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        //{
        //    try
        //    {
        //        _connection.Open();
        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                int studentVaccinationID;

        //                if (request.StudentVaccinationID == 0)
        //                {
        //                    // Insert new student vaccination record
        //                    string insertVaccinationSql = @"
        //                        INSERT INTO tblStudentVaccination (AcademicYear, ClassID, SectionID, VaccinationID, DateOfVaccination, InstituteID, IsActive)
        //                        VALUES (@AcademicYear, @ClassID, @SectionID, @VaccinationID, @DateOfVaccination, @InstituteID, @IsActive);
        //                        SELECT CAST(SCOPE_IDENTITY() as int);";

        //                    studentVaccinationID = await _connection.ExecuteScalarAsync<int>(insertVaccinationSql, request, transaction);
        //                }
        //                else
        //                {
        //                    // Update existing student vaccination record
        //                    string updateVaccinationSql = @"
        //                        UPDATE tblStudentVaccination
        //                        SET AcademicYear = @AcademicYear,
        //                            ClassID = @ClassID,
        //                            SectionID = @SectionID,
        //                            VaccinationID = @VaccinationID,
        //                            DateOfVaccination = @DateOfVaccination,
        //                            InstituteID = @InstituteID,
        //                            IsActive = @IsActive
        //                        WHERE StudentVaccinationID = @StudentVaccinationID";

        //                    await _connection.ExecuteAsync(updateVaccinationSql, request, transaction);
        //                    studentVaccinationID = request.StudentVaccinationID;
        //                }

        //                // Delete existing mappings if updating
        //                if (request.StudentVaccinationID != 0)
        //                {
        //                    string deleteMappingsSql = "DELETE FROM tblStudentVaccinationMapping WHERE StudentVaccinationID = @StudentVaccinationID";
        //                    await _connection.ExecuteAsync(deleteMappingsSql, new { StudentVaccinationID = studentVaccinationID }, transaction);
        //                }

        //                // Insert new student mappings
        //                foreach (var studentID in request.StudentIDs)
        //                {
        //                    string insertMappingSql = @"
        //                        INSERT INTO tblStudentVaccinationMapping (StudentVaccinationID, StudentID)
        //                        VALUES (@StudentVaccinationID, @StudentID)";

        //                    await _connection.ExecuteAsync(insertMappingSql, new { StudentVaccinationID = studentVaccinationID, StudentID = studentID }, transaction);
        //                }

        //                transaction.Commit();
        //                return new ServiceResponse<string>(true, "Operation Successful", "Student vaccinations added/updated successfully", 200);
        //            }
        //            catch (System.Exception ex)
        //            {
        //                transaction.Rollback();
        //                return new ServiceResponse<string>(false, ex.Message, null, 500);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        _connection.Close();
        //    }
        //}

        public async Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        int studentVaccinationID;

                        // Ensure the DateOfVaccination is formatted to 'DD-MM-YYYY'
                        DateTime parsedDate;
                        bool isValidDate = DateTime.TryParseExact(request.DateOfVaccination, "dd-MM-yyyy",
                                                                  System.Globalization.CultureInfo.InvariantCulture,
                                                                  System.Globalization.DateTimeStyles.None,
                                                                  out parsedDate);

                        if (!isValidDate)
                        {
                            return new ServiceResponse<string>(false, "Invalid date format. Please use 'DD-MM-YYYY'.", null, 400);
                        }

                        // Format the date to the desired format for SQL query
                        string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                        if (request.StudentVaccinationID == 0)
                        {
                            // Insert new student vaccination record
                            string insertVaccinationSql = @"
                        INSERT INTO tblStudentVaccination (AcademicYear, ClassID, SectionID, VaccinationID, DateOfVaccination, InstituteID, IsActive)
                        VALUES (@AcademicYear, @ClassID, @SectionID, @VaccinationID, @FormattedDate, @InstituteID, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                            studentVaccinationID = await _connection.ExecuteScalarAsync<int>(insertVaccinationSql, new
                            {
                                request.AcademicYear,
                                request.ClassID,
                                request.SectionID,
                                request.VaccinationID,
                                FormattedDate = formattedDate,
                                request.InstituteID,
                                request.IsActive
                            }, transaction);
                        }
                        else
                        {
                            // Update existing student vaccination record
                            string updateVaccinationSql = @"
                        UPDATE tblStudentVaccination
                        SET AcademicYear = @AcademicYear,
                            ClassID = @ClassID,
                            SectionID = @SectionID,
                            VaccinationID = @VaccinationID,
                            DateOfVaccination = @FormattedDate,
                            InstituteID = @InstituteID,
                            IsActive = @IsActive
                        WHERE StudentVaccinationID = @StudentVaccinationID";

                            await _connection.ExecuteAsync(updateVaccinationSql, new
                            {
                                request.AcademicYear,
                                request.ClassID,
                                request.SectionID,
                                request.VaccinationID,
                                FormattedDate = formattedDate,
                                request.InstituteID,
                                request.IsActive,
                                request.StudentVaccinationID
                            }, transaction);
                            studentVaccinationID = request.StudentVaccinationID;
                        }

                        // Delete existing mappings if updating
                        if (request.StudentVaccinationID != 0)
                        {
                            string deleteMappingsSql = "DELETE FROM tblStudentVaccinationMapping WHERE StudentVaccinationID = @StudentVaccinationID";
                            await _connection.ExecuteAsync(deleteMappingsSql, new { StudentVaccinationID = studentVaccinationID }, transaction);
                        }

                        // Insert new student mappings
                        foreach (var studentID in request.StudentIDs)
                        {
                            string insertMappingSql = @"
                        INSERT INTO tblStudentVaccinationMapping (StudentVaccinationID, StudentID)
                        VALUES (@StudentVaccinationID, @StudentID)";

                            await _connection.ExecuteAsync(insertMappingSql, new { StudentVaccinationID = studentVaccinationID, StudentID = studentID }, transaction);
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Operation Successful", "Student vaccinations added/updated successfully", 200);
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
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
                // Build dynamic count query based on filters
                string countSql = @"
                SELECT COUNT(*) 
                FROM tblStudentVaccination sv
                INNER JOIN tblStudentVaccinationMapping svm ON sv.StudentVaccinationID = svm.StudentVaccinationID
                WHERE sv.IsActive = 1 
                    AND sv.InstituteID = @InstituteID
                    AND (@ClassID = 0 OR sv.ClassID = @ClassID)
                    AND (@SectionID = 0 OR sv.SectionID = @SectionID)
                    AND (@VaccinationID = 0 OR sv.VaccinationID = @VaccinationID)";

                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    request.VaccinationID
                });

                // Updated SQL query to fetch the student vaccinations based on the filters
                string sql = @"
                SELECT 
                    sv.StudentVaccinationID,
                    sv.AcademicYear,
                    sv.ClassID,
                    c.class_name AS ClassName,
                    sv.SectionID,
                    sec.section_name AS SectionName,
                    svm.StudentID,  -- Use the correct alias for StudentID
                    s.First_Name + ' ' + s.Last_Name AS StudentName,
                    sv.VaccinationID,
                    sv.DateOfVaccination,
                    sv.InstituteID,
                    sv.IsActive
                FROM 
                    tblStudentVaccination sv
                INNER JOIN
                    tblStudentVaccinationMapping svm ON sv.StudentVaccinationID = svm.StudentVaccinationID  -- Ensure the join is first
                LEFT OUTER JOIN 
                    tbl_StudentMaster s ON svm.StudentID = s.student_id  -- Correct alias svm.StudentID
                LEFT OUTER JOIN 
                    tbl_Class c ON sv.ClassID = c.class_id
                LEFT OUTER JOIN 
                    tbl_Section sec ON sv.SectionID = sec.section_id
                WHERE
                    sv.IsActive = 1 
                    AND sv.InstituteID = @InstituteID
                    AND (@ClassID = 0 OR sv.ClassID = @ClassID)
                    AND (@SectionID = 0 OR sv.SectionID = @SectionID)
                    AND (@VaccinationID = 0 OR sv.VaccinationID = @VaccinationID)
                ORDER BY 
                    sv.StudentVaccinationID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<StudentVaccinationResponse>(sql, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    request.VaccinationID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // Format DateOfVaccination as '22 Feb, 2024 at 02:00 PM'
                foreach (var vaccination in result)
                {
                    vaccination.DateOfVaccination = DateTime.Parse(vaccination.DateOfVaccination).ToString("dd MMM, yyyy 'at' hh:mm tt");
                }

                return new ServiceResponse<List<StudentVaccinationResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentVaccinationResponse>>(false, ex.Message, null, 500);
            }
        }

        //public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request)
        //{
        //    try
        //    {
        //        string countSql = @"SELECT COUNT(*) FROM tblStudentVaccination WHERE IsActive = 1 AND InstituteID = @InstituteID";
        //        int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

        //        string sql = @"
        //        SELECT 
        //            sv.StudentVaccinationID,
        //            sv.AcademicYear,
        //            sv.ClassID,
        //            c.class_name AS ClassName,
        //            sv.SectionID,
        //            sec.section_name AS SectionName,
        //            sv.StudentID,
        //            s.First_Name + ' ' + s.Last_Name AS StudentName,
        //            sv.VaccinationID,
        //            sv.DateOfVaccination,
        //            sv.InstituteID,
        //            sv.IsActive
        //        FROM 
        //            tblStudentVaccination sv
        //        JOIN 
        //            tbl_StudentMaster s ON sv.StudentID = s.student_id
        //        JOIN 
        //            tbl_Class c ON sv.ClassID = c.class_id
        //        JOIN 
        //            tbl_Section sec ON sv.SectionID = sec.section_id
        //        WHERE
        //            sv.IsActive = 1 AND sv.InstituteID = @InstituteID
        //        ORDER BY 
        //            sv.StudentVaccinationID
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //        var result = await _connection.QueryAsync<StudentVaccinationResponse>(sql, new
        //        {
        //            request.InstituteID,
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize
        //        });

        //        return new ServiceResponse<List<StudentVaccinationResponse>>(true, "Records found", result.ToList(), 200, totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<StudentVaccinationResponse>>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetStudentVaccinationById(int id)
        {
            try
            {
                string query = @"
                SELECT 
                    sv.StudentVaccinationID,
                    sv.AcademicYear,
                    sv.ClassID,
                    sv.SectionID,
                    svm.StudentID,  -- StudentID from tblStudentVaccinationMapping
                    sv.VaccinationID,
                    sv.DateOfVaccination,
                    sv.InstituteID,
                    sv.IsActive,
                    s.First_Name + ' ' + s.Last_Name AS StudentName,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName
                FROM 
                    tblStudentVaccination sv
                INNER JOIN
                    tblStudentVaccinationMapping svm ON sv.StudentVaccinationID = svm.StudentVaccinationID  -- Join with mapping table
                LEFT OUTER JOIN 
                    tbl_StudentMaster s ON svm.StudentID = s.student_id  -- Get student name from tbl_StudentMaster
                LEFT OUTER JOIN 
                    tbl_Class c ON sv.ClassID = c.class_id  -- Get class name from tbl_Class
                LEFT OUTER JOIN 
                    tbl_Section sec ON sv.SectionID = sec.section_id  -- Get section name from tbl_Section
                WHERE 
                    sv.StudentVaccinationID = @Id AND sv.IsActive = 1";

                // Change to QueryAsync to handle multiple records
                var result = await _connection.QueryAsync<StudentVaccinationResponse>(query, new { Id = id });

                // Format DateOfVaccination for each record
                foreach (var vaccination in result)
                {
                    if (DateTime.TryParse(vaccination.DateOfVaccination, out var parsedDate))
                    {
                        vaccination.DateOfVaccination = parsedDate.ToString("dd MMM, yyyy 'at' hh:mm tt");
                    }
                }

                if (result != null && result.Any())
                    return new ServiceResponse<List<StudentVaccinationResponse>>(true, "Record found", result.ToList(), 200);
                else
                    return new ServiceResponse<List<StudentVaccinationResponse>>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentVaccinationResponse>>(false, ex.Message, null, 500);
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


        public async Task<List<GetStudentVaccinationsExportResponse>> GetStudentVaccinationsData(int instituteId, int classId, int sectionId, int vaccinationId)
        {
            string query = @"
                SELECT 
                    sv.StudentVaccinationID,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName,
                    s.First_Name + ' ' + s.Last_Name AS StudentName,
                    sv.DateOfVaccination
                FROM 
                    tblStudentVaccination sv
                INNER JOIN 
                    tblStudentVaccinationMapping svm ON sv.StudentVaccinationID = svm.StudentVaccinationID
                LEFT JOIN 
                    tbl_StudentMaster s ON svm.StudentID = s.student_id
                LEFT JOIN 
                    tbl_Class c ON sv.ClassID = c.class_id
                LEFT JOIN 
                    tbl_Section sec ON sv.SectionID = sec.section_id
                WHERE 
                    sv.IsActive = 1 
                    AND sv.InstituteID = @InstituteID
                    AND (@ClassID = 0 OR sv.ClassID = @ClassID)
                    AND (@SectionID = 0 OR sv.SectionID = @SectionID)
                    AND (@VaccinationID = 0 OR sv.VaccinationID = @VaccinationID)
                ORDER BY 
                    sv.StudentVaccinationID";

            return (await _connection.QueryAsync<GetStudentVaccinationsExportResponse>(query, new { InstituteID = instituteId, ClassID = classId, SectionID = sectionId, VaccinationID = vaccinationId })).AsList();
        }
    }
}
