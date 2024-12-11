using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Data;
using System.Globalization;

namespace FeesManagement_API.Repository.Implementations
{
    public class ChequeClearanceRepository : IChequeClearanceRepository
    {
        private readonly IDbConnection _connection;

        public ChequeClearanceRepository(IDbConnection connection)
        {
            _connection = connection;
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
