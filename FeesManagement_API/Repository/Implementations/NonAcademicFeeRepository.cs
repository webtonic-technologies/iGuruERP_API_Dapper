﻿using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Globalization;

namespace FeesManagement_API.Repository.Implementations
{
    public class NonAcademicFeeRepository : INonAcademicFeeRepository
    {
        private readonly IDbConnection _connection;

        public NonAcademicFeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public string AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request)
        {
            if (!DateTime.TryParseExact(
                    request.PaymentDate,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedPaymentDate))
            {
                throw new ArgumentException("Invalid date format for PaymentDate. Use DD-MM-YYYY.");
            }

            var query = @"
        INSERT INTO tblNonAcademicFees (InstituteID, PayeeTypeID, ClassID, SectionID, StudentID, EmployeeID, PayeeName, PaymentDate, 
            FeeHeadID, FeeAmount, PaymentModeID, TransactionDetails, Remarks, IsActive)
        VALUES (@InstituteID, @PayeeTypeID, @ClassID, @SectionID, @StudentID, @EmployeeID, @PayeeName, @PaymentDate, 
            @FeeHeadID, @FeeAmount, @PaymentModeID, @TransactionDetails, @Remarks, @IsActive);
    ";

            _connection.Execute(query, new
            {
                request.InstituteID,
                request.PayeeTypeID,
                request.ClassID,
                request.SectionID,
                request.StudentID,
                request.EmployeeID,
                request.PayeeName,
                PaymentDate = parsedPaymentDate.ToString("yyyy-MM-dd"), // Convert to database format
                request.FeeHeadID,
                request.FeeAmount,
                request.PaymentModeID,
                request.TransactionDetails,
                request.Remarks,
                request.IsActive
            });

            return "Success";
        }

        //public List<GetNonAcademicFeeResponse> GetNonAcademicFee(GetNonAcademicFeeRequest request)
        //{
        //    // Parse the dates from string to DateTime
        //    DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var query = @"
        //    SELECT 
        //        naf.NonAcademicFeesID,
        //        naf.PayeeTypeID,
        //        pt.PayeeType,
        //        CASE 
        //            WHEN naf.PayeeTypeID = 1 THEN naf.StudentID
        //            WHEN naf.PayeeTypeID = 2 THEN naf.EmployeeID
        //            ELSE NULL
        //        END AS PayeeID,
        //        CASE 
        //            WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
        //            WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
        //            WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
        //            ELSE NULL
        //        END AS PayeeName,
        //        c.class_name AS ClassName,
        //        s.section_name AS SectionName,
        //        FORMAT(naf.PaymentDate, 'dd-MM-yyyy') AS Date,
        //        naf.FeeHeadID,
        //        fh.FeeHead,
        //        naf.FeeAmount,
        //        naf.PaymentModeID,
        //        pm.PaymentMode,
        //        naf.TransactionDetails,
        //        naf.Remarks
        //    FROM 
        //        tblNonAcademicFees naf
        //    LEFT JOIN 
        //        tblPayeeType pt ON naf.PayeeTypeID = pt.PayeeTypeID
        //    LEFT JOIN 
        //        tbl_StudentMaster sm ON naf.StudentID = sm.student_id
        //    LEFT JOIN 
        //        tbl_EmployeeProfileMaster emp ON naf.EmployeeID = emp.Employee_id
        //    LEFT JOIN 
        //        tbl_Class c ON naf.ClassID = c.class_id
        //    LEFT JOIN 
        //        tbl_Section s ON naf.SectionID = s.section_id
        //    LEFT JOIN 
        //        tblFeeHead fh ON naf.FeeHeadID = fh.FeeHeadID
        //    LEFT JOIN 
        //        tblPaymentMode pm ON naf.PaymentModeID = pm.PaymentModeID
        //    WHERE 
        //        naf.InstituteID = @InstituteID 
        //        AND naf.PayeeTypeID = @PayeeTypeID 
        //        AND naf.PaymentDate BETWEEN @StartDate AND @EndDate
        //        AND naf.IsActive = 1
        //        AND (@Search IS NULL OR 
        //             CASE 
        //                WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
        //                WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
        //                WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
        //                ELSE NULL
        //             END LIKE '%' + @Search + '%')";

        //    // Adjust the request object to pass DateTime
        //    var parameters = new
        //    {
        //        request.InstituteID,
        //        request.PayeeTypeID,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        Search = request.Search
        //    };

        //    return _connection.Query<GetNonAcademicFeeResponse>(query, parameters).ToList();
        //}

