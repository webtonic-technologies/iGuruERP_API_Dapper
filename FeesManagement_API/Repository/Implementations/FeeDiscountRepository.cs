using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeeDiscountRepository : IFeeDiscountRepository
    {
        private readonly IDbConnection _connection;

        public FeeDiscountRepository(IDbConnection connection)
        {
            _connection = connection;
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
                        if (discount.FeesDiscountID > 0) // Update existing discount
                        {
                            var updateQuery = @"
                            UPDATE tblStudentDiscount
                            SET StudentID = @StudentID, ClassID = @ClassID, SectionID = @SectionID, InstituteID = @InstituteID, 
                                FeeGroupID = @FeeGroupID, FeeHeadID = @FeeHeadID, FeeTenurityID = @FeeTenurityID, 
                                TenuritySTMID = @TenuritySTMID, FeeCollectionSTMID = @FeeCollectionSTMID, Amount = @Amount
                            WHERE FeesDiscountID = @FeesDiscountID;";

                            _connection.Execute(updateQuery, discount, transaction);
                        }
                        else // Insert new discount
                        {
                            var insertQuery = @"
                            INSERT INTO tblStudentDiscount (StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID, Amount)
                            VALUES (@StudentID, @ClassID, @SectionID, @InstituteID, @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID, @Amount);";

                            _connection.Execute(insertQuery, discount, transaction);
                        }
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
    }
}
