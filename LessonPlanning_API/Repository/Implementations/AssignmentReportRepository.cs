using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.Repository.Interfaces;
using System.Data;
using System.Globalization;

namespace Lesson_API.Repository.Implementations
{
    public class AssignmentReportRepository : IAssignmentReportRepository
    {
        private readonly IDbConnection _dbConnection;

        public AssignmentReportRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<GetAssignmentsReportsResponse>> GetAssignmentsReports(GetAssignmentsReportsRequest request)
        {
            try
            {
                DateTime startDate, endDate;

                if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    throw new Exception("Invalid StartDate format. Please use 'dd-MM-yyyy'.");
                }

                if (!DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    throw new Exception("Invalid EndDate format. Please use 'dd-MM-yyyy'.");
                }

                // Define the SQL query for fetching assignments with Class, Section, and Student data
                string sql = @"
        WITH AssignmentData AS (
            SELECT 
                a.AssignmentID,
                a.AssignmentName,
                s.subject_name AS SubjectName,
                at.AssignmentType,
                a.Description,
                a.Reference, 
                a.StartDate,
                a.SubmissionDate,
                a.IsActive,
                a.CreatedBy,
                a.CreatedOn,
                a.IsClasswise,
                a.IsStudentwise,
                ROW_NUMBER() OVER (ORDER BY a.AssignmentID) AS RowNum
            FROM tblAssignment a
            INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
            INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
            WHERE a.InstituteID = @InstituteID 
                AND a.IsActive = 1
                AND a.StartDate >= @StartDate
                AND a.SubmissionDate <= @EndDate
                AND (@SearchText IS NULL OR a.AssignmentName LIKE '%' + @SearchText + '%' 
                    OR s.subject_name LIKE '%' + @SearchText + '%' 
                    OR at.AssignmentType LIKE '%' + @SearchText + '%')
                AND (
                    (a.IsClasswise = 1 AND @TypeWise = 1) OR 
                    (a.IsStudentwise = 1 AND @TypeWise = 2) OR 
                    @TypeWise = 0
                )
        ), ClassSectionData AS (
            SELECT 
                acs.AssignmentID,
                c.class_name AS ClassName,
                sec.section_name AS SectionName
            FROM tblAssignmentClassSection acs
            INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
            INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
            WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
        ), EmployeeData AS (
            SELECT 
                e.Employee_id,
                e.First_Name,
                e.Last_Name
            FROM tbl_EmployeeProfileMaster e WHERE e.Institute_id = @InstituteID
        ),
        StudentData AS (
            SELECT 
                s.student_id AS StudentID,
                CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                asg.AssignmentID
            FROM tbl_StudentMaster s
            INNER JOIN tblAssignmentStudent asg ON s.student_id = asg.StudentID
            WHERE s.Institute_id = @InstituteID
        )
        SELECT 
            ad.AssignmentID,
            ad.AssignmentName,
            ad.SubjectName,
            ad.AssignmentType,
            ad.Description,
            ad.Reference, 
            FORMAT(ad.StartDate, 'dd-MM-yyyy') AS StartDate,
            FORMAT(ad.SubmissionDate, 'dd-MM-yyyy') AS SubmissionDate,
            ad.IsActive,
            CONCAT(ed.First_Name, ' ', ed.Last_Name) AS CreatedBy,
            FORMAT(ad.CreatedOn, 'dd-MM-yyyy hh:mm tt') AS CreatedOnFormatted,
            cs.ClassName,
            cs.SectionName,
            sd.StudentName
        FROM AssignmentData ad
        LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
        LEFT JOIN EmployeeData ed ON ad.CreatedBy = ed.Employee_id
        LEFT JOIN StudentData sd ON ad.AssignmentID = sd.AssignmentID
        WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                // Fetch assignment data from the database
                var assignmentRecords = await _dbConnection.QueryAsync<dynamic>(sql, new
                {
                    request.InstituteID,
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchText = request.SearchText,
                    TypeWise = request.TypeWise,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // Group assignments by AssignmentID to eliminate duplicates and aggregate class sections/students
                var groupedAssignments = assignmentRecords
                    .GroupBy(ad => ad.AssignmentID)
                    .Select(group => new GetAssignmentsReportsResponse
                    {
                        AssignmentID = group.Key,
                        AssignmentName = group.First().AssignmentName,
                        SubjectName = group.First().SubjectName,
                        AssignmentType = group.First().AssignmentType,
                        Description = group.First().Description,
                        Reference = group.First().Reference,
                        StartDate = group.First().StartDate,
                        SubmissionDate = group.First().SubmissionDate,
                        IsActive = group.First().IsActive,
                        CreatedBy = group.First().CreatedBy,
                        CreatedOn = group.First().CreatedOnFormatted,

                        // Conditionally include class sections or students based on TypeWise
                        ClassSections = request.TypeWise == 1 ?
                            group.Select(g => new ClassSectionReport
                            {
                                ClassName = g.ClassName,
                                SectionName = g.SectionName
                            }).Distinct().ToList() : null,

                        Students = request.TypeWise == 2 ?
                            group.Select(g => new StudentReport
                            {
                                StudentName = g.StudentName
                            }).Distinct().ToList() : null,
                    }).ToList();

                return groupedAssignments;
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                throw new Exception($"Error while fetching assignment reports: {ex.Message}");
            }
        }


        //public async Task<List<GetAssignmentsReportsResponse>> GetAssignmentsReports(GetAssignmentsReportsRequest request)
        //{
        //    try
        //    {
        //        DateTime startDate, endDate;

        //        if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
        //        {
        //            throw new Exception("Invalid StartDate format. Please use 'dd-MM-yyyy'.");
        //        }

        //        if (!DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        //        {
        //            throw new Exception("Invalid EndDate format. Please use 'dd-MM-yyyy'.");
        //        }

        //        // Define the SQL query for fetching assignments with Class, Section, and Student data
        //        string sql = @"
        //WITH AssignmentData AS (
        //    SELECT 
        //        a.AssignmentID,
        //        a.AssignmentName,
        //        s.subject_name AS SubjectName,
        //        at.AssignmentType,
        //        a.Description,
        //        a.Reference, 
        //        a.StartDate,
        //        a.SubmissionDate,
        //        a.IsActive,
        //        a.CreatedBy,
        //        a.CreatedOn,
        //        a.IsClasswise,
        //        a.IsStudentwise,
        //        ROW_NUMBER() OVER (ORDER BY a.AssignmentID) AS RowNum
        //    FROM tblAssignment a
        //    INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
        //    INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
        //    WHERE a.InstituteID = @InstituteID 
        //        AND a.IsActive = 1
        //        AND a.StartDate >= @StartDate
        //        AND a.SubmissionDate <= @EndDate
        //        AND (@SearchText IS NULL OR a.AssignmentName LIKE '%' + @SearchText + '%' 
        //            OR s.subject_name LIKE '%' + @SearchText + '%' 
        //            OR at.AssignmentType LIKE '%' + @SearchText + '%')
        //        AND (
        //            (a.IsClasswise = 1 AND @TypeWise = 1) OR 
        //            (a.IsStudentwise = 1 AND @TypeWise = 2) OR 
        //            @TypeWise = 0
        //        )
        //), ClassSectionData AS (
        //    SELECT 
        //        acs.AssignmentID,
        //        c.class_name AS ClassName,
        //        sec.section_name AS SectionName
        //    FROM tblAssignmentClassSection acs
        //    INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
        //    INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
        //    WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
        //), EmployeeData AS (
        //    SELECT 
        //        e.Employee_id,
        //        e.First_Name,
        //        e.Last_Name
        //    FROM tbl_EmployeeProfileMaster e WHERE e.Institute_id = @InstituteID
        //),
        //StudentData AS (
        //    SELECT 
        //        s.student_id AS StudentID,
        //        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName
        //    FROM tbl_StudentMaster s
        //    WHERE s.Institute_id = @InstituteID
        //)
        //SELECT 
        //    ad.AssignmentID,
        //    ad.AssignmentName,
        //    ad.SubjectName,
        //    ad.AssignmentType,
        //    ad.Description,
        //    ad.Reference, 
        //    FORMAT(ad.StartDate, 'dd-MM-yyyy') AS StartDate,
        //    FORMAT(ad.SubmissionDate, 'dd-MM-yyyy') AS SubmissionDate,
        //    ad.IsActive,
        //    CONCAT(ed.First_Name, ' ', ed.Last_Name) AS CreatedBy,
        //    FORMAT(ad.CreatedOn, 'dd-MM-yyyy hh:mm tt') AS CreatedOnFormatted,
        //    cs.ClassName,
        //    cs.SectionName,
        //    sd.StudentName
        //FROM AssignmentData ad
        //LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
        //LEFT JOIN EmployeeData ed ON ad.CreatedBy = ed.Employee_id
        //LEFT JOIN StudentData sd ON ad.AssignmentID = sd.AssignmentID
        //WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

        //        // Fetch assignment data from database
        //        var assignmentRecords = await _dbConnection.QueryAsync<dynamic>(sql, new
        //        {
        //            request.InstituteID,
        //            StartDate = startDate,
        //            EndDate = endDate,
        //            SearchText = request.SearchText,
        //            TypeWise = request.TypeWise,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize
        //        });

        //        // Group assignments by AssignmentID to eliminate duplicates
        //        var groupedAssignments = assignmentRecords
        //            .GroupBy(ad => ad.AssignmentID)
        //            .Select(group => new GetAssignmentsReportsResponse
        //            {
        //                AssignmentID = group.Key,
        //                AssignmentName = group.First().AssignmentName,
        //                SubjectName = group.First().SubjectName,
        //                AssignmentType = group.First().AssignmentType,
        //                Description = group.First().Description,
        //                Reference = group.First().Reference,
        //                StartDate = group.First().StartDate,
        //                SubmissionDate = group.First().SubmissionDate,
        //                IsActive = group.First().IsActive,
        //                CreatedBy = group.First().CreatedBy,
        //                CreatedOn = group.First().CreatedOnFormatted,
        //                // Conditionally include class sections or students based on TypeWise
        //                ClassSections = request.TypeWise == 1 ?
        //                    group.Select(g => new ClassSectionReport
        //                    {
        //                        ClassName = g.ClassName,
        //                        SectionName = g.SectionName
        //                    }).Distinct().ToList() : null,

        //                Students = request.TypeWise == 2 ?
        //                    group.Select(g => new StudentReport
        //                    {
        //                        StudentName = g.StudentName
        //                    }).Distinct().ToList() : null,
        //            }).ToList();

        //        return groupedAssignments;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception appropriately
        //        throw new Exception($"Error while fetching assignment reports: {ex.Message}");
        //    }
        //}


        public async Task<IEnumerable<GetAssignmentsReportsExportResponse>> GetAssignmentsReportsExport(GetAssignmentsReportsExportRequest request)
        {
            // Convert the startDate and endDate into the correct format
            DateTime startDate, endDate;

            if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                throw new Exception("Invalid StartDate format. Please use 'dd-MM-yyyy'.");
            }

            if (!DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                throw new Exception("Invalid EndDate format. Please use 'dd-MM-yyyy'.");
            }
             


            //SQL query for fetching assignments data with class sections and students
                    string query = @"
            WITH AssignmentData AS(
    SELECT
        a.AssignmentID,
        a.AssignmentName,
        s.subject_name AS SubjectName,
        at.AssignmentType,
        a.Description,
        a.Reference,
        a.StartDate,
        a.SubmissionDate,
        a.IsActive,
        a.CreatedBy,
        a.CreatedOn,
        a.IsClasswise,
        a.IsStudentwise,
        ROW_NUMBER() OVER(ORDER BY a.AssignmentID) AS RowNum
    FROM tblAssignment a
    INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
    INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
    WHERE a.InstituteID = @InstituteID
        AND a.IsActive = 1
        AND a.StartDate >= @StartDate
        AND a.SubmissionDate <= @EndDate
        AND(@SearchText IS NULL OR a.AssignmentName LIKE '%' + @SearchText + '%'
            OR s.subject_name LIKE '%' + @SearchText + '%'
            OR at.AssignmentType LIKE '%' + @SearchText + '%')
        AND(
            (a.IsClasswise = 1 AND @TypeWise = 1) OR
            (a.IsStudentwise = 1 AND @TypeWise = 2) OR
            @TypeWise = 0
        )
), ClassSectionData AS(
    SELECT
        acs.AssignmentID,
        c.class_name AS ClassName,
        sec.section_name AS SectionName
    FROM tblAssignmentClassSection acs
    INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
    INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
    WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset +1 AND @Offset +@PageSize)
), EmployeeData AS(
    SELECT
        e.Employee_id,
        e.First_Name,
        e.Last_Name
    FROM tbl_EmployeeProfileMaster e
    WHERE e.Institute_id = @InstituteID
),
StudentData AS(
    SELECT
        s.student_id AS StudentID,
        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
        asg.AssignmentID
    FROM tbl_StudentMaster s
    INNER JOIN tblAssignmentStudent asg ON s.student_id = asg.StudentID
    WHERE s.Institute_id = @InstituteID
)
SELECT
    ad.AssignmentID,
    ad.AssignmentName,
    ad.SubjectName,
    ad.AssignmentType,
    ad.Description,
    ad.Reference, 
    FORMAT(ad.StartDate, 'dd-MM-yyyy') AS StartDate,
    FORMAT(ad.SubmissionDate, 'dd-MM-yyyy') AS SubmissionDate,
    ad.IsActive,
    CONCAT(ed.First_Name, ' ', ed.Last_Name) AS CreatedBy,
    FORMAT(ad.CreatedOn, 'dd-MM-yyyy hh:mm tt') AS CreatedOn,
     --Aggregate class sections into a comma-separated list
    STRING_AGG(cs.ClassName + ' ' + cs.SectionName, ', ') AS ClassSections,
    -- Aggregate student names into a comma-separated list
    STRING_AGG(sd.StudentName, ', ') AS Students
FROM AssignmentData ad
LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
LEFT JOIN EmployeeData ed ON ad.CreatedBy = ed.Employee_id
LEFT JOIN StudentData sd ON ad.AssignmentID = sd.AssignmentID
WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize
GROUP BY
    ad.AssignmentID,
    ad.AssignmentName,
    ad.SubjectName,
    ad.AssignmentType,
    ad.Description,
    ad.Reference, 
    ad.StartDate,
    ad.SubmissionDate,
    ad.IsActive,
    ed.First_Name,
    ed.Last_Name,
    ad.CreatedOn;";



            // Execute the query and return the result
            var assignments = await _dbConnection.QueryAsync<GetAssignmentsReportsExportResponse>(query, new
            {
                request.InstituteID,
                StartDate = startDate.ToString("yyyy-MM-dd"), // Convert to yyyy-MM-dd format
                EndDate = endDate.ToString("yyyy-MM-dd"),     // Convert to yyyy-MM-dd format
                SearchText = request.SearchText,
                TypeWise = request.TypeWise,
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            });

            return assignments;
        }
    }
}
