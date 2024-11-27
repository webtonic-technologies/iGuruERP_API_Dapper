using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeeWaiverRepository : IFeeWaiverRepository
    {
        private readonly IDbConnection _connection;

        public FeeWaiverRepository(IDbConnection connection)
        {
            _connection = connection;
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
                        string query;

                        // Check if FeesWaiverID is provided for updates
                        if (waiver.FeesWaiverID > 0) // Change here
                        {
                            query = @"
                        UPDATE tblStudentFeeWaiver
                        SET 
                            StudentID = @StudentID,
                            ClassID = @ClassID,
                            SectionID = @SectionID,
                            InstituteID = @InstituteID,
                            FeeGroupID = @FeeGroupID,
                            FeeHeadID = @FeeHeadID,
                            FeeTenurityID = @FeeTenurityID,
                            TenuritySTMID = @TenuritySTMID,
                            FeeCollectionSTMID = @FeeCollectionSTMID,
                            Amount = @Amount
                        WHERE FeesWaiverID = @FeesWaiverID;"; // Update the record by FeesWaiverID
                        }
                        else // If FeesWaiverID is not provided, insert a new record
                        {
                            query = @"
                        INSERT INTO tblStudentFeeWaiver (StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, Amount)
                        VALUES (@StudentID, @ClassID, @SectionID, @InstituteID, @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, @Amount);";
                        }

                        // Execute the query
                        _connection.Execute(query, waiver, transaction);
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
    }
}
