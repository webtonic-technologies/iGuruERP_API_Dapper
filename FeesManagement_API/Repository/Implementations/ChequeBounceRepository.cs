using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class ChequeBounceRepository : IChequeBounceRepository
    {
        private readonly IDbConnection _connection;

        public ChequeBounceRepository(IDbConnection connection)
        {
            _connection = connection;
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
    }
}
