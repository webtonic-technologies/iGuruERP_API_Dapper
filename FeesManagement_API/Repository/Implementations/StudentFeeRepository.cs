using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeesManagement_API.Repository.Implementations
{
    public class StudentFeeRepository : IStudentFeeRepository
    {
        private readonly IConfiguration _configuration;

        public StudentFeeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                SELECT 
                    sm.Admission_Number AS AdmissionNo,
                    CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Roll_Number AS RollNo,
                    c.class_name AS ClassName,
                    s.section_name AS SectionName,
                    CAST(fh.FeeHeadID AS int) AS FeeHeadID,
                    fh.FeeHead,
                    CASE 
                        WHEN fg.FeeTenurityID = 1 THEN 'Single'
                        WHEN fg.FeeTenurityID = 2 THEN tt.TermName
                        WHEN fg.FeeTenurityID = 3 THEN tm.Month
                        ELSE 'N/A'
                    END AS FeeType,
                    COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount
                FROM tbl_StudentMaster sm
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_Section s ON sm.section_id = s.section_id
                INNER JOIN tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
                INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
                LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
                LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
                LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
                WHERE sm.class_id = @ClassID 
                  AND sm.section_id = @SectionID
                  AND sm.Institute_id = @InstituteID
                  AND (@Search IS NULL OR 
                       sm.Admission_Number LIKE '%' + @Search + '%' OR
                       CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
                ORDER BY sm.Admission_Number;";

                var result = connection.Query<StudentFeeData>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    request.Search
                }).ToList();

                // Group the result by student information and map to StudentFeeResponse
                var response = result.GroupBy(x => new
                {
                    x.AdmissionNo,
                    x.StudentName,
                    x.RollNo,
                    x.ClassName,
                    x.SectionName
                }).Select(group => new StudentFeeResponse
                {
                    AdmissionNo = group.Key.AdmissionNo,
                    StudentName = group.Key.StudentName,
                    RollNo = group.Key.RollNo,
                    ClassName = group.Key.ClassName,
                    SectionName = group.Key.SectionName,
                    TotalFeeAmount = group.Sum(x => x.FeeAmount), // Sum up the fee amounts
                    FeeDetails = group.Select(x => new StudentFeeDetail
                    {
                        FeeHead = x.FeeHead,
                        TenureType = x.FeeType,
                        Amount = x.FeeAmount
                    }).ToList()
                }).ToList();

                return response;
            }
        }


        //public List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"SELECT 
        //                sm.Admission_Number AS AdmissionNo,
        //                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
        //                sm.Roll_Number AS RollNo,
        //                c.class_name AS ClassName,
        //                s.section_name AS SectionName,
        //                CAST(fh.FeeHeadID AS int) AS FeeHeadID,
        //                fh.FeeHead,
        //                CASE 
        //                    WHEN fg.FeeTenurityID = 1 THEN 'Single'
        //                    WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        //                    WHEN fg.FeeTenurityID = 3 THEN tm.Month
        //                    ELSE 'N/A'
        //                END AS FeeType,
        //                COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount
        //            FROM tbl_StudentMaster sm
        //            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        //            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        //            INNER JOIN tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
        //            INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
        //            INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
        //            LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
        //            LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
        //            LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
        //            WHERE sm.class_id = @ClassID 
        //              AND sm.section_id = @SectionID
        //              AND sm.Institute_id = @InstituteID
        //            ORDER BY sm.Admission_Number;";

        //        var result = connection.Query<StudentFeeData>(query, new
        //        {
        //            request.ClassID,
        //            request.SectionID,
        //            request.InstituteID
        //        }).ToList();

        //        // Group the result by student information and map to StudentFeeResponse
        //        var response = result.GroupBy(x => new
        //        {
        //            x.AdmissionNo,
        //            x.StudentName,
        //            x.RollNo,
        //            x.ClassName,
        //            x.SectionName
        //        }).Select(group => new StudentFeeResponse
        //        {
        //            AdmissionNo = group.Key.AdmissionNo,
        //            StudentName = group.Key.StudentName,
        //            RollNo = group.Key.RollNo,
        //            ClassName = group.Key.ClassName,
        //            SectionName = group.Key.SectionName,
        //            TotalFeeAmount = group.Sum(x => x.FeeAmount), // Sum up the fee amounts
        //            FeeDetails = group.Select(x => new StudentFeeDetail
        //            {
        //                FeeHead = x.FeeHead,
        //                TenureType = x.FeeType,
        //                Amount = x.FeeAmount
        //            }).ToList()
        //        }).ToList();

        //        return response;
        //    }
        //}
    }
}
