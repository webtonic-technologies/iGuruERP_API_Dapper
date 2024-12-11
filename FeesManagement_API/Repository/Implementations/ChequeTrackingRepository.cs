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

            DateTime startDate;
            DateTime endDate;

            try
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null); // Parse the StartDate
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null); // Parse the EndDate
            }
            catch (FormatException)
            {
                response.Success = false;
                response.Message = "Invalid date format. Use 'DD-MM-YYYY'.";
                return response;
            }

            string query = @"
            SELECT DISTINCT
                s.student_id AS StudentID,
                CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                s.Admission_Number AS AdmissionNo,
                c.Class_Name AS ClassName,
                sec.Section_Name AS SectionName,
                s.Roll_Number AS RollNo,
                t.ChequeNo,
                t.PaymentAmount AS Amount,
                t.ChequeBankName as BankName,
                t.ChequeDate
            FROM 
                tblStudentFeePaymentTransaction t
            INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))  -- Handle PaymentIDs as a comma-separated list
            INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID  -- Join using StudentID from tblStudentFeePayment
            INNER JOIN tbl_Class c ON c.class_id = sp.ClassID  -- Use ClassID from tblStudentFeePayment
            INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID  -- Use SectionID from tblStudentFeePayment
            WHERE 
                t.ChequeStatusID = @ChequeStatusID AND 
                t.ChequeDate BETWEEN @StartDate AND @EndDate
                AND (@Search IS NULL OR 
                     s.Admission_Number LIKE '%' + @Search + '%' OR 
                     t.ChequeNo LIKE '%' + @Search + '%' OR 
                     CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%');"; // Search condition

            var chequeTrackings = _connection.Query<ChequeTrackingResponse>(query, new
            {
                request.ChequeStatusID,
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search // Pass null if search is empty
            }).ToList();

            response.TotalCount = chequeTrackings.Count; // Set the TotalCount

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


        public ServiceResponse<List<GetChequeTrackingStatusResponse>> GetChequeTrackingStatus()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            var response = new ServiceResponse<List<GetChequeTrackingStatusResponse>>(true, "Cheque statuses retrieved successfully", new List<GetChequeTrackingStatusResponse>(), 200);

            string query = "SELECT ChequeStatusID, ChequeStatus FROM tblChequeStatus";

            var chequeStatuses = _connection.Query<GetChequeTrackingStatusResponse>(query).ToList();

            if (chequeStatuses.Any())
            {
                response.Data = chequeStatuses;
            }
            else
            {
                response.Success = false;
                response.Message = "No cheque statuses found.";
            }

            return response;
        }



        public DataTable GetChequeTrackingExportData(ChequeTrackingExportRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null); // Parse the StartDate
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null); // Parse the EndDate

            string query = @"
    SELECT DISTINCT
        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
        s.Admission_Number AS AdmissionNo,
        c.Class_Name AS ClassName,
        sec.Section_Name AS SectionName,
        s.Roll_Number AS RollNo,
        t.ChequeNo,
        t.PaymentAmount AS Amount,
        t.ChequeBankName as BankName,
        t.ChequeDate
    FROM 
        tblStudentFeePaymentTransaction t
    INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))  
    INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID  
    INNER JOIN tbl_Class c ON c.class_id = sp.ClassID  
    INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID  
    WHERE 
        t.ChequeStatusID = @ChequeStatusID AND 
        t.ChequeDate BETWEEN @StartDate AND @EndDate
        AND (@Search IS NULL OR 
             s.Admission_Number LIKE '%' + @Search + '%' OR 
             t.ChequeNo LIKE '%' + @Search + '%' OR 
             CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%');";

            var parameters = new
            {
                request.ChequeStatusID,
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            };

            var dataTable = new DataTable();
            using (var reader = _connection.ExecuteReader(query, parameters))
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

    }
}
