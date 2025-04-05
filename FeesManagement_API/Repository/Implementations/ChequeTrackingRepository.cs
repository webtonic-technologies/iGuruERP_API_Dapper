using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);       // Parse the EndDate
            }
            catch (FormatException)
            {
                response.Success = false;
                response.Message = "Invalid date format. Use 'DD-MM-YYYY'.";
                return response;
            }

            // Get total count of matching records (before paging)
            string countQuery = @"
    SELECT COUNT(*) FROM (
        SELECT DISTINCT
            s.student_id, t.ChequeNo
        FROM 
            tblStudentFeePaymentTransaction t
        INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
        INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
        INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
        INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
        WHERE 
            t.ChequeStatusID = 1 AND 
            t.ChequeDate BETWEEN @StartDate AND @EndDate
            AND (@Search IS NULL OR 
                 s.Admission_Number LIKE '%' + @Search + '%' OR 
                 t.ChequeNo LIKE '%' + @Search + '%' OR 
                 CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
    ) AS CountTable;
    ";

            var totalCount = _connection.ExecuteScalar<int>(countQuery, new
            {
                //request.ChequeStatusID,
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            });

            // Calculate offset based on the page number and page size
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Main query with paging using OFFSET and FETCH NEXT
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
    INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
    INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
    INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
    INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
    WHERE 
        t.ChequeStatusID = 1 AND 
        t.ChequeDate BETWEEN @StartDate AND @EndDate
        AND (@Search IS NULL OR 
             s.Admission_Number LIKE '%' + @Search + '%' OR 
             t.ChequeNo LIKE '%' + @Search + '%' OR 
             CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
    ORDER BY t.ChequeDate
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

            var chequeTrackings = _connection.Query<ChequeTrackingResponse>(query, new
            {
                //request.ChequeStatusID,
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                Offset = offset,
                PageSize = request.PageSize
            }).ToList();

            // Convert ChequeDate to "DD-MM-YYYY" format if needed
            foreach (var cheque in chequeTrackings)
            {
                if (DateTime.TryParse(cheque.ChequeDate, out DateTime parsedDate))
                {
                    cheque.ChequeDate = parsedDate.ToString("dd-MM-yyyy");
                }
            }

            response.TotalCount = totalCount; // Set the total number of matching records

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



        //public ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request)
        //{
        //    if (_connection.State != ConnectionState.Open)
        //    {
        //        _connection.Open();
        //    }

        //    var response = new ServiceResponse<List<ChequeTrackingResponse>>(true, "Cheque tracking retrieved successfully", new List<ChequeTrackingResponse>(), 200);

        //    DateTime startDate;
        //    DateTime endDate;

        //    try
        //    {
        //        startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null); // Parse the StartDate
        //        endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null); // Parse the EndDate
        //    }
        //    catch (FormatException)
        //    {
        //        response.Success = false;
        //        response.Message = "Invalid date format. Use 'DD-MM-YYYY'.";
        //        return response;
        //    }

        //    string query = @"
        //    SELECT DISTINCT
        //        s.student_id AS StudentID,
        //        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
        //        s.Admission_Number AS AdmissionNo,
        //        c.Class_Name AS ClassName,
        //        sec.Section_Name AS SectionName,
        //        s.Roll_Number AS RollNo,
        //        t.ChequeNo,
        //        t.PaymentAmount AS Amount,
        //        t.ChequeBankName as BankName,
        //        t.ChequeDate
        //    FROM 
        //        tblStudentFeePaymentTransaction t
        //    INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))  -- Handle PaymentIDs as a comma-separated list
        //    INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID  -- Join using StudentID from tblStudentFeePayment
        //    INNER JOIN tbl_Class c ON c.class_id = sp.ClassID  -- Use ClassID from tblStudentFeePayment
        //    INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID  -- Use SectionID from tblStudentFeePayment
        //    WHERE 
        //        t.ChequeStatusID = @ChequeStatusID AND 
        //        t.ChequeDate BETWEEN @StartDate AND @EndDate
        //        AND (@Search IS NULL OR 
        //             s.Admission_Number LIKE '%' + @Search + '%' OR 
        //             t.ChequeNo LIKE '%' + @Search + '%' OR 
        //             CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%');"; // Search condition

        //    var chequeTrackings = _connection.Query<ChequeTrackingResponse>(query, new
        //    {
        //        request.ChequeStatusID,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
        //    }).ToList();

        //    // Convert ChequeDate to "DD-MM-YYYY" format if needed
        //    foreach (var cheque in chequeTrackings)
        //    {
        //        if (DateTime.TryParse(cheque.ChequeDate, out DateTime parsedDate))
        //        {
        //            cheque.ChequeDate = parsedDate.ToString("dd-MM-yyyy");
        //        }
        //    }

        //    response.TotalCount = chequeTrackings.Count; // Set the TotalCount

        //    if (chequeTrackings.Any())
        //    {
        //        response.Data = chequeTrackings;
        //    }
        //    else
        //    {
        //        response.Success = false;
        //        response.Message = "No cheque tracking records found.";
        //    }

        //    return response;
        //}


        public ServiceResponse<List<GetChequeTrackingBouncedResponse>> GetChequeTrackingBounced(GetChequeTrackingBouncedRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            var response = new ServiceResponse<List<GetChequeTrackingBouncedResponse>>(
                true,
                "Bounced cheque tracking retrieved successfully",
                new List<GetChequeTrackingBouncedResponse>(),
                200
            );

            DateTime startDate;
            DateTime endDate;
            try
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                response.Success = false;
                response.Message = "Invalid date format. Use 'DD-MM-YYYY'.";
                return response;
            }

            // Count query to get the total number of matching bounced cheques
            string countQuery = @"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT
                        s.student_id, t.ChequeNo
                    FROM 
                        tblStudentFeePaymentTransaction t
                    INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
                    INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
                    INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
                    INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
                    INNER JOIN tblChequeBounceDetails cbd ON cbd.TransactionID = t.TransactionID
                    WHERE 
                        t.ChequeStatusID = 2 AND 
                        t.SysTransactionDate BETWEEN @StartDate AND @EndDate
                        AND (@Search IS NULL OR 
                             s.Admission_Number LIKE '%' + @Search + '%' OR 
                             t.ChequeNo LIKE '%' + @Search + '%' OR 
                             CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
                ) AS CountTable;
            ";

            var totalCount = _connection.ExecuteScalar<int>(countQuery, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            });

            int offset = (request.PageNumber - 1) * request.PageSize;

            // Main query with paging using OFFSET-FETCH
            string query = @"
                SELECT DISTINCT
                    s.student_id AS StudentID,
                    CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number AS AdmissionNo,
                    c.Class_Name AS ClassName,
                    sec.Section_Name AS SectionName,
                    s.Roll_Number AS RollNo,
                    t.ChequeNo,
                    CONVERT(varchar(10), t.SysTransactionDate, 105) AS BounceDate,
                    t.SysTransactionDate AS OrderDate,  -- Added for ordering
                    cbd.ChequeBounceCharges AS BounceCharges,
                    cbd.Reason
                FROM 
                    tblStudentFeePaymentTransaction t
                INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
                INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
                INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
                INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
                INNER JOIN tblChequeBounceDetails cbd ON cbd.TransactionID = t.TransactionID
                WHERE 
                    t.ChequeStatusID = 2 AND 
                    t.SysTransactionDate BETWEEN @StartDate AND @EndDate
                    AND (@Search IS NULL OR 
                         s.Admission_Number LIKE '%' + @Search + '%' OR 
                         t.ChequeNo LIKE '%' + @Search + '%' OR 
                         CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
                ORDER BY OrderDate
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";


            var bouncedCheques = _connection.Query<GetChequeTrackingBouncedResponse>(query, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                Offset = offset,
                PageSize = request.PageSize
            }).ToList();

            // (Optional) Ensure BounceDate is in "DD-MM-YYYY" format
            foreach (var bounce in bouncedCheques)
            {
                if (DateTime.TryParse(bounce.BounceDate, out DateTime parsedDate))
                {
                    bounce.BounceDate = parsedDate.ToString("dd-MM-yyyy");
                }
            }

            response.TotalCount = totalCount;
            if (bouncedCheques.Any())
            {
                response.Data = bouncedCheques;
            }
            else
            {
                response.Success = false;
                response.Message = "No bounced cheque records found.";
            }

            return response;
        }


        public ServiceResponse<List<GetChequeTrackingClearedResponse>> GetChequeTrackingCleared(GetChequeTrackingClearedRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            var response = new ServiceResponse<List<GetChequeTrackingClearedResponse>>(
                true,
                "Cleared cheque tracking retrieved successfully",
                new List<GetChequeTrackingClearedResponse>(),
                200
            );

            DateTime startDate;
            DateTime endDate;
            try
            {
                startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                response.Success = false;
                response.Message = "Invalid date format. Use 'DD-MM-YYYY'.";
                return response;
            }

            // Count query to get the total number of matching cleared cheques
            string countQuery = @"
         SELECT COUNT(*) FROM (
             SELECT DISTINCT
                 s.student_id, t.ChequeNo
             FROM 
                 tblStudentFeePaymentTransaction t
             INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
             INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
             INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
             INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
             INNER JOIN tblChequeClearanceDetails ccd ON ccd.TransactionID = t.TransactionID
             WHERE 
                 t.ChequeStatusID = 3 AND 
                 t.ChequeDate BETWEEN @StartDate AND @EndDate
                 AND (@Search IS NULL OR 
                      s.Admission_Number LIKE '%' + @Search + '%' OR 
                      t.ChequeNo LIKE '%' + @Search + '%' OR 
                      CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
         ) AS CountTable;
    ";

            var totalCount = _connection.ExecuteScalar<int>(countQuery, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            });

            int offset = (request.PageNumber - 1) * request.PageSize;

            // Main query with paging using OFFSET-FETCH.
            // We add t.ChequeDate as ChequeDateOrder to satisfy the ORDER BY requirement.
            string query = @"
         SELECT DISTINCT
             s.student_id AS StudentID,
             CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
             s.Admission_Number AS AdmissionNo,
             c.Class_Name AS ClassName,
             sec.Section_Name AS SectionName,
             s.Roll_Number AS RollNo,
             t.ChequeNo,
             CONVERT(varchar(10), t.ChequeDate, 105) AS ChequeDate,
             CONVERT(varchar(10), ccd.ChequeClearanceDate, 105) AS ChequeClearanceDate,
             t.PaymentAmount AS Amount,
             t.ChequeBankName AS BankName,
             t.ChequeDate AS ChequeDateOrder  -- Extra column for ordering
         FROM 
             tblStudentFeePaymentTransaction t
         INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
         INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
         INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
         INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
         INNER JOIN tblChequeClearanceDetails ccd ON ccd.TransactionID = t.TransactionID
         WHERE 
             t.ChequeStatusID = 3 AND 
             t.ChequeDate BETWEEN @StartDate AND @EndDate
             AND (@Search IS NULL OR 
                  s.Admission_Number LIKE '%' + @Search + '%' OR 
                  t.ChequeNo LIKE '%' + @Search + '%' OR 
                  CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
         ORDER BY ChequeDateOrder
         OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

            var clearedCheques = _connection.Query<GetChequeTrackingClearedResponse>(query, new
            {
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                Offset = offset,
                PageSize = request.PageSize
            }).ToList();

            response.TotalCount = totalCount;
            if (clearedCheques.Any())
            {
                response.Data = clearedCheques;
            }
            else
            {
                response.Success = false;
                response.Message = "No cleared cheque tracking records found.";
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
                --t.ChequeDate
                CONVERT(varchar(10), t.ChequeDate, 105) AS ChequeDate

            FROM 
                tblStudentFeePaymentTransaction t
            INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))  
            INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID  
            INNER JOIN tbl_Class c ON c.class_id = sp.ClassID  
            INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID  
            WHERE 
                t.ChequeStatusID = 1 AND 
                t.ChequeDate BETWEEN @StartDate AND @EndDate
                AND (@Search IS NULL OR 
                     s.Admission_Number LIKE '%' + @Search + '%' OR 
                     t.ChequeNo LIKE '%' + @Search + '%' OR 
                     CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%');";

            var parameters = new
            {
                //request.ChequeStatusID,
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

        public DataTable GetChequeTrackingBouncedExportData(ChequeTrackingExportBouncedRequest request)
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
                    CONVERT(varchar(10), t.SysTransactionDate, 105) AS BounceDate,
                    t.SysTransactionDate AS OrderDate,  -- Added for ordering
                    cbd.ChequeBounceCharges AS BounceCharges,
                    cbd.Reason
                FROM 
                    tblStudentFeePaymentTransaction t
                INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
                INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
                INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
                INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
                INNER JOIN tblChequeBounceDetails cbd ON cbd.TransactionID = t.TransactionID
                WHERE 
                    t.ChequeStatusID = 2 AND 
                    t.SysTransactionDate BETWEEN @StartDate AND @EndDate
                    AND (@Search IS NULL OR 
                         s.Admission_Number LIKE '%' + @Search + '%' OR 
                         t.ChequeNo LIKE '%' + @Search + '%' OR 
                         CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
                ORDER BY OrderDate;
            "; 

            var parameters = new
            { 
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

        public DataTable GetChequeTrackingClearedExportData(ChequeTrackingExportClearedRequest request)
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
                     CONVERT(varchar(10), t.ChequeDate, 105) AS ChequeDate,
                     CONVERT(varchar(10), ccd.ChequeClearanceDate, 105) AS ChequeClearanceDate,
                     t.PaymentAmount AS Amount,
                     t.ChequeBankName AS BankName,
                     t.ChequeDate AS ChequeDateOrder  -- Extra column for ordering
                 FROM 
                     tblStudentFeePaymentTransaction t
                 INNER JOIN tblStudentFeePayment sp ON sp.FeesPaymentID IN (SELECT value FROM STRING_SPLIT(t.PaymentIDs, ','))
                 INNER JOIN tbl_StudentMaster s ON s.student_id = sp.StudentID
                 INNER JOIN tbl_Class c ON c.class_id = sp.ClassID
                 INNER JOIN tbl_Section sec ON sec.section_id = sp.SectionID
                 INNER JOIN tblChequeClearanceDetails ccd ON ccd.TransactionID = t.TransactionID
                 WHERE 
                     t.ChequeStatusID = 3 AND 
                     t.ChequeDate BETWEEN @StartDate AND @EndDate
                     AND (@Search IS NULL OR 
                          s.Admission_Number LIKE '%' + @Search + '%' OR 
                          t.ChequeNo LIKE '%' + @Search + '%' OR 
                          CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
                 ORDER BY ChequeDateOrder;
            ";

            var parameters = new
            {
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


        public ServiceResponse<bool> AddChequeBounce(SubmitChequeBounceRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var insertQuery = @"
                        INSERT INTO tblChequeBounceDetails (TransactionID, ChequeBounceCharges, Reason)
                        VALUES (@TransactionID, @ChequeBounceCharges, @Reason);";

                    _connection.Execute(insertQuery, new
                    {
                        request.TransactionID,
                        request.ChequeBounceCharges,
                        request.Reason
                    }, transaction);

                    // Update ChequeStatusID to 2 (Cheque Bounced) based on TransactionID
                    var updateQuery = @"
                        UPDATE tblStudentFeePaymentTransaction
                        SET ChequeStatusID = 2 -- Cheque Bounced
                        WHERE TransactionID = @TransactionID;";

                    _connection.Execute(updateQuery, new { TransactionID = request.TransactionID }, transaction);

                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Cheque bounce added successfully and status updated", true, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }
        }

        public ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Validate and convert ChequeClearanceDate
                    if (!DateTime.TryParseExact(
                            request.ChequeClearanceDate,
                            "dd-MM-yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var parsedDate))
                    {
                        return new ServiceResponse<bool>(false, "Invalid date format. Use DD-MM-YYYY.", false, 400);
                    }

                    // Insert into tblChequeClearanceDetails
                    var insertQuery = @"
            INSERT INTO tblChequeClearanceDetails (TransactionID, ChequeClearanceDate, Remarks)
            VALUES (@TransactionID, @ChequeClearanceDate, @Remarks);";

                    _connection.Execute(insertQuery, new
                    {
                        request.TransactionID,
                        ChequeClearanceDate = parsedDate.ToString("yyyy-MM-dd"), // Store in database as YYYY-MM-DD
                        request.Remarks
                    }, transaction);

                    // Update ChequeStatusID in tblStudentFeePaymentTransaction
                    var updateQuery = @"
            UPDATE tblStudentFeePaymentTransaction
            SET ChequeStatusID = 3 -- Set status to 'Success'
            WHERE TransactionID = @TransactionID;";

                    _connection.Execute(updateQuery, new { TransactionID = request.TransactionID }, transaction);

                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Cheque clearance added successfully and status updated", true, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }
        }

    }
}
