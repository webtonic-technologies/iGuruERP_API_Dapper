using Dapper;
using FeesManagement_API.DTOs.Requests; 
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse; 
using FeesManagement_API.Repository.Interfaces;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class PaymentChecklistRepository : IPaymentChecklistRepository
    {
        private readonly IDbConnection _connection;

        public PaymentChecklistRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> SetPaymentChecklist(List<PaymentChecklistSetRequest> requests)
        {
            if (requests == null || !requests.Any())
                return 0;

            // Ensure the connection is open.
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            // Assume that all checklist records belong to the same Institute.
            var instituteId = requests.First().InstituteID;

            using (var transaction = _connection.BeginTransaction())
            {
                // Delete existing records for the given Institute.
                var deleteQuery = "DELETE FROM tblFeesMobilePaymentChecklist WHERE InstituteID = @InstituteID";
                await _connection.ExecuteAsync(deleteQuery, new { InstituteID = instituteId }, transaction);

                // Insert new checklist records.
                var insertQuery = @"
                    INSERT INTO tblFeesMobilePaymentChecklist 
                        (InstituteID, FeeHeadID, FeeTenureID, IsEnable, IsMandatory, IsEditable, SequenceOrder)
                    VALUES 
                        (@InstituteID, @FeeHeadID, @FeeTenureID, @IsEnable, @IsMandatory, @IsEditable, @SequenceOrder);
                ";

                var rowsAffected = await _connection.ExecuteAsync(insertQuery, requests, transaction);
                transaction.Commit();

                return rowsAffected;
            }
        }


        public ServiceResponse<List<PaymentChecklistGetResponse>> GetPaymentChecklist(PaymentChecklistGetRequest request)
        {
            var response = new ServiceResponse<List<PaymentChecklistGetResponse>>(
                true,
                "Payment checklist retrieved successfully",
                new List<PaymentChecklistGetResponse>(),
                200
            );

            // Query joining the checklist with FeeHead and FeeTenurityMaster.
            string query = @"
                SELECT 
                    p.FeeHeadID,
                    fh.FeeHead,
                    p.FeeTenureID,
                    ft.FeeTenurityType AS FeeTenure,
                    p.IsEnable,
                    p.IsMandatory,
                    p.IsEditable,
                    p.SequenceOrder
                FROM tblFeesMobilePaymentChecklist p
                LEFT JOIN tblFeeHead fh ON p.FeeHeadID = fh.FeeHeadID
                LEFT JOIN tblFeeTenurityMaster ft ON p.FeeTenureID = ft.FeeTenurityID
                WHERE p.InstituteID = @InstituteID
                ORDER BY p.SequenceOrder;
            ";

            var parameters = new { request.InstituteID };

            var checklist = _connection.Query<PaymentChecklistGetResponse>(query, parameters).ToList();
            response.Data = checklist;

            if (!checklist.Any())
            {
                response.Success = false;
                response.Message = "No payment checklist records found.";
            }

            return response;
        }

    }
}
