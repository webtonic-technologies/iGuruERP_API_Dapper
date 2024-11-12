using System.Data.SqlClient;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Linq;
using System.IO;


namespace FeesManagement_API.Repository.Implementations
{
    public class ConcessionMappingRepository : IConcessionMappingRepository
    {
        private readonly IConfiguration _configuration;

        public ConcessionMappingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AddUpdateConcession(AddUpdateConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"IF EXISTS (SELECT 1 FROM tblStudentConcession 
                                 WHERE StudentID = @StudentID AND InstituteID = @InstituteID)
                      BEGIN
                          UPDATE tblStudentConcession 
                          SET ConcessionGroupID = @ConcessionGroupID
                          WHERE StudentID = @StudentID AND InstituteID = @InstituteID;
                      END
                      ELSE
                      BEGIN
                          INSERT INTO tblStudentConcession (StudentID, ConcessionGroupID, InstituteID, IsActive)
                          VALUES (@StudentID, @ConcessionGroupID, @InstituteID, 1);
                      END";
                connection.Execute(query, request);
                return "Success";
            }
        }


        //public List<GetAllConcessionMappingResponse> GetAllConcessionMapping(GetAllConcessionMappingRequest request)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"SELECT 
        //                sm.student_id AS StudentID,
        //                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
        //                c.class_name AS ClassName,
        //                s.section_name AS SectionName,
        //                sm.Admission_Number AS AdmissionNumber,
        //                ISNULL(sc.ConcessionGroupID, 0) AS ConcessionGroupID,
        //                ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType
        //              FROM tbl_StudentMaster sm
        //              INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        //              INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        //              LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1 
        //              LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
        //              WHERE sm.Institute_id = @InstituteID AND sm.class_id = @ClassID AND sm.section_id = @SectionID";

        //        return connection.Query<GetAllConcessionMappingResponse>(query, request).ToList();
        //    }
        //}

        public List<GetAllConcessionMappingResponse> GetAllConcessionMapping(GetAllConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                // Build the query with optional search conditions
                var query = @"
            SELECT 
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                c.class_name AS ClassName,
                s.section_name AS SectionName,
                sm.Admission_Number AS AdmissionNumber,
                ISNULL(sc.ConcessionGroupID, 0) AS ConcessionGroupID,
                ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType, sm.IsActive
            FROM tbl_StudentMaster sm
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1 
            LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
            WHERE sm.Institute_id = @InstituteID 
              AND sm.class_id = @ClassID 
              AND sm.section_id = @SectionID";

                // Add search conditions if Search is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    query += @"
                AND (sm.First_Name LIKE @Search OR sm.Middle_Name LIKE @Search OR sm.Last_Name LIKE @Search OR sm.Admission_Number LIKE @Search)";
                }

                return connection.Query<GetAllConcessionMappingResponse>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    Search = "%" + request.Search + "%"  // To perform a LIKE search
                }).ToList();
            }
        }


        public byte[] GetConcessionListExcel(GetAllConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
            SELECT 
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                c.class_name AS ClassName,
                s.section_name AS SectionName,
                sm.Admission_Number AS AdmissionNumber,
                ISNULL(sc.ConcessionGroupID, 0) AS ConcessionGroupID,
                ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType,
                sm.IsActive
            FROM tbl_StudentMaster sm
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1 
            LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
            WHERE sm.Institute_id = @InstituteID 
              AND sm.class_id = @ClassID 
              AND sm.section_id = @SectionID";

                if (!string.IsNullOrEmpty(request.Search))
                {
                    query += @"
            AND (sm.First_Name LIKE @Search OR sm.Middle_Name LIKE @Search OR sm.Last_Name LIKE @Search OR sm.Admission_Number LIKE @Search)";
                }

                var concessionList = connection.Query<GetAllConcessionMappingResponse>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    Search = "%" + request.Search + "%" // To perform a LIKE search
                }).ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Concession List");

                    worksheet.Cells[1, 1].Value = "StudentID";
                    worksheet.Cells[1, 2].Value = "StudentName";
                    worksheet.Cells[1, 3].Value = "ClassName";
                    worksheet.Cells[1, 4].Value = "SectionName";
                    worksheet.Cells[1, 5].Value = "AdmissionNumber";
                    worksheet.Cells[1, 6].Value = "ConcessionGroupType";  // Removed ConcessionGroupID
                    worksheet.Cells[1, 7].Value = "IsActive";

                    var row = 2;
                    foreach (var concession in concessionList)
                    {
                        worksheet.Cells[row, 1].Value = concession.StudentID;
                        worksheet.Cells[row, 2].Value = concession.StudentName;
                        worksheet.Cells[row, 3].Value = concession.ClassName;
                        worksheet.Cells[row, 4].Value = concession.SectionName;
                        worksheet.Cells[row, 5].Value = concession.AdmissionNumber;
                        worksheet.Cells[row, 6].Value = concession.ConcessionGroupType; // Removed ConcessionGroupID
                        worksheet.Cells[row, 7].Value = concession.IsActive ? "Yes" : "No";
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
        }

        public async Task<List<GetAllConcessionMappingResponse>> GetConcessionListForExport(GetAllConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
            SELECT 
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                c.class_name AS ClassName,
                s.section_name AS SectionName,
                sm.Admission_Number AS AdmissionNumber,
                ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType,
                sm.IsActive
            FROM tbl_StudentMaster sm
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1
            LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
            WHERE sm.Institute_id = @InstituteID 
            AND sm.class_id = @ClassID 
            AND sm.section_id = @SectionID";

                // Add search conditions if Search is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    query += @"
            AND (sm.First_Name LIKE @Search OR sm.Middle_Name LIKE @Search OR sm.Last_Name LIKE @Search OR sm.Admission_Number LIKE @Search)";
                }

                return (await connection.QueryAsync<GetAllConcessionMappingResponse>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    Search = "%" + request.Search + "%" // To perform a LIKE search
                })).ToList();
            }
        }


        public async Task<List<GetAllConcessionMappingResponse>> GetConcessionList(GetAllConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                    SELECT 
                        sm.student_id AS StudentID,
                        CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                        c.class_name AS ClassName,
                        s.section_name AS SectionName,
                        sm.Admission_Number AS AdmissionNumber,
                        ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType,
                        sm.IsActive
                    FROM tbl_StudentMaster sm
                    INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                    INNER JOIN tbl_Section s ON sm.section_id = s.section_id
                    LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1
                    LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
                    WHERE sm.Institute_id = @InstituteID 
                    AND sm.class_id = @ClassID 
                    AND sm.section_id = @SectionID";

                // Add search conditions if Search is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    query += @"
                    AND (sm.First_Name LIKE @Search OR sm.Middle_Name LIKE @Search OR sm.Last_Name LIKE @Search OR sm.Admission_Number LIKE @Search)";
                }

                return (await connection.QueryAsync<GetAllConcessionMappingResponse>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID,
                    Search = "%" + request.Search + "%" // To perform a LIKE search
                })).ToList();
            }
        }

        public string UpdateStatus(int studentConcessionID)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"UPDATE tblStudentConcession 
                              SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                              WHERE StudentConcessionID = @StudentConcessionID";
                connection.Execute(query, new { StudentConcessionID = studentConcessionID });
                return "Success";
            }
        }

        public async Task<IEnumerable<ConcessionListResponse>> GetConcessionList(int instituteID)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                    SELECT 
                        ConcessionGroupID, 
                        ConcessionGroupType
                    FROM tblConcessionGroup
                    WHERE InstituteID = @InstituteID AND IsActive = 1";

                return await connection.QueryAsync<ConcessionListResponse>(query, new { InstituteID = instituteID });
            }
        }
    }
}
