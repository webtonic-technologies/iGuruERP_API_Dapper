using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class StudentInformationRepository : IStudentInformationRepository
    {
        private readonly IDbConnection _connection;

        public StudentInformationRepository(IDbConnection connection)
        {
            _connection = connection;
        }
         
        public ServiceResponse<List<StudentInformationResponse>> GetStudentInformation(StudentInformationRequest request)
        {
            var query = @"
        SELECT 
            sm.student_id AS StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            c.class_name AS ClassName,
            s.section_name AS SectionName,
            sm.Roll_Number AS RollNo,
            sm.Admission_Number AS AdmissionNo,
            CONCAT(fa.First_Name, ' ', fa.Last_Name) AS FatherName,
            fa.Mobile_Number AS PhoneNumber,
            cg.ConcessionGroupType AS ConcessionGroup,
            FORMAT(sm.Date_of_Joining, 'dd-MM-yyyy') AS DateOfJoining,
            CONCAT(ma.First_Name, ' ', ma.Last_Name) AS MotherName,
            '-' AS Hostel,
            '-' AS TransportionRoute
        FROM 
            tbl_StudentMaster sm
        LEFT JOIN 
            tbl_Class c ON sm.class_id = c.class_id
        LEFT JOIN 
            tbl_Section s ON sm.section_id = s.section_id
        LEFT JOIN 
            tbl_StudentParentsInfo fa ON fa.Student_id = sm.student_id AND fa.Parent_Type_id = 1 -- Father's info
        LEFT JOIN 
            tbl_StudentParentsInfo ma ON ma.Student_id = sm.student_id AND ma.Parent_Type_id = 2 -- Mother's info
        LEFT JOIN 
            tblStudentFeeConcessionMapping sc ON sc.StudentID = sm.student_id
        LEFT JOIN 
            tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
        WHERE 
            sm.institute_id = @InstituteID 
            AND (@StudentID IS NOT NULL AND sm.student_id = @StudentID 
                 OR (@Search IS NOT NULL AND (
                     sm.Admission_Number LIKE '%' + @Search + '%' OR 
                     CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')));";

            var studentInfo = _connection.Query<StudentInformationResponse>(
                query,
                new
                {
                    request.InstituteID,
                    request.StudentID,
                    request.Search
                });

            if (studentInfo == null)
            {
                return new ServiceResponse<List< StudentInformationResponse >>(false, "Student not found", null, 404);
            }

            return new ServiceResponse<List< StudentInformationResponse>>(true, "Student information retrieved successfully", studentInfo.ToList(), 200);
        }


        //public ServiceResponse<StudentInformationResponse> GetStudentInformation(StudentInformationRequest request)
        //{
        //    var query = @"
        //        SELECT 
        //            sm.student_id AS StudentID,
        //            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
        //            c.class_name AS ClassName,
        //            s.section_name AS SectionName,
        //            sm.Roll_Number AS RollNo,
        //            sm.Admission_Number AS AdmissionNo,
        //            CONCAT(fa.First_Name, ' ', fa.Last_Name) AS FatherName,
        //            fa.Mobile_Number AS PhoneNumber,
        //            cg.ConcessionGroupType AS ConcessionGroup,
        //            FORMAT(sm.Date_of_Joining, 'dd-MM-yyyy') AS DateOfJoining,
        //            CONCAT(ma.First_Name, ' ', ma.Last_Name) AS MotherName,
        //            '-' AS Hostel,
        //            '-' AS TransportionRoute
        //        FROM 
        //            tbl_StudentMaster sm
        //        LEFT JOIN 
        //            tbl_Class c ON sm.class_id = c.class_id
        //        LEFT JOIN 
        //            tbl_Section s ON sm.section_id = s.section_id
        //        LEFT JOIN 
        //            tbl_StudentParentsInfo fa ON fa.Student_id = sm.student_id AND fa.Parent_Type_id = 1 -- Father's info
        //        LEFT JOIN 
        //            tbl_StudentParentsInfo ma ON ma.Student_id = sm.student_id AND ma.Parent_Type_id = 2 -- Mother's info
        //        LEFT JOIN 
        //            tblStudentFeeConcessionMapping sc ON sc.StudentID = sm.student_id
        //        LEFT JOIN 
        //            tblConcessionGroup cg ON sc.ConcessionGroupID = cg.ConcessionGroupID
        //        WHERE 
        //            sm.student_id = @StudentID AND sm.institute_id = @InstituteID;";

        //    var studentInfo = _connection.QuerySingleOrDefault<StudentInformationResponse>(query, new { request.StudentID, request.InstituteID });

        //    if (studentInfo == null)
        //    {
        //        return new ServiceResponse<StudentInformationResponse>(false, "Student not found", null, 404);
        //    }

        //    return new ServiceResponse<StudentInformationResponse>(true, "Student information retrieved successfully", studentInfo, 200);
        //}
    }
}
