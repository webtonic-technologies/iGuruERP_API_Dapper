using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FeesManagement_API.Repository.Implementations
{
    public class ChequeTrackingRepository : IChequeTrackingRepository
    {
        private readonly IDbConnection _connection;

        public ChequeTrackingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            var response = new ServiceResponse<List<ChequeTrackingResponse>>(true, "Cheque tracking retrieved successfully", new List<ChequeTrackingResponse>(), 200);

            string query = @"
        SELECT DISTINCT
            s.student_id,
            CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
            s.Admission_Number AS AdmissionNo,
            c.Class_Name AS ClassName,
            sec.Section_Name AS SectionName,
            s.Roll_Number AS RollNo,
            t.ChequeNo,
            t.PaymentAmount,
            t.ChequeBankName,
            t.ChequeDate
        FROM 
            tblStudentFeePaymentTransaction t
        INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))  -- Handle PaymentIDs as a comma-separated list
        INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID  -- Join using StudentID from tblStudentFeePayment
        INNER JOIN tbl_Class c ON c.class_id = sp.ClassID  -- Use ClassID from tblStudentFeePayment
        INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID  -- Use SectionID from tblStudentFeePayment
        WHERE 
            t.ChequeStatusID = @ChequeStatusID AND 
            t.ChequeDate BETWEEN @StartDate AND @EndDate;";

            var chequeTrackings = _connection.Query<ChequeTrackingResponse>(query, new
            {
                request.ChequeStatusID,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            }).ToList();

            if (chequeTrackings.Any())
            {
                response.Data = chequeTrackings;
            }
            else
            {
                response.Success = false;
                response.Message = "No cheque tracking records found.";
            }

            return response;
        }
    }
}
