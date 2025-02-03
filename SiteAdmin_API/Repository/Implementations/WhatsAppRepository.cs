using Dapper;
using Microsoft.Extensions.Configuration;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SiteAdmin_API.Repository.Implementations
{
    public class WhatsAppRepository : IWhatsAppRepository
    {
        private readonly IDbConnection _dbConnection;

        public WhatsAppRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateWhatsAppVendor(AddUpdateWhatsAppVendorRequest request)
        {
            try
            {
                string query;

                if (request.WhatsAppVendorID == 0)
                {
                    // Insert new WhatsApp vendor
                    query = @"INSERT INTO tblWhatsAppVendor (VendorName, PerWhatsAppCost, Email, PhoneNumber, Address) 
                              VALUES (@VendorName, @PerWhatsAppCost, @Email, @PhoneNumber, @Address)";
                }
                else
                {
                    // Update existing WhatsApp vendor
                    query = @"UPDATE tblWhatsAppVendor 
                              SET VendorName = @VendorName, PerWhatsAppCost = @PerWhatsAppCost, Email = @Email, 
                                  PhoneNumber = @PhoneNumber, Address = @Address 
                              WHERE WhatsAppVendorID = @WhatsAppVendorID";
                }

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                {
                    request.WhatsAppVendorID,
                    request.VendorName,
                    request.PerWhatsAppCost,
                    request.Email,
                    request.PhoneNumber,
                    request.Address
                });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "WhatsApp Vendor added/updated successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to add/update WhatsApp Vendor.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>> GetAllWhatsAppVendor()
        {
            try
            {
                string query = @"SELECT WhatsAppVendorID, VendorName, PerWhatsAppCost, Email, PhoneNumber, Address
                                 FROM tblWhatsAppVendor";

                var vendors = await _dbConnection.QueryAsync<GetAllWhatsAppVendorResponse>(query);

                return new ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>(true, "WhatsApp Vendors fetched successfully.", vendors, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetWhatsAppVendorByIDResponse>> GetWhatsAppVendorByID(int WhatsAppVendorID)
        {
            try
            {
                string query = @"SELECT WhatsAppVendorID, VendorName, PerWhatsAppCost, Email, PhoneNumber, Address
                                 FROM tblWhatsAppVendor
                                 WHERE WhatsAppVendorID = @WhatsAppVendorID";

                var vendor = await _dbConnection.QueryFirstOrDefaultAsync<GetWhatsAppVendorByIDResponse>(query, new { WhatsAppVendorID });

                if (vendor != null)
                {
                    return new ServiceResponse<GetWhatsAppVendorByIDResponse>(true, "WhatsApp Vendor fetched successfully.", vendor, 200);
                }

                return new ServiceResponse<GetWhatsAppVendorByIDResponse>(false, "WhatsApp Vendor not found.", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetWhatsAppVendorByIDResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteWhatsAppVendor(int WhatsAppVendorID)
        {
            try
            {
                // Soft delete by updating IsActive to 0
                string query = @"UPDATE tblWhatsAppVendor 
                                 SET IsActive = 0 
                                 WHERE WhatsAppVendorID = @WhatsAppVendorID";

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { WhatsAppVendorID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "WhatsApp Vendor soft deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "WhatsApp Vendor not found or already inactive.", "Failure", 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdateWhatsAppPlan(AddUpdateWhatsAppPlanRequest request)
        {
            try
            {
                // If WhatsAppVendorID already exists, delete the previous plan
                if (request.WhatsAppVendorID != 0)
                {
                    string deleteQuery = @"DELETE FROM tblWhatsAppTopUpRate 
                                   WHERE WhatsAppVendorID = @WhatsAppVendorID AND IsDeleted = 0";

                    int rowsDeleted = await _dbConnection.ExecuteAsync(deleteQuery, new { request.WhatsAppVendorID });

                    // If any rows are deleted, continue with the update/insert
                    if (rowsDeleted > 0 || request.RateID == 0)
                    {
                        // Now insert or update the WhatsApp plan
                        string query = request.RateID == 0
                            ? @"INSERT INTO tblWhatsAppTopUpRate (WhatsAppVendorID, CreditCount, CreditAmount) 
                        VALUES (@WhatsAppVendorID, @CreditCount, @CreditAmount)"
                            : @"UPDATE tblWhatsAppTopUpRate 
                        SET WhatsAppVendorID = @WhatsAppVendorID, CreditCount = @CreditCount, CreditAmount = @CreditAmount 
                        WHERE RateID = @RateID";

                        int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                        {
                            request.RateID,
                            request.WhatsAppVendorID,
                            request.CreditCount,
                            request.CreditAmount
                        });

                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "WhatsApp Plan added/updated successfully.", "Success", 200);
                        }
                        return new ServiceResponse<string>(false, "Failed to add/update WhatsApp Plan.", "Failure", 400);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Failed to delete existing WhatsApp plan.", "Failure", 400);
                    }
                }

                // If WhatsAppVendorID is 0, it's a new insert
                else
                {
                    string query = @"INSERT INTO tblWhatsAppTopUpRate (WhatsAppVendorID, CreditCount, CreditAmount) 
                             VALUES (@WhatsAppVendorID, @CreditCount, @CreditAmount)";

                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        request.WhatsAppVendorID,
                        request.CreditCount,
                        request.CreditAmount
                    });

                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "WhatsApp Plan added successfully.", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to add WhatsApp Plan.", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>> GetAllWhatsAppPlan(GetAllWhatsAppPlanRequest request)
        {
            try
            {
                string query = @"
                    SELECT r.RateID, r.WhatsAppVendorID, r.CreditCount, r.CreditAmount, v.VendorName
                    FROM tblWhatsAppTopUpRate r
                    INNER JOIN tblWhatsAppVendor v ON r.WhatsAppVendorID = v.WhatsAppVendorID
                    WHERE (@WhatsAppVendorID = 0 OR r.WhatsAppVendorID = @WhatsAppVendorID) AND r.IsDeleted = 0";

                var plans = await _dbConnection.QueryAsync<GetAllWhatsAppPlanResponse>(query, new { request.WhatsAppVendorID });

                foreach (var plan in plans)
                {
                    plan.Plan = $"{plan.CreditCount} credits for ₹ {plan.CreditAmount}";
                }

                return new ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>(true, "WhatsApp Plans fetched successfully.", plans, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetWhatsAppPlanByIDResponse>> GetWhatsAppPlanByID(int WhatsAppVendorID)
        {
            try
            {
                string query = @"
                    SELECT r.RateID, r.WhatsAppVendorID, r.CreditCount, r.CreditAmount, v.VendorName
                    FROM tblWhatsAppTopUpRate r
                    INNER JOIN tblWhatsAppVendor v ON r.WhatsAppVendorID = v.WhatsAppVendorID
                    WHERE r.WhatsAppVendorID = @WhatsAppVendorID AND r.IsDeleted = 0";

                var plan = await _dbConnection.QueryFirstOrDefaultAsync<GetWhatsAppPlanByIDResponse>(query, new { WhatsAppVendorID });

                if (plan == null)
                {
                    return new ServiceResponse<GetWhatsAppPlanByIDResponse>(false, "WhatsApp Plan not found for the given Vendor ID", null, 404);
                }

                // Format the Plan string
                plan.Plan = $"{plan.CreditCount} credits for ₹ {plan.CreditAmount}";

                return new ServiceResponse<GetWhatsAppPlanByIDResponse>(true, "WhatsApp Plan fetched successfully", plan, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetWhatsAppPlanByIDResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteWhatsAppPlan(int RateID)
        {
            try
            {
                // SQL query to set IsDeleted = 1 for the given RateID
                string query = @"
                    UPDATE tblWhatsAppTopUpRate 
                    SET IsDeleted = 1
                    WHERE RateID = @RateID";

                // Execute the query
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { RateID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "WhatsApp Plan deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to delete WhatsApp Plan.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        //public async Task<ServiceResponse<string>> CreateWhatsAppOrder(CreateWhatsAppOrderRequest request)
        //{
        //    try
        //    {
        //        string query = @"
        //            INSERT INTO tblWhatsAppOrder (WhatsAppVendorID, InstituteID, TransactionID, TransactionAmount, TransactionDate, OrderStatus)
        //            VALUES (@WhatsAppVendorID, @InstituteID, @TransactionID, @TransactionAmount, @TransactionDate, @OrderStatus)";

        //        var rowsAffected = await _dbConnection.ExecuteAsync(query, new
        //        {
        //            request.WhatsAppVendorID,
        //            request.InstituteID,
        //            request.TransactionID,
        //            request.TransactionAmount,
        //            request.TransactionDate,
        //            request.OrderStatus
        //        });

        //        if (rowsAffected > 0)
        //        {
        //            return new ServiceResponse<string>(true, "WhatsApp Order created successfully.", "Success", 200);
        //        }

        //        return new ServiceResponse<string>(false, "Failed to create WhatsApp Order.", "Failure", 400);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, ex.Message, "Error", 500);
        //    }
        //}

        public async Task<ServiceResponse<string>> CreateWhatsAppOrder(CreateWhatsAppOrderRequest request)
        {
            try
            {
                // Modified the SQL query to include RateID
                string query = @"
            INSERT INTO tblWhatsAppOrder (WhatsAppVendorID, InstituteID, TransactionID, TransactionAmount, TransactionDate, OrderStatus, RateID)
            VALUES (@WhatsAppVendorID, @InstituteID, @TransactionID, @TransactionAmount, @TransactionDate, @OrderStatus, @RateID)"; // Include RateID in the insert query

                // Execute the query and pass the RateID from the request body
                var rowsAffected = await _dbConnection.ExecuteAsync(query, new
                {
                    request.WhatsAppVendorID,
                    request.InstituteID,
                    request.TransactionID,
                    request.TransactionAmount,
                    request.TransactionDate,
                    request.OrderStatus,
                    request.RateID // Pass RateID as part of the parameters
                });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "WhatsApp Order created successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to create WhatsApp Order.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>> GetWhatsAppOrder(GetWhatsAppOrderRequest request)
        {
            try
            {
                // Parse the startDate and endDate to DateTime
                DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null);
                DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);

                string query = @"
                    SELECT o.WhatsAppVendorID, v.VendorName, o.InstituteID, o.TransactionID, o.TransactionAmount, o.TransactionDate, o.OrderStatus, 
                           s.Status
                    FROM tblWhatsAppOrder o
                    INNER JOIN tblWhatsAppVendor v ON o.WhatsAppVendorID = v.WhatsAppVendorID
                    LEFT JOIN tblCPSMSStatus s ON o.OrderStatus = s.StatusID
                    WHERE o.TransactionDate BETWEEN @StartDate AND @EndDate
                    AND (@StatusID = 0 OR o.OrderStatus = @StatusID)";

                var orders = await _dbConnection.QueryAsync<GetWhatsAppOrderResponse>(query, new { StartDate = startDate, EndDate = endDate, StatusID = request.StatusID });

                return new ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>(
                    success: true,
                    message: "WhatsApp Orders fetched successfully.",
                    data: orders,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500
                );
            }
        }

    }
}
