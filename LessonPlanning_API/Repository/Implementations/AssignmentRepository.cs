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
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<AssignmentRepository> _logger;

        public AssignmentRepository(IConfiguration configuration, ILogger<AssignmentRepository> logger)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update Assignment
                if (request.AssignmentID == 0)
                {
                    var assignmentSql = @"INSERT INTO tblAssignment (AssignmentName, IsClasswise, IsStudentwise, ClassID, SectionID, StudentID, SubjectID, AssignmentTypeID, StartDate, SubmissionDate, Description, Reference, Attachments, InstituteID, IsActive) 
                                        VALUES (@AssignmentName, @IsClasswise, @IsStudentwise, @ClassID, @SectionID, @StudentID, @SubjectID, @AssignmentTypeID, @StartDate, @SubmissionDate, @Description, @Reference, @Attachments, @InstituteID, 1);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.AssignmentID = await _dbConnection.QuerySingleAsync<int>(assignmentSql, new
                    {
                        request.AssignmentName,
                        request.IsClasswise,
                        request.IsStudentwise,
                        request.ClassID,
                        request.SectionID,
                        request.StudentID,
                        request.SubjectID,
                        request.AssignmentTypeID,
                        request.StartDate,
                        request.SubmissionDate,
                        request.Description,
                        request.Reference,
                        request.Attachments,
                        request.InstituteID
                    }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblAssignmentClassSection (AssignmentID, ClassID, SectionID) 
                                                VALUES (@AssignmentID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            AssignmentID = request.AssignmentID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }
                else
                {
                    var assignmentSql = @"UPDATE tblAssignment SET 
                                        AssignmentName = @AssignmentName, 
                                        IsClasswise = @IsClasswise,
                                        IsStudentwise = @IsStudentwise,
                                        ClassID = @ClassID, 
                                        SectionID = @SectionID,
                                        StudentID = @StudentID,
                                        SubjectID = @SubjectID, 
                                        AssignmentTypeID = @AssignmentTypeID,
                                        StartDate = @StartDate,
                                        SubmissionDate = @SubmissionDate,
                                        Description = @Description,
                                        Reference = @Reference,
                                        Attachments = @Attachments,
                                        InstituteID = @InstituteID,
                                        IsActive = @IsActive
                                        WHERE AssignmentID = @AssignmentID";
                    await _dbConnection.ExecuteAsync(assignmentSql, new
                    {
                        request.AssignmentID,
                        request.AssignmentName,
                        request.IsClasswise,
                        request.IsStudentwise,
                        request.ClassID,
                        request.SectionID,
                        request.StudentID,
                        request.SubjectID,
                        request.AssignmentTypeID,
                        request.StartDate,
                        request.SubmissionDate,
                        request.Description,
                        request.Reference,
                        request.Attachments,
                        request.InstituteID,
                        request.IsActive
                    }, transaction);

                    var deleteClassSectionsSql = @"DELETE FROM tblAssignmentClassSection WHERE AssignmentID = @AssignmentID";
                    await _dbConnection.ExecuteAsync(deleteClassSectionsSql, new { request.AssignmentID }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblAssignmentClassSection (AssignmentID, ClassID, SectionID) 
                                                VALUES (@AssignmentID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            AssignmentID = request.AssignmentID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }

                transaction.Commit();
                return new ServiceResponse<string>(request.AssignmentID.ToString(), true, "Assignment added/updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating assignment.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request)
        {
            try
            {
                var sql = @"
                    WITH AssignmentData AS (
                        SELECT 
                            a.AssignmentID,
                            a.AssignmentName,
                            s.subject_name AS SubjectName,
                            at.AssignmentType,
                            a.Description,
                            a.Reference,
                            a.Attachments,
                            a.StartDate,
                            a.SubmissionDate,
                            a.IsActive,
                            ROW_NUMBER() OVER (ORDER BY a.AssignmentID) AS RowNum
                        FROM tblAssignment a
                        INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
                        INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
                        WHERE a.InstituteID = @InstituteID AND a.IsActive = 1
                    ), ClassSectionData AS (
                        SELECT 
                            acs.AssignmentID,
                            c.class_name AS ClassName,
                            sec.section_name AS SectionName
                        FROM tblAssignmentClassSection acs
                        INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
                        INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
                        WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
                    )
                    SELECT 
                        ad.AssignmentID,
                        ad.AssignmentName,
                        ad.SubjectName,
                        ad.AssignmentType,
                        ad.Description,
                        ad.Reference,
                        ad.Attachments,
                        ad.StartDate,
                        ad.SubmissionDate,
                        ad.IsActive,
                        cs.ClassName,
                        cs.SectionName
                    FROM AssignmentData ad
                    LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
                    WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                var assignmentList = new List<GetAllAssignmentsResponse>();

                var assignmentData = await multi.ReadAsync<dynamic>();
                var groupedData = assignmentData.GroupBy(ad => new
                {
                    ad.AssignmentID,
                    ad.AssignmentName,
                    ad.SubjectName,
                    ad.AssignmentType,
                    ad.Description,
                    ad.Reference,
                    ad.Attachments,
                    ad.StartDate,
                    ad.SubmissionDate,
                    ad.IsActive
                });

                foreach (var group in groupedData)
                {
                    var assignmentResponse = new GetAllAssignmentsResponse
                    {
                        AssignmentID = group.Key.AssignmentID,
                        AssignmentName = group.Key.AssignmentName,
                        SubjectName = group.Key.SubjectName,
                        AssignmentType = group.Key.AssignmentType,
                        Description = group.Key.Description,
                        Reference = group.Key.Reference,
                        Attachments = group.Key.Attachments,
                        StartDate = group.Key.StartDate,
                        SubmissionDate = group.Key.SubmissionDate,
                        IsActive = group.Key.IsActive,
                        ClassSections = group.Select(g => new ClassSectionResponse
                        {
                            AssignmentID = g.AssignmentID,
                            ClassName = g.ClassName,
                            SectionName = g.SectionName
                        }).ToList()
                    };

                    assignmentList.Add(assignmentResponse);
                }

                return new ServiceResponse<List<GetAllAssignmentsResponse>>(assignmentList, true, "Assignments retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving assignments.");
                return new ServiceResponse<List<GetAllAssignmentsResponse>>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<Assignment>> GetAssignmentById(int id)
        {
            try
            {
                var sql = @"
                    SELECT 
                        a.AssignmentID,
                        a.AssignmentName,
                        a.IsClasswise,
                        a.IsStudentwise,
                        a.ClassID,
                        a.SectionID,
                        a.StudentID,
                        a.SubjectID,
                        s.subject_name AS SubjectName,
                        a.AssignmentTypeID,
                        at.AssignmentType,
                        a.StartDate,
                        a.SubmissionDate,
                        a.Description,
                        a.Reference,
                        a.Attachments,
                        a.InstituteID,
                        a.IsActive
                    FROM tblAssignment a
                    INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
                    INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
                    WHERE a.AssignmentID = @AssignmentID;

                    SELECT 
                        acs.AssignmentID,
                        acs.ClassID,
                        c.class_name AS ClassName,
                        acs.SectionID,
                        sec.section_name AS SectionName
                    FROM tblAssignmentClassSection acs
                    INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
                    INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
                    WHERE acs.AssignmentID = @AssignmentID;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new { AssignmentID = id });

                var assignment = await multi.ReadSingleOrDefaultAsync<Assignment>();
                if (assignment != null)
                {
                    assignment.ClassSections = (await multi.ReadAsync<AssignmentClassSection>()).ToList();
                }

                return new ServiceResponse<Assignment>(assignment, assignment != null, assignment != null ? "Assignment found." : "Assignment not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving assignment by ID.");
                return new ServiceResponse<Assignment>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAssignment(int id)
        {
            try
            {
                var sql = @"UPDATE tblAssignment SET IsActive = 0 WHERE AssignmentID = @AssignmentID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { AssignmentID = id });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0, rowsAffected > 0 ? "Assignment deleted successfully." : "Delete operation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting assignment.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message);
            }
        }
    }
}
