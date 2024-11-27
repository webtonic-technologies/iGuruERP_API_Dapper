using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class RefundRepository : IRefundRepository
    {
        private readonly IDbConnection _connection;

        public RefundRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public string AddRefund(AddRefundRequest request)
        {
            int refundID;

            // SQL query to add or update the refund
            var query = @"
        DECLARE @NewRefundID INT; -- Declare the variable

        IF EXISTS (SELECT 1 FROM tblRefund WHERE RefundID = @RefundID)
        BEGIN
            UPDATE tblRefund 
            SET AcademiceYearCode = @AcademiceYearCode, ClassID = @ClassID, SectionID = @SectionID,
                StudentStatus = @StudentStatus, StudentID = @StudentID, RefundAmount = @RefundAmount,
                RefundDate = @RefundDate, PaymentModeID = @PaymentModeID, Remarks = @Remarks,
                InstituteID = @InstituteID, EmployeeID = @EmployeeID, IsActive = @IsActive
            WHERE RefundID = @RefundID;

            SET @NewRefundID = @RefundID; -- Keep the same RefundID after update
        END
        ELSE
        BEGIN
            INSERT INTO tblRefund (AcademiceYearCode, ClassID, SectionID, StudentStatus, StudentID, 
                RefundAmount, RefundDate, PaymentModeID, Remarks, InstituteID, EmployeeID, IsActive)
            VALUES (@AcademiceYearCode, @ClassID, @SectionID, @StudentStatus, @StudentID, 
                @RefundAmount, @RefundDate, @PaymentModeID, @Remarks, @InstituteID, @EmployeeID, @IsActive);
            
            -- Get the last inserted RefundID
            SET @NewRefundID = SCOPE_IDENTITY();
        END;

        SELECT @NewRefundID; -- Return the RefundID at the end
    ";

            // Execute the query and get the RefundID
            refundID = _connection.ExecuteScalar<int>(query, new
            {
                request.RefundID,
                request.AcademiceYearCode,
                request.ClassID,
                request.SectionID,
                request.StudentStatus,
                request.StudentID,
                request.RefundAmount,
                request.RefundDate,
                request.PaymentModeID,
                request.Remarks,
                request.InstituteID,
                request.EmployeeID,
                request.IsActive
            });

            // Handle refund transfer
            if (request.PaymentModeID == 2 && request.RefundTransfer != null)
            {
                var refundTransferQuery = @"
            INSERT INTO tblRefundTransfer (RefundID, BankName, AccountNo, IFSCCode, TransactionDate)
            VALUES (@RefundID, @BankName, @AccountNo, @IFSCCode, @TransactionDate);";

                _connection.Execute(refundTransferQuery, new
                {
                    RefundID = refundID,
                    BankName = request.RefundTransfer.BankName,
                    AccountNo = request.RefundTransfer.AccountNo,
                    IFSCCode = request.RefundTransfer.IFSCCode,
                    TransactionDate = request.RefundTransfer.TransactionDate
                });
            }

            // Handle refund cheque
            if (request.PaymentModeID == 3 && request.RefundCheque != null)
            {
                var refundChequeQuery = @"
            INSERT INTO tblRefundCheque (RefundID, ChequeNo, BankName, AccountNo, IFSCCode, IssueDate)
            VALUES (@RefundID, @ChequeNo, @BankName, @AccountNo, @IFSCCode, @IssueDate);";

                _connection.Execute(refundChequeQuery, new
                {
                    RefundID = refundID,
                    ChequeNo = request.RefundCheque.ChequeNo,
                    BankName = request.RefundCheque.BankName,
                    AccountNo = request.RefundCheque.AccountNo,
                    IFSCCode = request.RefundCheque.IFSCCode,
                    IssueDate = request.RefundCheque.IssueDate
                });
            }

            // Handle refund card
            if (request.PaymentModeID == 4 && request.RefundCard != null)
            {
                var refundCardQuery = @"
            INSERT INTO tblRefundCard (RefundID, TransactionID, IssueDate)
            VALUES (@RefundID, @TransactionID, @IssueDate);";

                _connection.Execute(refundCardQuery, new
                {
                    RefundID = refundID,
                    TransactionID = request.RefundCard.TransactionID,
                    IssueDate = request.RefundCard.IssueDate
                });
            }

            // Handle refund wallet
            if (request.PaymentModeID == 5 && request.RefundWallet != null)
            {
                var refundWalletQuery = @"
            INSERT INTO tblRefundWallet (RefundID, WalletBalance)
            VALUES (@RefundID, @WalletBalance);";

                _connection.Execute(refundWalletQuery, new
                {
                    RefundID = refundID,
                    WalletBalance = request.RefundWallet.WalletBalance
                });
            }

            return "Success";
        }

        public List<GetRefundResponse> GetRefund(GetRefundRequest request)
        {
            var query = @"
        SELECT 
            r.RefundID,
            r.AcademiceYearCode,
            r.ClassID,
            r.SectionID,
            r.StudentStatus,
            r.StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            r.RefundAmount,
            FORMAT(r.RefundDate, 'dd-MM-yyyy') AS RefundDate,
            r.PaymentModeID,
            m.PaymentMode,
            r.Remarks,
            r.InstituteID,
            r.EmployeeID,
            CONCAT(emp.First_Name, ' ', emp.Last_Name) AS EmployeeName,
            c.class_name AS ClassName,
            s.section_name AS SectionName,
            r.IsActive
        FROM 
            tblRefund r
        LEFT JOIN 
            tbl_Class c ON r.ClassID = c.class_id
        LEFT JOIN 
            tbl_Section s ON r.SectionID = s.section_id
        LEFT JOIN 
            tblPaymentMode m ON r.PaymentModeID = m.PaymentModeID
        LEFT JOIN 
            tbl_StudentMaster sm ON r.StudentID = sm.student_id
        LEFT JOIN 
            tbl_EmployeeProfileMaster emp ON r.EmployeeID = emp.Employee_id
        WHERE 
            r.InstituteID = @InstituteID 
            AND r.ClassID = @ClassID 
            AND r.SectionID = @SectionID 
            AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
                 sm.Admission_Number LIKE '%' + @Search + '%')";

            return _connection.Query<GetRefundResponse>(query, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                Search = request.Search // Assuming Search is a property in GetRefundRequest
            }).ToList();
        }

        public string DeleteRefund(int refundID)
        {
            var query = @"UPDATE tblRefund SET IsActive = 0 WHERE RefundID = @RefundID";
            _connection.Execute(query, new { RefundID = refundID });
            return "Success";
        }
    }
}
