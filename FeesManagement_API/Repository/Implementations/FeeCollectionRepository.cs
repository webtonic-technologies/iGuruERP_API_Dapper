using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.AspNetCore.Connections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeeCollectionRepository : IFeeCollectionRepository
    {
        private readonly IDbConnection _connection;

        public FeeCollectionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public ServiceResponse<GetFeeResponse> GetFeesCollection(GetFeeRequest request)
        {
            var query = @"
        SELECT 
            fh.FeeHead,
            COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) AS FeeAmount,
            CASE 
                WHEN fg.FeeTenurityID = 1 THEN 'Single'
                WHEN fg.FeeTenurityID = 2 THEN tt.TermName
                WHEN fg.FeeTenurityID = 3 THEN tm.Month
                ELSE 'N/A'
            END AS FeeType,

            -- Dynamic Waiver Amount
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sw.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sw.Amount 
                         FROM tblStudentFeeWaiver sw 
                         WHERE sw.StudentID = sm.student_id 
                           AND sw.FeeHeadID = fh.FeeHeadID 
                           AND sw.FeeGroupID = fg.FeeGroupID 
                           AND sw.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sw.Amount 
                         FROM tblStudentFeeWaiver sw 
                         WHERE sw.StudentID = sm.student_id 
                           AND sw.FeeHeadID = fh.FeeHeadID 
                           AND sw.FeeGroupID = fg.FeeGroupID 
                           AND sw.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) AS WaiverAmount,

            -- Dynamic Discount Amount
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sd.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sd.Amount 
                         FROM tblStudentDiscount sd 
                         WHERE sd.StudentID = sm.student_id 
                           AND sd.FeeHeadID = fh.FeeHeadID 
                           AND sd.FeeGroupID = fg.FeeGroupID 
                           AND sd.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sd.Amount 
                         FROM tblStudentDiscount sd 
                         WHERE sd.StudentID = sm.student_id 
                           AND sd.FeeHeadID = fh.FeeHeadID 
                           AND sd.FeeGroupID = fg.FeeGroupID 
                           AND sd.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) AS DiscountAmount,

            -- Dynamic Paid Amount
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sp.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sp.Amount 
                         FROM tblStudentFeePayment sp 
                         WHERE sp.StudentID = sm.student_id 
                           AND sp.FeeHeadID = fh.FeeHeadID 
                           AND sp.FeeGroupID = fg.FeeGroupID 
                           AND sp.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sp.Amount 
                         FROM tblStudentFeePayment sp 
                         WHERE sp.StudentID = sm.student_id 
                           AND sp.FeeHeadID = fh.FeeHeadID 
                           AND sp.FeeGroupID = fg.FeeGroupID 
                           AND sp.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) AS PaidAmount,

            -- Calculate Balance Amount
            COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) - 
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sp.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sp.Amount 
                         FROM tblStudentFeePayment sp 
                         WHERE sp.StudentID = sm.student_id 
                           AND sp.FeeHeadID = fh.FeeHeadID 
                           AND sp.FeeGroupID = fg.FeeGroupID 
                           AND sp.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sp.Amount 
                         FROM tblStudentFeePayment sp 
                         WHERE sp.StudentID = sm.student_id 
                           AND sp.FeeHeadID = fh.FeeHeadID 
                           AND sp.FeeGroupID = fg.FeeGroupID 
                           AND sp.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) - 
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sw.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sw.Amount 
                         FROM tblStudentFeeWaiver sw 
                         WHERE sw.StudentID = sm.student_id 
                           AND sw.FeeHeadID = fh.FeeHeadID 
                           AND sw.FeeGroupID = fg.FeeGroupID 
                           AND sw.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sw.Amount 
                         FROM tblStudentFeeWaiver sw 
                         WHERE sw.StudentID = sm.student_id 
                           AND sw.FeeHeadID = fh.FeeHeadID 
                           AND sw.FeeGroupID = fg.FeeGroupID 
                           AND sw.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) - 
            COALESCE(
                CASE 
                    WHEN fg.FeeTenurityID = 1 THEN sd.Amount
                    WHEN fg.FeeTenurityID = 2 THEN 
                        (SELECT sd.Amount 
                         FROM tblStudentDiscount sd 
                         WHERE sd.StudentID = sm.student_id 
                           AND sd.FeeHeadID = fh.FeeHeadID 
                           AND sd.FeeGroupID = fg.FeeGroupID 
                           AND sd.TenuritySTMID = tt.TenurityTermID)
                    WHEN fg.FeeTenurityID = 3 THEN 
                        (SELECT sd.Amount 
                         FROM tblStudentDiscount sd 
                         WHERE sd.StudentID = sm.student_id 
                           AND sd.FeeHeadID = fh.FeeHeadID 
                           AND sd.FeeGroupID = fg.FeeGroupID 
                           AND sd.TenuritySTMID = tm.TenurityMonthID)
                    ELSE 0
                END, 0
            ) AS Balance,

            sm.student_id,
            sm.class_id,
            sm.section_id,
            sm.Institute_id,
            fg.FeeGroupID,
            fh.FeeHeadID,
            fg.FeeTenurityID,
            -- Derive TenuritySTMID based on FeeTenurityID
            CASE 
                WHEN fg.FeeTenurityID = 1 THEN ts.TenuritySingleID
                WHEN fg.FeeTenurityID = 2 THEN tt.TenurityTermID
                WHEN fg.FeeTenurityID = 3 THEN tm.TenurityMonthID
                ELSE NULL
            END AS TenuritySTMID,
            -- Derive FeeCollectionSTMID based on FeeTenurityID
            CASE 
                WHEN fg.FeeTenurityID = 1 THEN ts.FeeCollectionID
                WHEN fg.FeeTenurityID = 2 THEN tt.FeeCollectionID
                WHEN fg.FeeTenurityID = 3 THEN tm.FeeCollectionID
                ELSE NULL
            END AS FeeCollectionSTMID
        FROM tbl_StudentMaster sm
        INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        INNER JOIN tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
        INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
        INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
        LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
        LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
        LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
        LEFT JOIN tblStudentFeeWaiver sw ON sw.StudentID = sm.student_id AND sw.FeeHeadID = fh.FeeHeadID AND sw.FeeGroupID = fg.FeeGroupID
        LEFT JOIN tblStudentDiscount sd ON sd.StudentID = sm.student_id AND sd.FeeHeadID = fh.FeeHeadID AND sd.FeeGroupID = fg.FeeGroupID
        LEFT JOIN tblStudentFeePayment sp ON sp.StudentID = sm.student_id AND sp.FeeHeadID = fh.FeeHeadID AND sp.FeeGroupID = fg.FeeGroupID
        WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;
    ";

            // Execute the query using Dapper (or your preferred ORM)
            var results = _connection.Query<dynamic>(query, new { request.StudentID, request.InstituteID });

            // Initialize the response object with new aggregated totals (TotalWaiver, TotalDiscount)
            var feeData = new GetFeeResponse
            {
                FeeType = new Dictionary<string, FeeTypeDetail>(),
                TotalFee = 0,
                TotalFeePaid = 0,
                TotalBalance = 0,
                TotalWaiver = 0,
                TotalDiscount = 0
            };

            // Loop through the results and group fee details by FeeType (e.g., Term 1, Term 2, etc.)
            foreach (var result in results)
            {
                // Use the FeeType column (like "Term 1") as the key
                string feeTypeKey = result.FeeType ?? "N/A";

                if (!feeData.FeeType.ContainsKey(feeTypeKey))
                {
                    feeData.FeeType[feeTypeKey] = new FeeTypeDetail();
                }

                // Create a FeeCollectionDetail object and populate it with query result values
                var feeDetail = new FeeCollectionDetail
                {
                    FeeHead = result.FeeHead,
                    FeeAmount = result.FeeAmount,
                    PaidAmount = result.PaidAmount,
                    WaiverAmount = result.WaiverAmount,
                    DiscountAmount = result.DiscountAmount,
                    BalanceAmount = result.Balance,
                    FeeGroupID = result.FeeGroupID,
                    FeeHeadID = result.FeeHeadID,
                    FeeTenurityID = result.FeeTenurityID,
                    TenuritySTMID = result.TenuritySTMID,
                    FeeCollectionSTMID = result.FeeCollectionSTMID
                };

                // Add the fee detail to the corresponding fee type entry and update the term total fee
                feeData.FeeType[feeTypeKey].FeeDetails.Add(feeDetail);
                feeData.FeeType[feeTypeKey].SectionTotalFee += feeDetail.FeeAmount;

                // Update overall totals
                feeData.TotalFee += feeDetail.FeeAmount;
                feeData.TotalFeePaid += feeDetail.PaidAmount;
                feeData.TotalBalance += feeDetail.BalanceAmount;

                // Update the new aggregated totals
                feeData.TotalWaiver += feeDetail.WaiverAmount;
                feeData.TotalDiscount += feeDetail.DiscountAmount;
            }

            return new ServiceResponse<GetFeeResponse>(true, "Fee data retrieved successfully", feeData, 200);
        }

        //    public ServiceResponse<GetFeeResponse> GetFeesCollection(GetFeeRequest request)
        //    {
        //        var query = @"
        //    SELECT 
        //        fh.FeeHead,
        //        COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) AS FeeAmount,
        //        CASE 
        //            WHEN fg.FeeTenurityID = 1 THEN 'Single'
        //            WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        //            WHEN fg.FeeTenurityID = 3 THEN tm.Month
        //            ELSE 'N/A'
        //        END AS FeeType,

        //        -- Dynamic Waiver Amount
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sw.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sw.Amount 
        //                     FROM tblStudentFeeWaiver sw 
        //                     WHERE sw.StudentID = sm.student_id 
        //                       AND sw.FeeHeadID = fh.FeeHeadID 
        //                       AND sw.FeeGroupID = fg.FeeGroupID 
        //                       AND sw.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sw.Amount 
        //                     FROM tblStudentFeeWaiver sw 
        //                     WHERE sw.StudentID = sm.student_id 
        //                       AND sw.FeeHeadID = fh.FeeHeadID 
        //                       AND sw.FeeGroupID = fg.FeeGroupID 
        //                       AND sw.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) AS WaiverAmount,

        //        -- Dynamic Discount Amount
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sd.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sd.Amount 
        //                     FROM tblStudentDiscount sd 
        //                     WHERE sd.StudentID = sm.student_id 
        //                       AND sd.FeeHeadID = fh.FeeHeadID 
        //                       AND sd.FeeGroupID = fg.FeeGroupID 
        //                       AND sd.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sd.Amount 
        //                     FROM tblStudentDiscount sd 
        //                     WHERE sd.StudentID = sm.student_id 
        //                       AND sd.FeeHeadID = fh.FeeHeadID 
        //                       AND sd.FeeGroupID = fg.FeeGroupID 
        //                       AND sd.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) AS DiscountAmount,

        //        -- Dynamic Paid Amount
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sp.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sp.Amount 
        //                     FROM tblStudentFeePayment sp 
        //                     WHERE sp.StudentID = sm.student_id 
        //                       AND sp.FeeHeadID = fh.FeeHeadID 
        //                       AND sp.FeeGroupID = fg.FeeGroupID 
        //                       AND sp.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sp.Amount 
        //                     FROM tblStudentFeePayment sp 
        //                     WHERE sp.StudentID = sm.student_id 
        //                       AND sp.FeeHeadID = fh.FeeHeadID 
        //                       AND sp.FeeGroupID = fg.FeeGroupID 
        //                       AND sp.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) AS PaidAmount,

        //        -- Calculate Balance
        //        COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) - 
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sp.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sp.Amount 
        //                     FROM tblStudentFeePayment sp 
        //                     WHERE sp.StudentID = sm.student_id 
        //                       AND sp.FeeHeadID = fh.FeeHeadID 
        //                       AND sp.FeeGroupID = fg.FeeGroupID 
        //                       AND sp.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sp.Amount 
        //                     FROM tblStudentFeePayment sp 
        //                     WHERE sp.StudentID = sm.student_id 
        //                       AND sp.FeeHeadID = fh.FeeHeadID 
        //                       AND sp.FeeGroupID = fg.FeeGroupID 
        //                       AND sp.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) - 
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sw.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sw.Amount 
        //                     FROM tblStudentFeeWaiver sw 
        //                     WHERE sw.StudentID = sm.student_id 
        //                       AND sw.FeeHeadID = fh.FeeHeadID 
        //                       AND sw.FeeGroupID = fg.FeeGroupID 
        //                       AND sw.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sw.Amount 
        //                     FROM tblStudentFeeWaiver sw 
        //                     WHERE sw.StudentID = sm.student_id 
        //                       AND sw.FeeHeadID = fh.FeeHeadID 
        //                       AND sw.FeeGroupID = fg.FeeGroupID 
        //                       AND sw.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) - 
        //        COALESCE(
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN sd.Amount
        //                WHEN fg.FeeTenurityID = 2 THEN 
        //                    (SELECT sd.Amount 
        //                     FROM tblStudentDiscount sd 
        //                     WHERE sd.StudentID = sm.student_id 
        //                       AND sd.FeeHeadID = fh.FeeHeadID 
        //                       AND sd.FeeGroupID = fg.FeeGroupID 
        //                       AND sd.TenuritySTMID = tt.TenurityTermID)
        //                WHEN fg.FeeTenurityID = 3 THEN 
        //                    (SELECT sd.Amount 
        //                     FROM tblStudentDiscount sd 
        //                     WHERE sd.StudentID = sm.student_id 
        //                       AND sd.FeeHeadID = fh.FeeHeadID 
        //                       AND sd.FeeGroupID = fg.FeeGroupID 
        //                       AND sd.TenuritySTMID = tm.TenurityMonthID)
        //                ELSE 0
        //            END, 0
        //        ) AS Balance,

        //        sm.student_id,
        //        sm.class_id,
        //        sm.section_id,
        //        sm.Institute_id,
        //        fg.FeeGroupID,
        //        fh.FeeHeadID,
        //        fg.FeeTenurityID,
        //        -- Derive TenuritySTMID based on FeeTenurityID
        //        CASE 
        //            WHEN fg.FeeTenurityID = 1 THEN ts.TenuritySingleID
        //            WHEN fg.FeeTenurityID = 2 THEN tt.TenurityTermID
        //            WHEN fg.FeeTenurityID = 3 THEN tm.TenurityMonthID
        //            ELSE NULL
        //        END AS TenuritySTMID,
        //        -- Derive FeeCollectionSTMID based on FeeTenurityID
        //        CASE 
        //            WHEN fg.FeeTenurityID = 1 THEN ts.FeeCollectionID
        //            WHEN fg.FeeTenurityID = 2 THEN tt.FeeCollectionID
        //            WHEN fg.FeeTenurityID = 3 THEN tm.FeeCollectionID
        //            ELSE NULL
        //        END AS FeeCollectionSTMID
        //    FROM tbl_StudentMaster sm
        //    INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        //    INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        //    INNER JOIN tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
        //    INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
        //    INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
        //    LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblStudentFeeWaiver sw ON sw.StudentID = sm.student_id AND sw.FeeHeadID = fh.FeeHeadID AND sw.FeeGroupID = fg.FeeGroupID
        //    LEFT JOIN tblStudentDiscount sd ON sd.StudentID = sm.student_id AND sd.FeeHeadID = fh.FeeHeadID AND sd.FeeGroupID = fg.FeeGroupID
        //    LEFT JOIN tblStudentFeePayment sp ON sp.StudentID = sm.student_id AND sp.FeeHeadID = fh.FeeHeadID AND sp.FeeGroupID = fg.FeeGroupID
        //    WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;
        //";

        //        // Execute the query using Dapper (or your preferred ORM)
        //        var results = _connection.Query<dynamic>(query, new { request.StudentID, request.InstituteID });

        //        // Initialize the response object
        //        var feeData = new GetFeeResponse
        //        {
        //            FeeType = new Dictionary<string, FeeTypeDetail>(),
        //            TotalFee = 0,
        //            TotalFeePaid = 0,
        //            TotalBalance = 0
        //        };

        //        // Loop through the results and group fee details by FeeType (e.g., Term 1, Term 2, etc.)
        //        foreach (var result in results)
        //        {
        //            // Use the FeeType column (like "Term 1") as the key
        //            string feeTypeKey = result.FeeType ?? "N/A";

        //            if (!feeData.FeeType.ContainsKey(feeTypeKey))
        //            {
        //                feeData.FeeType[feeTypeKey] = new FeeTypeDetail();
        //            }

        //            // Create a FeeCollectionDetail object and populate it with query result values
        //            var feeDetail = new FeeCollectionDetail
        //            {
        //                FeeHead = result.FeeHead,
        //                FeeAmount = result.FeeAmount,
        //                PaidAmount = result.PaidAmount,
        //                WaiverAmount = result.WaiverAmount,
        //                DiscountAmount = result.DiscountAmount,
        //                BalanceAmount = result.Balance,

        //                // Populate the new parameters
        //                FeeGroupID = result.FeeGroupID,
        //                FeeHeadID = result.FeeHeadID,
        //                FeeTenurityID = result.FeeTenurityID,
        //                TenuritySTMID = result.TenuritySTMID,
        //                FeeCollectionSTMID = result.FeeCollectionSTMID
        //            };

        //            // Add the fee detail to the corresponding fee type entry and update the term total fee
        //            feeData.FeeType[feeTypeKey].FeeDetails.Add(feeDetail);
        //            feeData.FeeType[feeTypeKey].SectionTotalFee += feeDetail.FeeAmount;

        //            // Update overall totals
        //            feeData.TotalFee += feeDetail.FeeAmount;
        //            feeData.TotalFeePaid += feeDetail.PaidAmount;
        //            feeData.TotalBalance += feeDetail.BalanceAmount;
        //        }

        //        return new ServiceResponse<GetFeeResponse>(true, "Fee data retrieved successfully", feeData, 200);
        //    }



        public ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request)
        {
            // Open connection if not already open
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            // Begin a transaction using the current connection
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Generate a unique TransactionCode (e.g., based on current timestamp)
                    string transactionCode = "TXN" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    // Insert each payment into tblStudentFeePayment
                    foreach (var payment in request.Payment)
                    {
                        var insertPaymentQuery = @"
                        INSERT INTO tblStudentFeePayment 
                            (StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, Amount, TransactionCode)
                        VALUES 
                            (@StudentID, @ClassID, @SectionID, @InstituteID, @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, @Amount, @TransactionCode);";

                        var paymentParameters = new
                        {
                            payment.StudentID,
                            payment.ClassID,
                            payment.SectionID,
                            payment.InstituteID,
                            payment.FeeGroupID,
                            payment.FeeHeadID,
                            payment.FeeTenurityID,
                            payment.TenuritySTMID,
                            payment.FeeCollectionSTMID,
                            payment.Amount,
                            TransactionCode = transactionCode
                        };

                        _connection.Execute(insertPaymentQuery, paymentParameters, transaction);
                    }

                    // Insert into tblStudentFeePaymentTransaction using the new schema.
                    var pt = request.PaymentTransaction;
                    var insertTransactionQuery = @"
                    INSERT INTO tblStudentFeePaymentTransaction 
                    (
                        TransactionCode, TotalAmount, LateFeesAmount, OfferAmount, IsWalletUsed, WalletAmount, PayableAmount, 
                        PaymentModeID, ChequeNo, ChequeDate, ChequeBankName, TransactionDetail, QRImage, TransactionDate, Remarks, InstituteID, SysTransactionDate
                    )
                    VALUES
                    (
                        @TransactionCode, @TotalAmount, @LateFeesAmount, @OfferAmount, @IsWalletUsed, @WalletAmount, @PayableAmount, 
                        @PaymentModeID, @ChequeNo, @ChequeDate, @ChequeBankName, @TransactionDetail, @QRImage, @TransactionDate, @Remarks, @InstituteID, @SysTransactionDate
                    );";

                    var transactionParameters = new
                    {
                        TransactionCode = transactionCode,
                        pt.TotalAmount,
                        pt.LateFeesAmount,
                        pt.OfferAmount,
                        pt.IsWalletUsed,
                        pt.WalletAmount,
                        pt.PayableAmount,
                        pt.PaymentModeID,
                        pt.ChequeNo,
                        ChequeDate = string.IsNullOrEmpty(pt.ChequeDate) ? (DateTime?)null :
                            DateTime.ParseExact(pt.ChequeDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        pt.ChequeBankName,
                        pt.TransactionDetail,
                        pt.QRImage,
                        TransactionDate = string.IsNullOrEmpty(pt.TransactionDate) ? (DateTime?)null :
                            DateTime.ParseExact(pt.TransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        pt.Remarks,
                        pt.InstituteID,
                        SysTransactionDate = DateTime.ParseExact(pt.SysTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                    };

                    _connection.Execute(insertTransactionQuery, transactionParameters, transaction);

                    // Commit the transaction if all commands execute successfully
                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Payment submitted successfully", true, 200);
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
                }
                finally
                {
                    // Ensure the connection is closed if it was opened in this method
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }
        }

         
        public ServiceResponse<bool> SubmitFeeWaiver(SubmitFeeWaiverRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    foreach (var waiver in request.FeeWaivers)
                    {
                        // Delete any existing record with matching key parameters.
                        string deleteQuery = @"
                            DELETE FROM tblStudentFeeWaiver
                            WHERE StudentID = @StudentID
                              AND ClassID = @ClassID
                              AND SectionID = @SectionID
                              AND InstituteID = @InstituteID
                              AND FeeGroupID = @FeeGroupID
                              AND FeeHeadID = @FeeHeadID
                              AND FeeTenurityID = @FeeTenurityID
                              AND TenuritySTMID = @TenuritySTMID
                              AND FeeCollectionSTMID = @FeeCollectionSTMID;
                        ";
                        _connection.Execute(deleteQuery, waiver, transaction);

                        // Insert a new fee waiver record using the system current date for FeeWaiverDate.
                        string insertQuery = @"
                            INSERT INTO tblStudentFeeWaiver 
                            (
                                StudentID, ClassID, SectionID, InstituteID, 
                                FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, 
                                Amount, FeeWaiverDate, WaiverGivenBy, Reason
                            )
                            VALUES 
                            (
                                @StudentID, @ClassID, @SectionID, @InstituteID, 
                                @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, 
                                @Amount, GETDATE(), @WaiverGivenBy, @Reason
                            );
                        ";
                                _connection.Execute(insertQuery, waiver, transaction);
                            }

                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Fee waivers submitted successfully", true, 200);
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

        public ServiceResponse<bool> ApplyDiscount(SubmitFeeDiscountRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    foreach (var discount in request.FeeDiscounts)
                    {
                        // Delete any previous record that matches the key parameters.
                        string deleteQuery = @"
                    DELETE FROM tblStudentDiscount
                    WHERE StudentID = @StudentID
                      AND ClassID = @ClassID
                      AND SectionID = @SectionID
                      AND InstituteID = @InstituteID
                      AND FeeGroupID = @FeeGroupID
                      AND FeeHeadID = @FeeHeadID
                      AND FeeTenurityID = @FeeTenurityID
                      AND TenuritySTMID = @TenuritySTMID
                      AND FeeCollectionSTMID = @FeeCollectionSTMID;
                ";
                        _connection.Execute(deleteQuery, discount, transaction);

                        // Insert new discount record.
                        // FeeDiscountDate is set using GETDATE() to capture the system current date.
                        string insertQuery = @"
                    INSERT INTO tblStudentDiscount
                    (
                        StudentID, ClassID, SectionID, InstituteID, 
                        FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, 
                        Amount, FeeDiscountDate, DiscountGivenBy, Reason
                    )
                    VALUES
                    (
                        @StudentID, @ClassID, @SectionID, @InstituteID, 
                        @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, 
                        @Amount, GETDATE(), @DiscountGivenBy, @Reason
                    );
                ";
                        _connection.Execute(insertQuery, discount, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Fee discounts applied/updated successfully", true, 200);
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


        public ServiceResponse<GetWaiverSummaryResponse> GetWaiverSummary(GetWaiverSummaryRequest request)
        {
            // Query to retrieve basic student details (name, admission number, class, section, and roll number)
            string studentQuery = @"
                SELECT 
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name + ' ', ''), sm.Last_Name) AS StudentName,
                    sm.Admission_Number,
                    sm.Roll_Number,
                    c.class_name AS Class,
                    s.section_name AS Section
                FROM tbl_StudentMaster sm
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_Section s ON sm.section_id = s.section_id
                WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;
            ";

            var studentDetail = _connection.QueryFirstOrDefault<dynamic>(
                studentQuery, new { request.StudentID, request.InstituteID });

            if (studentDetail == null)
            {
                return new ServiceResponse<GetWaiverSummaryResponse>(false, "Student not found", null, 404);
            }

            // Query to get all fee waiver records for the student
            string waiverQuery = @"
                SELECT 
                    sw.FeesWaiverID,
                    sw.Amount AS Waiver,
                    sw.FeeWaiverDate,
                    sw.Reason,
                    sw.FeeTenurityID,
                    sw.TenuritySTMID,
                    fh.FeeHead,
                    -- Determine the term string based on FeeTenurityID:
                    CASE 
                        WHEN sw.FeeTenurityID = 1 THEN 'Single'
                        WHEN sw.FeeTenurityID = 2 THEN ft.TermName
                        WHEN sw.FeeTenurityID = 3 THEN tm.Month
                        ELSE 'N/A'
                    END AS Term,
                    ep.First_Name AS EmpFirstName,
                    ep.Middle_Name AS EmpMiddleName,
                    ep.Last_Name AS EmpLastName
                FROM tblStudentFeeWaiver sw
                INNER JOIN tblFeeHead fh ON sw.FeeHeadID = fh.FeeHeadID
                LEFT JOIN tblTenurityTerm ft ON sw.FeeTenurityID = 2 AND sw.TenuritySTMID = ft.TenurityTermID
                LEFT JOIN tblTenurityMonthly tm ON sw.FeeTenurityID = 3 AND sw.TenuritySTMID = tm.TenurityMonthID
                LEFT JOIN tbl_EmployeeProfileMaster ep ON sw.WaiverGivenBy = ep.Employee_id
                WHERE sw.StudentID = @StudentID AND sw.InstituteID = @InstituteID;
            ";

            var waiverRecords = _connection.Query<dynamic>(
                waiverQuery, new { request.StudentID, request.InstituteID }).ToList();

            // Map the results to the DTO list
            List<WaiverDetail> waivers = new List<WaiverDetail>();
            foreach (var record in waiverRecords)
            {
                string feeType = $"{record.FeeHead} - {record.Term}";
                string givenBy = string.Empty;
                if (record.EmpFirstName != null)
                {
                    givenBy = record.EmpFirstName +
                              (string.IsNullOrEmpty(record.EmpMiddleName) ? " " : " " + record.EmpMiddleName + " ") +
                              record.EmpLastName;
                }

                // Format the FeeWaiverDate as "dd-MM-yyyy at hh:mm tt"
                string giveDate = string.Empty;
                if (record.FeeWaiverDate != null)
                {
                    DateTime waiverDate = record.FeeWaiverDate;
                    giveDate = waiverDate.ToString("dd-MM-yyyy 'at' hh:mm tt", CultureInfo.InvariantCulture);
                }

                waivers.Add(new WaiverDetail
                {
                    FeesWaiverID = record.FeesWaiverID,
                    FeeType = feeType,
                    Waiver = record.Waiver,
                    GivenBy = givenBy,
                    GiveDate = giveDate,
                    Reason = record.Reason
                });
            }

            // Assemble the final response DTO
            var responseData = new GetWaiverSummaryResponse
            {
                StudentDetail = new StudentDetail
                {
                    StudentName = studentDetail.StudentName,
                    AdmissionNumber = studentDetail.Admission_Number,
                    Class = studentDetail.Class,
                    Section = studentDetail.Section,
                    RollNumber = studentDetail.Roll_Number
                },
                Waivers = waivers
            };

            return new ServiceResponse<GetWaiverSummaryResponse>(true, "Waiver summary retrieved successfully", responseData, 200);
        }


        public ServiceResponse<GetDiscountSummaryResponse> GetDiscountSummary(GetDiscountSummaryRequest request)
        {
            // Query to retrieve basic student details (name, admission number, class, section, roll number)
            string studentQuery = @"
                SELECT 
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name + ' ', ''), sm.Last_Name) AS StudentName,
                    sm.Admission_Number,
                    sm.Roll_Number,
                    c.class_name AS Class,
                    s.section_name AS Section
                FROM tbl_StudentMaster sm
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_Section s ON sm.section_id = s.section_id
                WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;
            ";

            var studentDetail = _connection.QueryFirstOrDefault<dynamic>(
                studentQuery, new { request.StudentID, request.InstituteID });

            if (studentDetail == null)
            {
                return new ServiceResponse<GetDiscountSummaryResponse>(false, "Student not found", null, 404);
            }

            // Query to get all discount records for the student
            string discountQuery = @"
                SELECT 
                    sd.FeesDiscountID,
                    sd.Amount AS Discount,
                    sd.FeeDiscountDate,
                    sd.Reason,
                    sd.FeeTenurityID,
                    sd.TenuritySTMID,
                    fh.FeeHead,
                    -- Determine the term string based on FeeTenurityID:
                    CASE 
                        WHEN sd.FeeTenurityID = 1 THEN 'Single'
                        WHEN sd.FeeTenurityID = 2 THEN ft.TermName
                        WHEN sd.FeeTenurityID = 3 THEN tm.Month
                        ELSE 'N/A'
                    END AS Term,
                    ep.First_Name AS EmpFirstName,
                    ep.Middle_Name AS EmpMiddleName,
                    ep.Last_Name AS EmpLastName
                FROM tblStudentDiscount sd
                INNER JOIN tblFeeHead fh ON sd.FeeHeadID = fh.FeeHeadID
                LEFT JOIN tblTenurityTerm ft ON sd.FeeTenurityID = 2 AND sd.TenuritySTMID = ft.TenurityTermID
                LEFT JOIN tblTenurityMonthly tm ON sd.FeeTenurityID = 3 AND sd.TenuritySTMID = tm.TenurityMonthID
                LEFT JOIN tbl_EmployeeProfileMaster ep ON sd.DiscountGivenBy = ep.Employee_id
                WHERE sd.StudentID = @StudentID AND sd.InstituteID = @InstituteID;
            ";

            var discountRecords = _connection.Query<dynamic>(
                discountQuery, new { request.StudentID, request.InstituteID }).ToList();

            // Map the records into the DiscountDetail DTO list
            List<DiscountDetail11> discounts = new List<DiscountDetail11>();
            foreach (var record in discountRecords)
            {
                // Combine FeeHead and Term to form the FeeType (for example, "Tuition Fee - Term 1")
                string feeType = $"{record.FeeHead} - {record.Term}";

                // Build the GivenBy string from employee first, middle, and last names if available
                string givenBy = string.Empty;
                if (record.EmpFirstName != null)
                {
                    givenBy = record.EmpFirstName +
                              (string.IsNullOrEmpty(record.EmpMiddleName) ? " " : " " + record.EmpMiddleName + " ") +
                              record.EmpLastName;
                }

                // Format the FeeDiscountDate as "dd-MM-yyyy at hh:mm tt"
                string giveDate = string.Empty;
                if (record.FeeDiscountDate != null)
                {
                    DateTime discountDate = record.FeeDiscountDate;
                    giveDate = discountDate.ToString("dd-MM-yyyy 'at' hh:mm tt", CultureInfo.InvariantCulture);
                }

                discounts.Add(new DiscountDetail11
                {
                    FeesDiscountID = record.FeesDiscountID,
                    FeeType = feeType,
                    Discount = record.Discount,
                    GivenBy = givenBy,
                    GiveDate = giveDate,
                    Reason = record.Reason
                });
            }

            // Assemble the final response DTO
            var responseData = new GetDiscountSummaryResponse
            {
                StudentDetail = new StudentDetail
                {
                    StudentName = studentDetail.StudentName,
                    AdmissionNumber = studentDetail.Admission_Number,
                    Class = studentDetail.Class,
                    Section = studentDetail.Section,
                    // Convert to string if necessary
                    RollNumber = studentDetail.Roll_Number.ToString()
                },
                Discounts = discounts
            };

            return new ServiceResponse<GetDiscountSummaryResponse>(true, "Discount summary retrieved successfully", responseData, 200);
        }

        public ServiceResponse<GetCollectFeeResponse> GetCollectFee(GetCollectFeeRequest request)
        {
            // Query to count total matching students (for pagination)
            string countQuery = @"
                SELECT COUNT(*) 
                FROM tbl_StudentMaster s
                WHERE s.Institute_id = @InstituteID
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                  AND s.AcademicYearCode = @AcademicYearCode
                  AND (
                        CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name + ' ', ''), s.Last_Name) LIKE '%' + @Search + '%' 
                        OR s.Admission_Number LIKE '%' + @Search + '%'
                      );
            ";

            int totalCount = _connection.ExecuteScalar<int>(countQuery, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                request.AcademicYearCode,
                request.Search
            });

            // Data query with pagination – note the use of OFFSET/FETCH (SQL Server syntax)
            string dataQuery = @"
                SELECT
                   s.student_id AS StudentID,
                   CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name + ' ', ''), s.Last_Name) AS StudentName,
                   s.Admission_Number AS AdmissionNumber,
                   c.class_name AS Class,
                   sec.section_name AS Section,
                   s.Roll_Number AS RollNumber,
                   CONCAT(sp.First_Name, ' ', ISNULL(sp.Middle_Name + ' ', ''), sp.Last_Name) AS FatherName,
                   sp.Mobile_Number AS MobileNumber,
                   (
                      SELECT SUM(COALESCE(ts.Amount, tt.Amount, tm.Amount, 0))
                      FROM tblFeeGroupClassSection fgcs
                      INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                      LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fg.FeeGroupID
                      LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fg.FeeGroupID
                      LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fg.FeeGroupID
                      WHERE fgcs.ClassID = s.class_id 
                        AND fgcs.SectionID = s.section_id
                        AND fg.InstituteID = s.Institute_id
                        AND fg.AcademicYearCode = s.AcademicYearCode
                   ) AS TotalFee,
                   (
                      SELECT COALESCE(SUM(sp.Amount), 0)
                      FROM tblStudentFeePayment sp
                      WHERE sp.StudentID = s.student_id AND sp.InstituteID = s.Institute_id
                   ) AS Paid,
                   (
                      (
                        SELECT SUM(COALESCE(ts.Amount, tt.Amount, tm.Amount, 0))
                        FROM tblFeeGroupClassSection fgcs
                        INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                        LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fg.FeeGroupID
                        LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fg.FeeGroupID
                        LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fg.FeeGroupID
                        WHERE fgcs.ClassID = s.class_id 
                          AND fgcs.SectionID = s.section_id
                          AND fg.InstituteID = s.Institute_id
                          AND fg.AcademicYearCode = s.AcademicYearCode
                      ) - COALESCE((
                        SELECT SUM(sp.Amount)
                        FROM tblStudentFeePayment sp
                        WHERE sp.StudentID = s.student_id AND sp.InstituteID = s.Institute_id
                      ), 0)
                   ) AS Balance,
                   (
                      SELECT NULL
                   ) AS LastPaidDate
                FROM tbl_StudentMaster s
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                LEFT JOIN tbl_StudentParentsInfo sp 
                      ON s.student_id = sp.Student_id AND sp.Parent_Type_id = 1
                WHERE s.Institute_id = @InstituteID
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                  AND s.AcademicYearCode = @AcademicYearCode
                  AND (
                        CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name + ' ', ''), s.Last_Name) LIKE '%' + @Search + '%' 
                        OR s.Admission_Number LIKE '%' + @Search + '%'
                      )
                ORDER BY s.student_id
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";

            var collectFeeList = _connection.Query<CollectFeeDetail>(dataQuery, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                request.AcademicYearCode,
                request.Search,
                request.PageNumber,
                request.PageSize
            }).ToList();

            var responseData = new GetCollectFeeResponse
            {
                CollectFeeDetails = collectFeeList
            };

            return new ServiceResponse<GetCollectFeeResponse>(true, "Student fee details retrieved successfully", responseData, 200, totalCount);

        }
    }
}