        public ServiceResponse<IEnumerable<GetNonAcademicFeeResponse>> GetNonAcademicFee(GetNonAcademicFeeRequest request)
        {
            // Parse the dates from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Prepare the common parameters
            var parameters = new
            {
                request.InstituteID,
                request.PayeeTypeID,
                StartDate = startDate,
                EndDate = endDate,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                Offset = offset,
                PageSize = request.PageSize
            };

            // Query to get the total count of records matching the filters
            var countQuery = @"
        SELECT COUNT(*)
        FROM tblNonAcademicFees naf
        LEFT JOIN tbl_StudentMaster sm ON naf.StudentID = sm.student_id
        LEFT JOIN tbl_EmployeeProfileMaster emp ON naf.EmployeeID = emp.Employee_id
        WHERE 
            naf.InstituteID = @InstituteID 
            AND naf.PayeeTypeID = @PayeeTypeID 
            AND naf.PaymentDate BETWEEN @StartDate AND @EndDate
            AND naf.IsActive = 1
            AND (@Search IS NULL OR 
                 CASE 
                    WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
                    WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                    WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
                    ELSE NULL
                 END LIKE '%' + @Search + '%')";

            int totalCount = _connection.ExecuteScalar<int>(countQuery, parameters);

            // Main query with paging
            var query = @"
        SELECT 
            naf.NonAcademicFeesID,
            naf.PayeeTypeID,
            pt.PayeeType,
            CASE 
                WHEN naf.PayeeTypeID = 1 THEN naf.StudentID
                WHEN naf.PayeeTypeID = 2 THEN naf.EmployeeID
                ELSE NULL
            END AS PayeeID,
            CASE 
                WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
                WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
                ELSE NULL
            END AS PayeeName,
            c.class_name AS ClassName,
            s.section_name AS SectionName,
            FORMAT(naf.PaymentDate, 'dd-MM-yyyy') AS Date,
            naf.FeeHeadID,
            fh.FeeHead,
            naf.FeeAmount,
            naf.PaymentModeID,
            pm.PaymentMode,
            naf.TransactionDetails,
            naf.Remarks
        FROM tblNonAcademicFees naf
        LEFT JOIN tblPayeeType pt ON naf.PayeeTypeID = pt.PayeeTypeID
        LEFT JOIN tbl_StudentMaster sm ON naf.StudentID = sm.student_id
        LEFT JOIN tbl_EmployeeProfileMaster emp ON naf.EmployeeID = emp.Employee_id
        LEFT JOIN tbl_Class c ON naf.ClassID = c.class_id
        LEFT JOIN tbl_Section s ON naf.SectionID = s.section_id
        LEFT JOIN tblFeeHead fh ON naf.FeeHeadID = fh.FeeHeadID
        LEFT JOIN tblPaymentMode pm ON naf.PaymentModeID = pm.PaymentModeID
        WHERE 
            naf.InstituteID = @InstituteID 
            AND naf.PayeeTypeID = @PayeeTypeID 
            AND naf.PaymentDate BETWEEN @StartDate AND @EndDate
            AND naf.IsActive = 1
            AND (@Search IS NULL OR 
                 CASE 
                    WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
                    WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                    WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
                    ELSE NULL
                 END LIKE '%' + @Search + '%')
        ORDER BY naf.NonAcademicFeesID
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            var feeResponses = _connection.Query<GetNonAcademicFeeResponse>(query, parameters).ToList();

            return new ServiceResponse<IEnumerable<GetNonAcademicFeeResponse>>(
                true,
                "NonAcademicFees retrieved successfully",
                feeResponses,
                200,
                totalCount
            );
        }




        public string DeleteNonAcademicFee(int nonAcademicFeesID)
        {
            // Check if the ID exists before trying to update
            var checkQuery = "SELECT COUNT(1) FROM tblNonAcademicFees WHERE NonAcademicFeesID = @NonAcademicFeesID";
            var exists = _connection.ExecuteScalar<int>(checkQuery, new { NonAcademicFeesID = nonAcademicFeesID });

            if (exists == 0)
            {
                throw new Exception("The specified NonAcademicFeesID does not exist.");
            }

            var query = @"UPDATE tblNonAcademicFees SET IsActive = 0 WHERE NonAcademicFeesID = @NonAcademicFeesID";
            _connection.Execute(query, new { NonAcademicFeesID = nonAcademicFeesID });

            return "Success";
        }

        public DataTable GetNonAcademicFeeExportData(GetNonAcademicFeeExportRequest request)
        {
            var query = @"
            SELECT 
                pt.PayeeType,
                CASE 
                    WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
                    WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                    WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
                    ELSE NULL
                END AS PayeeName,
                c.class_name AS ClassName,
                s.section_name AS SectionName,
                FORMAT(naf.PaymentDate, 'dd-MM-yyyy') AS Date,
                fh.FeeHead,
                naf.FeeAmount,
                pm.PaymentMode,
                naf.TransactionDetails,
                naf.Remarks
            FROM 
                tblNonAcademicFees naf
            LEFT JOIN 
                tblPayeeType pt ON naf.PayeeTypeID = pt.PayeeTypeID
            LEFT JOIN 
                tbl_StudentMaster sm ON naf.StudentID = sm.student_id
            LEFT JOIN 
                tbl_EmployeeProfileMaster emp ON naf.EmployeeID = emp.Employee_id
            LEFT JOIN 
                tbl_Class c ON naf.ClassID = c.class_id
            LEFT JOIN 
                tbl_Section s ON naf.SectionID = s.section_id
            LEFT JOIN 
                tblFeeHead fh ON naf.FeeHeadID = fh.FeeHeadID
            LEFT JOIN 
                tblPaymentMode pm ON naf.PaymentModeID = pm.PaymentModeID
            WHERE 
                naf.InstituteID = @InstituteID 
                AND naf.PayeeTypeID = @PayeeTypeID 
                AND naf.PaymentDate BETWEEN @StartDate AND @EndDate
                AND naf.IsActive = 1
                AND (@Search IS NULL OR 
                     CASE 
                        WHEN naf.PayeeTypeID = 1 THEN CONCAT(sm.First_Name, ' ', sm.Last_Name)
                        WHEN naf.PayeeTypeID = 2 THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                        WHEN naf.PayeeTypeID = 3 THEN naf.PayeeName
                        ELSE NULL
                     END LIKE '%' + @Search + '%')";

            var parameters = new
            {
                request.InstituteID,
                request.PayeeTypeID,
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null),
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
