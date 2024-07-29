using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class LessonPlanningRepository : ILessonPlanningRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<LessonPlanningRepository> _logger;

        public LessonPlanningRepository(IConfiguration configuration, ILogger<LessonPlanningRepository> logger)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> AddUpdateLessonPlanning(LessonPlanningRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update LessonPlanning
                if (request.LessonPlanningID == 0)
                {
                    var lessonPlanningSql = @"INSERT INTO tblLessonPlanning (AcademicYear, ClassID, SectionID, SubjectID, InstituteID, IsActive) 
                                              VALUES (@AcademicYear, @ClassID, @SectionID, @SubjectID, @InstituteID, 1);
                                              SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.LessonPlanningID = await _dbConnection.QuerySingleAsync<int>(lessonPlanningSql, new
                    {
                        request.AcademicYear,
                        request.ClassID,
                        request.SectionID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }
                else
                {
                    var lessonPlanningSql = @"UPDATE tblLessonPlanning SET 
                                              AcademicYear = @AcademicYear, 
                                              ClassID = @ClassID, 
                                              SectionID = @SectionID, 
                                              SubjectID = @SubjectID, 
                                              InstituteID = @InstituteID,
                                              IsActive = @IsActive
                                              WHERE LessonPlanningID = @LessonPlanningID";
                    await _dbConnection.ExecuteAsync(lessonPlanningSql, new
                    {
                        request.LessonPlanningID,
                        request.AcademicYear,
                        request.ClassID,
                        request.SectionID,
                        request.SubjectID,
                        request.InstituteID,
                        request.IsActive
                    }, transaction);
                }

                // Insert or Update LessonPlanning Information
                foreach (var info in request.LessonPlanningInformation)
                {
                    if (info.LessonPlanningInfoID == 0)
                    {
                        var infoSql = @"INSERT INTO tblLessonPlanningInformation (LessonPlanningID, LessonDate, PlanTypeID, CurriculumChapterID, CurriculumSubTopicID, Synopsis, Introduction, MainTeaching, Conclusion, Attachments) 
                                        VALUES (@LessonPlanningID, @LessonDate, @PlanTypeID, @CurriculumChapterID, @CurriculumSubTopicID, @Synopsis, @Introduction, @MainTeaching, @Conclusion, @Attachments);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                        info.LessonPlanningInfoID = await _dbConnection.QuerySingleAsync<int>(infoSql, new
                        {
                            LessonPlanningID = request.LessonPlanningID,
                            info.LessonDate,
                            info.PlanTypeID,
                            info.CurriculumChapterID,
                            info.CurriculumSubTopicID,
                            info.Synopsis,
                            info.Introduction,
                            info.MainTeaching,
                            info.Conclusion,
                            info.Attachments
                        }, transaction);
                    }
                    else
                    {
                        var infoSql = @"UPDATE tblLessonPlanningInformation SET 
                                        LessonDate = @LessonDate, 
                                        PlanTypeID = @PlanTypeID, 
                                        CurriculumChapterID = @CurriculumChapterID, 
                                        CurriculumSubTopicID = @CurriculumSubTopicID, 
                                        Synopsis = @Synopsis, 
                                        Introduction = @Introduction, 
                                        MainTeaching = @MainTeaching, 
                                        Conclusion = @Conclusion, 
                                        Attachments = @Attachments
                                        WHERE LessonPlanningInfoID = @LessonPlanningInfoID";
                        await _dbConnection.ExecuteAsync(infoSql, new
                        {
                            info.LessonPlanningInfoID,
                            info.LessonDate,
                            info.PlanTypeID,
                            info.CurriculumChapterID,
                            info.CurriculumSubTopicID,
                            info.Synopsis,
                            info.Introduction,
                            info.MainTeaching,
                            info.Conclusion,
                            info.Attachments
                        }, transaction);
                    }
                }

                transaction.Commit();
                return new ServiceResponse<string>(request.LessonPlanningID.ToString(), true, "Lesson Planning added/updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating lesson planning.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GetAllLessonPlanningResponse>>> GetAllLessonPlanning(GetAllLessonPlanningRequest request)
        {
            try
            {
                var sql = @"
            WITH LessonPlanningData AS (
                SELECT 
                    lp.LessonPlanningID,
                    lp.AcademicYear,
                    lp.ClassID,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName,
                    lp.SubjectID,
                    s.subject_name AS SubjectName,
                    t.employee_name AS TeacherName,
                    lp.InstituteID,
                    lp.IsActive,
                    ROW_NUMBER() OVER (ORDER BY lp.LessonPlanningID) AS RowNum
                FROM tblLessonPlanning lp
                INNER JOIN tbl_Class c ON lp.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON lp.SectionID = sec.section_id
                INNER JOIN tbl_InstituteSubjects s ON lp.SubjectID = s.institute_subject_id
                INNER JOIN tbl_EmployeeProfileMaster t ON lp.TeacherID = t.employee_id
                WHERE lp.InstituteID = @InstituteID AND lp.IsActive = 1
            )
            SELECT 
                lpd.LessonPlanningID,
                lpd.AcademicYear,
                lpd.ClassID,
                lpd.ClassName,
                lpd.SectionName,
                lpd.SubjectID,
                lpd.SubjectName,
                lpd.TeacherName,
                lpd.InstituteID,
                lpd.IsActive
            FROM LessonPlanningData lpd
            WHERE lpd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;

            WITH LessonPlanningDataForInfo AS (
                SELECT 
                    lp.LessonPlanningID,
                    ROW_NUMBER() OVER (ORDER BY lp.LessonPlanningID) AS RowNum
                FROM tblLessonPlanning lp
                WHERE lp.InstituteID = @InstituteID AND lp.IsActive = 1
            )
            SELECT
                lpi.LessonPlanningInfoID,
                lpi.LessonPlanningID,
                lpi.LessonDate,
                lpi.PlanTypeID,
                lpi.CurriculumChapterID,
                lpi.CurriculumSubTopicID,
                lpi.Synopsis,
                lpi.Introduction,
                lpi.MainTeaching,
                lpi.Conclusion,
                lpi.Attachments
            FROM tblLessonPlanningInformation lpi
            INNER JOIN LessonPlanningDataForInfo lpd ON lpi.LessonPlanningID = lpd.LessonPlanningID
            WHERE lpd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                var lessonPlannings = (await multi.ReadAsync<GetAllLessonPlanningResponse>()).ToList();
                var lessonPlanningInfos = (await multi.ReadAsync<LessonPlanningInformationResponse>()).ToList();

                foreach (var lessonPlanning in lessonPlannings)
                {
                    lessonPlanning.LessonPlanningInformation = lessonPlanningInfos
                        .Where(info => info.LessonPlanningID == lessonPlanning.LessonPlanningID)
                        .ToList();
                }

                return new ServiceResponse<List<GetAllLessonPlanningResponse>>(lessonPlannings, true, "Lesson Plannings retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lesson plannings.");
                return new ServiceResponse<List<GetAllLessonPlanningResponse>>(null, false, "Operation failed: " + ex.Message);
            }
        }


        public async Task<ServiceResponse<LessonPlanning>> GetLessonPlanningById(int id)
        {
            try
            {
                var sql = @"
                    SELECT * FROM tblLessonPlanning WHERE LessonPlanningID = @LessonPlanningID;
                    SELECT * FROM tblLessonPlanningInformation WHERE LessonPlanningID = @LessonPlanningID;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new { LessonPlanningID = id });

                var lessonPlanning = await multi.ReadSingleOrDefaultAsync<LessonPlanning>();
                if (lessonPlanning != null)
                {
                    lessonPlanning.LessonPlanningInformation = (await multi.ReadAsync<LessonPlanningInformation>()).ToList();
                }

                return new ServiceResponse<LessonPlanning>(lessonPlanning, lessonPlanning != null, lessonPlanning != null ? "Lesson Planning found." : "Lesson Planning not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lesson planning by ID.");
                return new ServiceResponse<LessonPlanning>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteLessonPlanning(int id)
        {
            try
            {
                var sql = @"UPDATE tblLessonPlanning SET IsActive = 0 WHERE LessonPlanningID = @LessonPlanningID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { LessonPlanningID = id });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0, rowsAffected > 0 ? "Lesson Planning deleted successfully." : "Delete operation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting lesson planning.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message);
            }
        }
    }
}


