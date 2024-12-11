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
            //    var query = @"
            //SELECT 
            //    fh.FeeHead,
            //    COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) AS FeeAmount,
            //    CASE 
            //        WHEN fg.FeeTenurityID = 1 THEN 'Single'
            //        WHEN fg.FeeTenurityID = 2 THEN tt.TermName
            //        WHEN fg.FeeTenurityID = 3 THEN tm.Month
            //        ELSE 'N/A'
            //    END AS FeeType,

            //    -- Dynamic Waiver Amount
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sw.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sw.Amount 
            //                 FROM tblStudentFeeWaiver sw 
            //                 WHERE sw.StudentID = sm.student_id 
            //                   AND sw.FeeHeadID = fh.FeeHeadID 
            //                   AND sw.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Waiver based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sw.Amount 
            //                 FROM tblStudentFeeWaiver sw 
            //                 WHERE sw.StudentID = sm.student_id 
            //                   AND sw.FeeHeadID = fh.FeeHeadID 
            //                   AND sw.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Waiver based on month
            //            ELSE 0
            //        END, 0
            //    ) AS WaiverAmount,

            //    -- Dynamic Discount Amount
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sd.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sd.Amount 
            //                 FROM tblStudentDiscount sd 
            //                 WHERE sd.StudentID = sm.student_id 
            //                   AND sd.FeeHeadID = fh.FeeHeadID 
            //                   AND sd.FeeGroupID = fg.FeeGroupID 
            //                   AND sd.TenuritySTMID = tt.TenurityTermID) -- Discount based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sd.Amount 
            //                 FROM tblStudentDiscount sd 
            //                 WHERE sd.StudentID = sm.student_id 
            //                   AND sd.FeeHeadID = fh.FeeHeadID 
            //                   AND sd.FeeGroupID = fg.FeeGroupID 
            //                   AND sd.TenuritySTMID = tm.TenurityMonthID) -- Discount based on month
            //            ELSE 0
            //        END, 0
            //    ) AS DiscountAmount,

            //    -- Dynamic Paid Amount
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sp.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sp.Amount 
            //                 FROM tblStudentFeePayment sp 
            //                 WHERE sp.StudentID = sm.student_id 
            //                   AND sp.FeeHeadID = fh.FeeHeadID 
            //                   AND sp.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Paid based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sp.Amount 
            //                 FROM tblStudentFeePayment sp 
            //                 WHERE sp.StudentID = sm.student_id 
            //                   AND sp.FeeHeadID = fh.FeeHeadID 
            //                   AND sp.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Paid based on month
            //            ELSE 0
            //        END, 0
            //    ) AS PaidAmount,

            //    -- Calculate Balance
            //    COALESCE(ts.Amount, tt.Amount, tm.Amount, 0) - 
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sp.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sp.Amount 
            //                 FROM tblStudentFeePayment sp 
            //                 WHERE sp.StudentID = sm.student_id 
            //                   AND sp.FeeHeadID = fh.FeeHeadID 
            //                   AND sp.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Paid based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sp.Amount 
            //                 FROM tblStudentFeePayment sp 
            //                 WHERE sp.StudentID = sm.student_id 
            //                   AND sp.FeeHeadID = fh.FeeHeadID 
            //                   AND sp.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Paid based on month
            //            ELSE 0
            //        END, 0
            //    ) - 
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sw.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sw.Amount 
            //                 FROM tblStudentFeeWaiver sw 
            //                 WHERE sw.StudentID = sm.student_id 
            //                   AND sw.FeeHeadID = fh.FeeHeadID 
            //                   AND sw.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Waiver based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sw.Amount 
            //                 FROM tblStudentFeeWaiver sw 
            //                 WHERE sw.StudentID = sm.student_id 
            //                   AND sw.FeeHeadID = fh.FeeHeadID 
            //                   AND sw.FeeGroupID = fg.FeeGroupID 
            //                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Waiver based on month
            //            ELSE 0
            //        END, 0
            //    ) - 
            //    COALESCE(
            //        CASE 
            //            WHEN fg.FeeTenurityID = 1 THEN sd.Amount
            //            WHEN fg.FeeTenurityID = 2 THEN 
            //                (SELECT sd.Amount 
            //                 FROM tblStudentDiscount sd 
            //                 WHERE sd.StudentID = sm.student_id 
            //                   AND sd.FeeHeadID = fh.FeeHeadID 
            //                   AND sd.FeeGroupID = fg.FeeGroupID 
            //                   AND sd.TenuritySTMID = tt.TenurityTermID) -- Discount based on term
            //            WHEN fg.FeeTenurityID = 3 THEN 
            //                (SELECT sd.Amount 
            //                 FROM tblStudentDiscount sd 
            //                 WHERE sd.StudentID = sm.student_id 
            //                   AND sd.FeeHeadID = fh.FeeHeadID 
            //                   AND sd.FeeGroupID = fg.FeeGroupID 
            //                   AND sd.TenuritySTMID = tm.TenurityMonthID) -- Discount based on month
            //            ELSE 0
            //        END, 0
            //    ) AS Balance

            //FROM tbl_StudentMaster sm
            //INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            //INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            //INNER JOIN tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
            //INNER JOIN tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
            //INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
            //LEFT JOIN tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
            //LEFT JOIN tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
            //LEFT JOIN tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
            //LEFT JOIN tblStudentFeeWaiver sw ON sw.StudentID = sm.student_id AND sw.FeeHeadID = fh.FeeHeadID AND sw.FeeGroupID = fg.FeeGroupID
            //LEFT JOIN tblStudentDiscount sd ON sd.StudentID = sm.student_id AND sd.FeeHeadID = fh.FeeHeadID AND sd.FeeGroupID = fg.FeeGroupID
            //LEFT JOIN tblStudentFeePayment sp ON sp.StudentID = sm.student_id AND sp.FeeHeadID = fh.FeeHeadID AND sp.FeeGroupID = fg.FeeGroupID
            //WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;";



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
                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Waiver based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sw.Amount 
                 FROM tblStudentFeeWaiver sw 
                 WHERE sw.StudentID = sm.student_id 
                   AND sw.FeeHeadID = fh.FeeHeadID 
                   AND sw.FeeGroupID = fg.FeeGroupID 
                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Waiver based on month
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
                   AND sd.TenuritySTMID = tt.TenurityTermID) -- Discount based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sd.Amount 
                 FROM tblStudentDiscount sd 
                 WHERE sd.StudentID = sm.student_id 
                   AND sd.FeeHeadID = fh.FeeHeadID 
                   AND sd.FeeGroupID = fg.FeeGroupID 
                   AND sd.TenuritySTMID = tm.TenurityMonthID) -- Discount based on month
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
                   AND sp.TenuritySTMID = tt.TenurityTermID) -- Paid based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sp.Amount 
                 FROM tblStudentFeePayment sp 
                 WHERE sp.StudentID = sm.student_id 
                   AND sp.FeeHeadID = fh.FeeHeadID 
                   AND sp.FeeGroupID = fg.FeeGroupID 
                   AND sp.TenuritySTMID = tm.TenurityMonthID) -- Paid based on month
            ELSE 0
        END, 0
    ) AS PaidAmount,

    -- Calculate Balance
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
                   AND sp.TenuritySTMID = tt.TenurityTermID) -- Paid based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sp.Amount 
                 FROM tblStudentFeePayment sp 
                 WHERE sp.StudentID = sm.student_id 
                   AND sp.FeeHeadID = fh.FeeHeadID 
                   AND sp.FeeGroupID = fg.FeeGroupID 
                   AND sp.TenuritySTMID = tm.TenurityMonthID) -- Paid based on month
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
                   AND sw.TenuritySTMID = tt.TenurityTermID) -- Waiver based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sw.Amount 
                 FROM tblStudentFeeWaiver sw 
                 WHERE sw.StudentID = sm.student_id 
                   AND sw.FeeHeadID = fh.FeeHeadID 
                   AND sw.FeeGroupID = fg.FeeGroupID 
                   AND sw.TenuritySTMID = tm.TenurityMonthID) -- Waiver based on month
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
                   AND sd.TenuritySTMID = tt.TenurityTermID) -- Discount based on term
            WHEN fg.FeeTenurityID = 3 THEN 
                (SELECT sd.Amount 
                 FROM tblStudentDiscount sd 
                 WHERE sd.StudentID = sm.student_id 
                   AND sd.FeeHeadID = fh.FeeHeadID 
                   AND sd.FeeGroupID = fg.FeeGroupID 
                   AND sd.TenuritySTMID = tm.TenurityMonthID) -- Discount based on month
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
    ts.TenuritySingleID, 
    ts.FeeCollectionID,
    tt.TenurityTermID, 
    tt.FeeCollectionID,
    tm.TenurityMonthID,
    tm.FeeCollectionID
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
        WHERE sm.student_id = @StudentID AND sm.institute_id = @InstituteID;";

            var results = _connection.Query<dynamic>(query, new { request.StudentID, request.InstituteID });

            var feeData = new GetFeeResponse
            {
                FeeType = new Dictionary<string, FeeCollectionDetail>(),
                TotalFee = 0,
                TotalFeePaid = 0,
                TotalBalance = 0
            };

            foreach (var result in results)
            {
                var feeType = result.FeeType ?? "N/A"; // Default to "N/A" if null

                // Create a new FeeCollectionDetail if not already present
                if (!feeData.FeeType.ContainsKey(feeType))
                {
                    feeData.FeeType[feeType] = new FeeCollectionDetail
                    {
                        FeeHead = result.FeeHead,
                        FeeAmount = result.FeeAmount,
                        PaidAmount = result.PaidAmount,
                        WaiverAmount = result.WaiverAmount,
                        DiscountAmount = result.DiscountAmount,
                        BalanceAmount = result.Balance // Adding Balance to the response
                    };

                    // Update total amounts
                    feeData.TotalFee += result.FeeAmount;
                    feeData.TotalFeePaid += result.PaidAmount;
                    feeData.TotalBalance += result.Balance;
                }
            }

            return new ServiceResponse<GetFeeResponse>(true, "Fee data retrieved successfully", feeData, 200);
        }

        public ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request)
        {
            // Check if the connection is open, if not open it
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open(); // Ensure the connection is opened
            }

            // Use the existing connection for the transaction
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Create a list to hold generated FeesPaymentIDs
                    var feesPaymentIDs = new List<int>();

                    // Insert into tblStudentFeePayment
                    foreach (var payment in request.Payment)
                    {
                        var insertPaymentQuery = @"
                        INSERT INTO tblStudentFeePayment (StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, Amount)
                        OUTPUT INSERTED.FeesPaymentID -- Retrieve the generated FeesPaymentID
                        VALUES (@StudentID, @ClassID, @SectionID, @InstituteID, @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, @Amount);";

                        // Execute the query and retrieve the FeesPaymentID
                        var feesPaymentID = _connection.QuerySingle<int>(insertPaymentQuery, payment, transaction);
                        feesPaymentIDs.Add(feesPaymentID); // Add the ID to the list
                    }

                    // Prepare the PaymentIDs string
                    var paymentIDsString = string.Join(",", feesPaymentIDs);

                    // Insert into tblStudentFeePaymentTransaction
                    var paymentTransaction = request.PaymentTransaction;

                    var insertTransactionQuery = @"
                    INSERT INTO tblStudentFeePaymentTransaction (PaymentAmount, LateFee, Offer, PaymentModeID, CashTransactionDate, ChequeNo, ChequeDate, ChequeBankName, CardTransactionDetail, CardTransactionDate, OnlineTransactionDetail, OnlineTransactionDate, QRDate, ChallanTransactionDetail, ChallanTransactionDate, Remarks, PaymentIDs, InstituteID, SysTransactionDate)
                    VALUES (@PaymentAmount, @LateFee, @Offer, @PaymentModeID, @CashTransactionDate, @ChequeNo, @ChequeDate, @ChequeBankName, @CardTransactionDetail, @CardTransactionDate, @OnlineTransactionDetail, @OnlineTransactionDate, @QRDate, @ChallanTransactionDetail, @ChallanTransactionDate, @Remarks, @PaymentIDs, @InstituteID, @SysTransactionDate);";

                    // Use a dynamic parameter for the paymentIDs in the insert
                    //var parameters = new
                    //{
                    //    paymentTransaction.PaymentAmount,
                    //    paymentTransaction.LateFee,
                    //    paymentTransaction.Offer,
                    //    paymentTransaction.PaymentModeID,
                    //    paymentTransaction.CashTransactionDate,
                    //    paymentTransaction.ChequeNo,
                    //    paymentTransaction.ChequeDate,
                    //    paymentTransaction.ChequeBankName,
                    //    paymentTransaction.CardTransactionDetail,
                    //    paymentTransaction.CardTransactionDate,
                    //    paymentTransaction.OnlineTransactionDetail,
                    //    paymentTransaction.OnlineTransactionDate,
                    //    paymentTransaction.QRDate,
                    //    paymentTransaction.ChallanTransactionDetail,
                    //    paymentTransaction.ChallanTransactionDate,
                    //    paymentTransaction.Remarks,
                    //    PaymentIDs = paymentIDsString, // Inline assignment
                    //    paymentTransaction.InstituteID,
                    //    paymentTransaction.SysTransactionDate
                    //};


                    var parameters = new
                    {
                        paymentTransaction.PaymentAmount,
                        paymentTransaction.LateFee,
                        paymentTransaction.Offer,
                        paymentTransaction.PaymentModeID,
                        CashTransactionDate = string.IsNullOrEmpty(paymentTransaction.CashTransactionDate) ?
                          (DateTime?)null :
                          DateTime.ParseExact(paymentTransaction.CashTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        paymentTransaction.ChequeNo,
                        ChequeDate = string.IsNullOrEmpty(paymentTransaction.ChequeDate) ?
                 (DateTime?)null :
                 DateTime.ParseExact(paymentTransaction.ChequeDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        paymentTransaction.ChequeBankName,
                        paymentTransaction.CardTransactionDetail,
                        CardTransactionDate = string.IsNullOrEmpty(paymentTransaction.CardTransactionDate) ?
                          (DateTime?)null :
                          DateTime.ParseExact(paymentTransaction.CardTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        paymentTransaction.OnlineTransactionDetail,
                        OnlineTransactionDate = string.IsNullOrEmpty(paymentTransaction.OnlineTransactionDate) ?
                            (DateTime?)null :
                            DateTime.ParseExact(paymentTransaction.OnlineTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        QRDate = string.IsNullOrEmpty(paymentTransaction.QRDate) ?
             (DateTime?)null :
             DateTime.ParseExact(paymentTransaction.QRDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        paymentTransaction.ChallanTransactionDetail,
                        ChallanTransactionDate = string.IsNullOrEmpty(paymentTransaction.ChallanTransactionDate) ?
                             (DateTime?)null :
                             DateTime.ParseExact(paymentTransaction.ChallanTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        paymentTransaction.Remarks,
                        PaymentIDs = paymentIDsString,
                        paymentTransaction.InstituteID,
                        SysTransactionDate = DateTime.ParseExact(paymentTransaction.SysTransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                    };


                    // Execute the insert for the payment transaction
                    _connection.Execute(insertTransactionQuery, parameters, transaction);

                    // Commit the transaction
                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Payment submitted successfully", true, 200);
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of error
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
                }
                finally
                {
                    // Ensure to close the connection if you opened it in this method
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }
        }


        //public ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request)
        //{
        //    // Check if the connection is open, if not open it
        //    if (_connection.State != ConnectionState.Open)
        //    {
        //        _connection.Open(); // Ensure the connection is opened
        //    }

        //    // Use the existing connection for the transaction
        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Create a list to hold generated FeesPaymentIDs
        //            var feesPaymentIDs = new List<int>();

        //            // Insert into tblStudentFeePayment
        //            foreach (var payment in request.Payment)
        //            {
        //                var insertPaymentQuery = @"
        //        INSERT INTO tblStudentFeePayment (StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, Amount)
        //        OUTPUT INSERTED.FeesPaymentID -- Retrieve the generated FeesPaymentID
        //        VALUES (@StudentID, @ClassID, @SectionID, @InstituteID, @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, @Amount);";

        //                // Execute the query and retrieve the FeesPaymentID
        //                var feesPaymentID = _connection.QuerySingle<int>(insertPaymentQuery, payment, transaction);
        //                feesPaymentIDs.Add(feesPaymentID); // Add the ID to the list
        //            }

        //            // Prepare the PaymentIDs string for the SQL insert
        //            var paymentIDsString = string.Join(",", feesPaymentIDs);

        //            // Prepare the PaymentTransaction without the PaymentIDs property in the request
        //            var paymentTransaction = request.PaymentTransaction;
        //            // Setting PaymentIDs dynamically for the transaction
        //            paymentTransaction.PaymentIDs = paymentIDsString; // This line will set the PaymentIDs for the transaction SQL

        //            var insertTransactionQuery = @"
        //    INSERT INTO tblStudentFeePaymentTransaction (PaymentAmount, LateFee, Offer, PaymentModeID, CashTransactionDate, ChequeNo, ChequeDate, ChequeBankName, CardTransactionDetail, CardTransactionDate, OnlineTransactionDetail, OnlineTransactionDate, QRDate, ChallanTransactionDetail, ChallanTransactionDate, Remarks, PaymentIDs, InstituteID, SysTransactionDate)
        //    VALUES (@PaymentAmount, @LateFee, @Offer, @PaymentModeID, @CashTransactionDate, @ChequeNo, @ChequeDate, @ChequeBankName, @CardTransactionDetail, @CardTransactionDate, @OnlineTransactionDetail, @OnlineTransactionDate, @QRDate, @ChallanTransactionDetail, @ChallanTransactionDate, @Remarks, @PaymentIDs, @InstituteID, @SysTransactionDate);";

        //            // Execute the insert for the payment transaction
        //            _connection.Execute(insertTransactionQuery, paymentTransaction, transaction);

        //            // Commit the transaction
        //            transaction.Commit();
        //            return new ServiceResponse<bool>(true, "Payment submitted successfully", true, 200);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback the transaction in case of error
        //            transaction.Rollback();
        //            return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
        //        }
        //        finally
        //        {
        //            // Ensure to close the connection if you opened it in this method
        //            if (_connection.State == ConnectionState.Open)
        //            {
        //                _connection.Close();
        //            }
        //        }
        //    }
        //}
    }
}
