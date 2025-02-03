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
    public class SMSRepository : ISMSRepository
    {
        private readonly IDbConnection _dbConnection;

        public SMSRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateSMSVendor(AddUpdateSMSVendorRequest request)
        {
            try
            {
                string query;

                if (request.SMSVendorID == 0)
                {
                    // Insert new SMS vendor
                    query = @"INSERT INTO tblSMSVendor (VendorName, PerSMSCost, Email, PhoneNumber, Address) 
                              VALUES (@VendorName, @PerSMSCost, @Email, @PhoneNumber, @Address)";
                }
                else
                {
                    // Update existing SMS vendor
                    query = @"UPDATE tblSMSVendor 
                              SET VendorName = @VendorName, PerSMSCost = @PerSMSCost, Email = @Email, 
                                  PhoneNumber = @PhoneNumber, Address = @Address 
                              WHERE SMSVendorID = @SMSVendorID";
                }

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                {
                    request.SMSVendorID,
                    request.VendorName,
                    request.PerSMSCost,
                    request.Email,
                    request.PhoneNumber,
                    request.Address
                });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "SMS Vendor added/updated successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to add/update SMS Vendor.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>> GetAllSMSVendor()
        {
            try
            {
                string query = @"SELECT SMSVendorID, VendorName, PerSMSCost, Email, PhoneNumber, Address
                                 FROM tblSMSVendor";

                var vendors = await _dbConnection.QueryAsync<GetAllSMSVendorResponse>(query);

                return new ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>(true, "SMS Vendors fetched successfully.", vendors, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetSMSVendorByIDResponse>> GetSMSVendorByID(int SMSVendorID)
        {
            try
            {
                string query = @"SELECT SMSVendorID, VendorName, PerSMSCost, Email, PhoneNumber, Address
                                 FROM tblSMSVendor
                                 WHERE SMSVendorID = @SMSVendorID";

                var vendor = await _dbConnection.QueryFirstOrDefaultAsync<GetSMSVendorByIDResponse>(query, new { SMSVendorID });

                if (vendor != null)
                {
                    return new ServiceResponse<GetSMSVendorByIDResponse>(true, "SMS Vendor fetched successfully.", vendor, 200);
                }

                return new ServiceResponse<GetSMSVendorByIDResponse>(false, "SMS Vendor not found.", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetSMSVendorByIDResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteSMSVendor(DeleteSMSVendorRequest request)
        {
            try
            {
                // Update IsActive to 0 for the given SMSVendorID to perform a soft delete
                string query = @"UPDATE tblSMSVendor 
                                 SET IsActive = 0 
                                 WHERE SMSVendorID = @SMSVendorID";

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { request.SMSVendorID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "SMS Vendor soft deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to soft delete SMS Vendor.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdateSMSPlan(AddUpdateSMSPlanRequest request)
        {
            try
            {
                // If SMSVendorID already exists, delete the previous plan
                if (request.SMSVendorID != 0)
                {
                    string deleteQuery = @"DELETE FROM tblSMSTopUpRate WHERE SMSVendorID = @SMSVendorID AND IsDeleted = 0";

                    int rowsDeleted = await _dbConnection.ExecuteAsync(deleteQuery, new { request.SMSVendorID });

                    // If any rows are deleted, continue with the update/insert
                    if (rowsDeleted > 0 || request.RateID == 0)
                    {
                        // Now insert or update the SMS plan
                        string query = request.RateID == 0
                            ? @"INSERT INTO tblSMSTopUpRate (SMSVendorID, CreditCount, CreditAmount) 
                        VALUES (@SMSVendorID, @CreditCount, @CreditAmount)"
                            : @"UPDATE tblSMSTopUpRate 
                        SET SMSVendorID = @SMSVendorID, CreditCount = @CreditCount, CreditAmount = @CreditAmount 
                        WHERE RateID = @RateID";

                        int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                        {
                            request.RateID,
                            request.SMSVendorID,
                            request.CreditCount,
                            request.CreditAmount
                        });

                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "SMS Plan added/updated successfully.", "Success", 200);
                        }
                        return new ServiceResponse<string>(false, "Failed to add/update SMS Plan.", "Failure", 400);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Failed to delete existing SMS plan.", "Failure", 400);
                    }
                }

                // If SMSVendorID is 0, it's a new insert
                else
                {
                    string query = @"INSERT INTO tblSMSTopUpRate (SMSVendorID, CreditCount, CreditAmount) 
                             VALUES (@SMSVendorID, @CreditCount, @CreditAmount)";

                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        request.SMSVendorID,
                        request.CreditCount,
                        request.CreditAmount
                    });

                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "SMS Plan added successfully.", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to add SMS Plan.", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }


        public async Task<IEnumerable<GetAllSMSPlanResponse>> GetAllSMSPlan(GetAllSMSPlanRequest request)
        {
            try
            {
                // SQL query to fetch all plans when smsVendorID is 0
                string query = @"
            SELECT r.RateID, r.SMSVendorID, r.CreditCount, r.CreditAmount, v.VendorName
            FROM tblSMSTopUpRate r
            INNER JOIN tblSMSVendor v ON r.SMSVendorID = v.SMSVendorID
            WHERE (@smsVendorID = 0 OR r.SMSVendorID = @smsVendorID) AND r.IsDeleted = 0";

                var plans = await _dbConnection.QueryAsync<GetAllSMSPlanResponse>(query, new { request.SMSVendorID });

                foreach (var plan in plans)
                {
                    plan.Plan = $"{plan.CreditCount} credits for ₹ {plan.CreditAmount}";
                }

                return plans;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching SMS Plans: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<GetSMSPlanByIDResponse>> GetSMSPlanByID(int SMSVendorID)
        {
            try
            {
                // SQL query to fetch the SMS plan based on SMSVendorID
                string query = @"
            SELECT r.RateID, r.SMSVendorID, r.CreditCount, r.CreditAmount, v.VendorName
            FROM tblSMSTopUpRate r
            INNER JOIN tblSMSVendor v ON r.SMSVendorID = v.SMSVendorID
            WHERE r.SMSVendorID = @SMSVendorID AND r.IsDeleted = 0";

                var plan = await _dbConnection.QueryFirstOrDefaultAsync<GetSMSPlanByIDResponse>(query, new { SMSVendorID });

                if (plan == null)
                {
                    return new ServiceResponse<GetSMSPlanByIDResponse>(false, "SMS Plan not found for the given Vendor ID", null, 404);
                }

                // Format the Plan string
                plan.Plan = $"{plan.CreditCount} credits for ₹ {plan.CreditAmount}";

                return new ServiceResponse<GetSMSPlanByIDResponse>(true, "SMS Plan fetched successfully", plan, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetSMSPlanByIDResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteSMSPlan(int rateID)
        {
            try
            {
                // SQL query to set IsDeleted = 1 for the given RateID
                string query = @"
                UPDATE tblSMSTopUpRate 
                SET IsDeleted = 1
                WHERE RateID = @RateID";

                // Execute the query
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { RateID = rateID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "SMS Plan deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to delete SMS Plan.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        //public async Task<ServiceResponse<string>> CreateSMSOrder(CreateSMSOrderRequest request)
        //{
        //    try
        //    {
        //        string query = @"
        //        INSERT INTO tblSMSOrder (SMSVendorID, InstituteID, TransactionID, TransactionAmount, TransactionDate, OrderStatus)
        //        VALUES (@SMSVendorID, @InstituteID, @TransactionID, @TransactionAmount, @TransactionDate, @OrderStatus)";

        //        var rowsAffected = await _dbConnection.ExecuteAsync(query, new
        //        {
        //            request.SMSVendorID,
        //            request.InstituteID,
        //            request.TransactionID,
        //            request.TransactionAmount,
        //            request.TransactionDate,
        //            request.OrderStatus
        //        });

        //        if (rowsAffected > 0)
        //        {
        //            return new ServiceResponse<string>(true, "SMS Order created successfully.", "Success", 200);
        //        }

        //        return new ServiceResponse<string>(false, "Failed to create SMS Order.", "Failure", 400);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, ex.Message, "Error", 500);
        //    }
        //}


        public async Task<ServiceResponse<string>> CreateSMSOrder(CreateSMSOrderRequest request)
        {
            try
            {
                string query = @"
                INSERT INTO tblSMSOrder (SMSVendorID, InstituteID, TransactionID, TransactionAmount, TransactionDate, OrderStatus, RateID)
                VALUES (@SMSVendorID, @InstituteID, @TransactionID, @TransactionAmount, @TransactionDate, @OrderStatus, @RateID)"; // Add RateID in the insert query

                var rowsAffected = await _dbConnection.ExecuteAsync(query, new
                {
                    request.SMSVendorID,
                    request.InstituteID,
                    request.TransactionID,
                    request.TransactionAmount,
                    request.TransactionDate,
                    request.OrderStatus,
                    request.RateID  // Pass RateID as part of the parameters
                });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "SMS Order created successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to create SMS Order.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetSMSOrderResponse>>> GetSMSOrder(DateTime startDate, DateTime endDate, int statusID)
        {
            try
            {
                // SQL query to fetch SMS order details based on StartDate, EndDate, and StatusID
                string query = @"
                    SELECT o.SMSVendorID, v.VendorName, o.InstituteID, o.TransactionID, o.TransactionAmount, o.TransactionDate, o.OrderStatus, 
                           s.Status
                    FROM tblSMSOrder o
                    INNER JOIN tblSMSVendor v ON o.SMSVendorID = v.SMSVendorID
                    LEFT JOIN tblCPSMSStatus s ON o.OrderStatus = s.StatusID
                    WHERE o.TransactionDate BETWEEN @StartDate AND @EndDate 
                    AND (@StatusID = 0 OR o.OrderStatus = @StatusID)";

                var orders = await _dbConnection.QueryAsync<GetSMSOrderResponse>(query, new { StartDate = startDate, EndDate = endDate, StatusID = statusID });

                return new ServiceResponse<IEnumerable<GetSMSOrderResponse>>(
                    success: true,
                    message: "SMS Orders fetched successfully.",
                    data: orders,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetSMSOrderResponse>>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500
                );
            }
        }

    }
}
