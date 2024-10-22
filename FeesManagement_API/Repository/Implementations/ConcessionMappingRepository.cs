using System.Data.SqlClient;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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


        public List<GetAllConcessionMappingResponse> GetAllConcessionMapping(GetAllConcessionMappingRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"SELECT 
                        sm.student_id AS StudentID,
                        CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                        c.class_name AS ClassName,
                        s.section_name AS SectionName,
                        sm.Admission_Number AS AdmissionNumber,
                        ISNULL(sc.ConcessionGroupID, 0) AS ConcessionGroupID,
                        ISNULL(cg.ConcessionGroupType, 'None') AS ConcessionGroupType
                      FROM tbl_StudentMaster sm
                      INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                      INNER JOIN tbl_Section s ON sm.section_id = s.section_id
                      LEFT JOIN tblStudentConcession sc ON sm.student_id = sc.StudentID AND sc.InstituteID = @InstituteID AND sc.IsActive = 1 
                      LEFT JOIN tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
                      WHERE sm.Institute_id = @InstituteID AND sm.class_id = @ClassID AND sm.section_id = @SectionID";

                return connection.Query<GetAllConcessionMappingResponse>(query, request).ToList();
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
    }
}
