using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
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
            var query = @"IF EXISTS (SELECT 1 FROM tblNonAcademicFees WHERE NonAcademicFeesID = @NonAcademicFeesID)
                  BEGIN
                      UPDATE tblNonAcademicFees 
                      SET InstituteID = @InstituteID, PayeeTypeID = @PayeeTypeID, ClassID = @ClassID, 
                          SectionID = @SectionID, StudentID = @StudentID, EmployeeID = @EmployeeID,
                          PayeeName = @PayeeName, PaymentDate = @PaymentDate, 
                          FeeHeadID = @FeeHeadID, FeeAmount = @FeeAmount, PaymentModeID = @PaymentModeID, 
                          TransactionDetails = @TransactionDetails, Remarks = @Remarks, IsActive = @IsActive
                      WHERE NonAcademicFeesID = @NonAcademicFeesID;
                  END
                  ELSE
                  BEGIN
                      INSERT INTO tblNonAcademicFees (InstituteID, PayeeTypeID, ClassID, SectionID, StudentID, EmployeeID, PayeeName, PaymentDate, 
                          FeeHeadID, FeeAmount, PaymentModeID, TransactionDetails, Remarks, IsActive)
                      VALUES (@InstituteID, @PayeeTypeID, @ClassID, @SectionID, @StudentID, @EmployeeID, @PayeeName, @PaymentDate, 
                          @FeeHeadID, @FeeAmount, @PaymentModeID, @TransactionDetails, @Remarks, @IsActive);
                  END";
            _connection.Execute(query, request);
            return "Success";
        }

        public List<GetNonAcademicFeeResponse> GetNonAcademicFee(GetNonAcademicFeeRequest request)
        {
            // Parse the dates from string to DateTime
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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
            AND naf.PaymentDate BETWEEN @StartDate AND @EndDate";

            // Adjust the request object to pass DateTime
            var parameters = new
            {
                request.InstituteID,
                request.PayeeTypeID,
                StartDate = startDate,
                EndDate = endDate
            };

            return _connection.Query<GetNonAcademicFeeResponse>(query, parameters).ToList();
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

    }
}
