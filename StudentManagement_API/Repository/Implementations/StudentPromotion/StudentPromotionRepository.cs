using Dapper;
using System.Data.SqlClient;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace StudentManagement_API.Repository.Implementations
{
    public class StudentPromotionRepository : IStudentPromotionRepository
    {
        private readonly string _connectionString;

        public StudentPromotionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<GetClassPromotionResponse>> GetClassPromotionAsync(GetClassPromotionRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            SELECT
                c.class_id AS ClassID,
                c.class_name AS Class,
                cp.PromotedClassID,
                cpPromoted.class_name AS PromotedClass
            FROM tbl_Class c
            LEFT JOIN tblClassPromotion cp ON c.class_id = cp.CurrentClassID AND cp.InstituteID = @InstituteID
            LEFT JOIN tbl_Class cpPromoted ON cp.PromotedClassID = cpPromoted.class_id
            WHERE c.institute_id = @InstituteID AND c.IsDeleted = 0;
        ";

            connection.Open();
            var result = await connection.QueryAsync<GetClassPromotionResponse>(sql, new { InstituteID = request.InstituteID });
            return result;
        }


        public async Task<bool> UpdateClassPromotionConfigurationAsync(UpdateClassPromotionConfigurationRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var config in request.Classes)
                {
                    // Check if a configuration exists for the given class and institute.
                    var existsQuery = @"
                SELECT COUNT(1) 
                FROM tblClassPromotion 
                WHERE CurrentClassID = @CurrentClassID 
                  AND InstituteID = @InstituteID;";
                    var count = await connection.ExecuteScalarAsync<int>(
                        existsQuery,
                        new { CurrentClassID = config.CurrentClassID, InstituteID = request.InstituteID },
                        transaction
                    );

                    if (count > 0)
                    {
                        // Update the existing configuration.
                        var updateQuery = @"
                    UPDATE tblClassPromotion 
                    SET PromotedClassID = @PromotedClassID
                    WHERE CurrentClassID = @CurrentClassID 
                      AND InstituteID = @InstituteID;";
                        await connection.ExecuteAsync(
                            updateQuery,
                            new
                            {
                                PromotedClassID = config.PromotedClassID,
                                CurrentClassID = config.CurrentClassID,
                                InstituteID = request.InstituteID
                            },
                            transaction
                        );
                    }
                    else
                    {
                        // Insert a new configuration.
                        var insertQuery = @"
                    INSERT INTO tblClassPromotion (CurrentClassID, PromotedClassID, InstituteID)
                    VALUES (@CurrentClassID, @PromotedClassID, @InstituteID);";
                        await connection.ExecuteAsync(
                            insertQuery,
                            new
                            {
                                CurrentClassID = config.CurrentClassID,
                                PromotedClassID = config.PromotedClassID,
                                InstituteID = request.InstituteID
                            },
                            transaction
                        );
                    }
                }

                // Insert log information for the overall configuration update into tblClassPromotionLogs.
                var logQuery = @"
            INSERT INTO tblClassPromotionLogs (UserID, IPAddress, ConfigurationDate, InstituteID)
            VALUES (@UserID, @IPAddress, GETDATE(), @InstituteID);";
                await connection.ExecuteAsync(
                    logQuery,
                    new
                    {
                        UserID = request.UserID,
                        IPAddress = request.IPAddress,
                        InstituteID = request.InstituteID
                    },
                    transaction
                );

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
         
        public async Task<(IEnumerable<GetClassPromotionHistoryResponse> Data, int TotalCount)> GetClassPromotionHistoryAsync(GetClassPromotionHistoryRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Calculate the offset for pagination.
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Query with formatted date
            string query = @"
        SELECT 
            ep.First_Name + ' ' + ISNULL(ep.Middle_Name + ' ', '') + ep.Last_Name AS UserName,
            FORMAT(l.ConfigurationDate, 'dd-MM-yyyy ""at"" hh:mm tt') AS DateTime,
            l.IPAddress
        FROM tblClassPromotionLogs l
        JOIN tbl_EmployeeProfileMaster ep ON l.UserID = ep.Employee_id
        WHERE l.InstituteID = @InstituteID
        ORDER BY l.ConfigurationDate DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

            var data = await connection.QueryAsync<GetClassPromotionHistoryResponse>(
                query,
                new { InstituteID = request.InstituteID, Offset = offset, PageSize = request.PageSize }
            );

            // Get the total count for pagination.
            string countQuery = @"
        SELECT COUNT(1)
        FROM tblClassPromotionLogs
        WHERE InstituteID = @InstituteID;
    ";

            int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { InstituteID = request.InstituteID });

            return (data, totalCount);
        }

        public async Task<IEnumerable<GetClassPromotionHistoryExportResponse>> GetClassPromotionHistoryExportAsync(GetClassPromotionHistoryExportRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = @"
                SELECT 
                    ep.First_Name + ' ' + ISNULL(ep.Middle_Name + ' ', '') + ep.Last_Name AS UserName,
                    FORMAT(l.ConfigurationDate, 'dd-MM-yyyy ""at"" hh:mm tt') AS DateTime,
                    l.IPAddress
                FROM tblClassPromotionLogs l
                JOIN tbl_EmployeeProfileMaster ep ON l.UserID = ep.Employee_id
                WHERE l.InstituteID = @InstituteID
                ORDER BY l.ConfigurationDate DESC;
            ";

            var data = await connection.QueryAsync<GetClassPromotionHistoryExportResponse>(
                sql, new { InstituteID = request.InstituteID }
            );
            return data;
        }


        public async Task<bool> PromoteStudentsAsync(PromoteStudentsRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Loop through each student ID provided
                foreach (var studentId in request.Students)
                {
                    // Retrieve the student's current section from tbl_StudentMaster
                    string selectQuery = @"
                        SELECT section_id 
                        FROM tbl_StudentMaster 
                        WHERE student_id = @StudentID 
                          AND Institute_id = @InstituteID
                          AND class_id = @CurrentClassID;";

                    var currentSection = await connection.ExecuteScalarAsync<int?>(
                        selectQuery,
                        new
                        {
                            StudentID = studentId,
                            InstituteID = request.InstituteID,
                            CurrentClassID = request.CurrentClass.ClassID
                        },
                        transaction
                    );

                    // If student record not found or current section is not set, skip or throw error as needed.
                    if (currentSection == null)
                    {
                        // Optionally, you can skip promotion for this student or throw an exception.
                        continue; // or throw new Exception($"Student {studentId} not found in the current class.");
                    }

                    // Check if the student's current section is among the provided CurrentClass.Sections
                    int index = request.CurrentClass.Sections.IndexOf(currentSection.Value);
                    if (index == -1)
                    {
                        // Optionally, skip promotion for this student if not found in mapping
                        continue; // or throw new Exception($"Student {studentId}'s section {currentSection.Value} is not eligible for promotion.");
                    }

                    // Determine the corresponding new section using the same index from NextClass.Sections.
                    int newSection = request.NextClass.Sections[index];

                    // Update the student record in tbl_StudentMaster
                    string updateStudentQuery = @"
                        UPDATE tbl_StudentMaster
                        SET 
                            class_id = @NextClassID,
                            section_id = @NewSection,
                            AcademicYearCode = @AcademicYearCode
                        WHERE student_id = @StudentID
                          AND Institute_id = @InstituteID;";

                    await connection.ExecuteAsync(
                        updateStudentQuery,
                        new
                        {
                            NextClassID = request.NextClass.ClassID,
                            NewSection = newSection,
                            AcademicYearCode = request.AcademicYearCode,
                            StudentID = studentId,
                            InstituteID = request.InstituteID
                        },
                        transaction
                    );

                    // Insert a new record into tblStudentStandards
                    string insertStandardQuery = @"
                        INSERT INTO tblStudentStandards (StudentID, ClassID, SectionID, AcademicYearCode, InstituteID)
                        VALUES (@StudentID, @NextClassID, @NewSection, @AcademicYearCode, @InstituteID);";

                    await connection.ExecuteAsync(
                        insertStandardQuery,
                        new
                        {
                            StudentID = studentId,
                            NextClassID = request.NextClass.ClassID,
                            NewSection = newSection,
                            AcademicYearCode = request.AcademicYearCode,
                            InstituteID = request.InstituteID
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<GetClassesResponse>> GetClassesAsync(GetClassesRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = @"
                SELECT 
                    class_id AS ClassID, 
                    class_name AS ClassName
                FROM tbl_Class
                WHERE institute_id = @InstituteID
                  AND IsDeleted = 0;
            ";
            var result = await connection.QueryAsync<GetClassesResponse>(sql, new { InstituteID = request.InstituteID });
            return result;
        }

        public async Task<IEnumerable<GetSectionsResponse>> GetSectionsAsync(GetSectionsRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT 
                    s.section_id AS SectionID, 
                    s.section_name AS SectionName
                FROM tbl_Section s
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                WHERE s.class_id = @ClassID
                  AND c.institute_id = @InstituteID
                  AND s.IsDeleted = 0;
            ";

            var result = await connection.QueryAsync<GetSectionsResponse>(
                sql, new { ClassID = request.ClassID, InstituteID = request.InstituteID }
            );

            return result;
        }


        public async Task<IEnumerable<GetStudentsPromotionResponse>> GetStudentsAsync(GetStudentsPromotionRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = @"
                SELECT sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name + ' ', ''), sm.Last_Name) AS StudentName,
                    c.class_name + ' - ' + sct.section_name AS ClassSection
                FROM tbl_StudentMaster sm
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_Section sct ON sm.section_id = sct.section_id
                WHERE sm.Institute_id = @InstituteID 
                  AND sm.class_id = @ClassID;
            ";
            var result = await connection.QueryAsync<GetStudentsPromotionResponse>(
                sql, new { InstituteID = request.InstituteID, ClassID = request.ClassID });
            return result;
        }

    }
}
