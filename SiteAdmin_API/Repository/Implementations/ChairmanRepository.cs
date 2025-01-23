using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace SiteAdmin_API.Repository.Implementations
{
    public class ChairmanRepository : IChairmanRepository
    {
        private readonly IDbConnection _dbConnection;

        public ChairmanRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public async Task<ServiceResponse<CreateUserResponse>> CreateUserLoginInfo(CreateUserRequest request)
        {
            var response = new ServiceResponse<CreateUserResponse>(true, string.Empty, null, 200);
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.MobileNumber))
                {
                    response.Success = false;
                    response.Message = "Invalid input. Name, MobileNumber are required.";
                    return response;
                }

                // Generate username by combining Name and last 4 digits of MobileNumber
                string username = GenerateUsername(request.Name, request.MobileNumber);

                // Define a common password
                string commonPassword = "123456";

                // Prepare the response
                response.Success = true;
                response.Message = "User created successfully.";
                response.Data = new CreateUserResponse
                {
                    UserName = username,
                    Password = commonPassword
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating user login info: {ex.Message}");

                // Return failure response
                response.Success = false;
                response.Message = "An error occurred while creating the user.";
            }

            return response;
        }

        private string GenerateUsername(string name, string mobileNumber)
        {
            // Extract the last 4 digits of the mobile number
            string mobileSuffix = mobileNumber.Length >= 4 ? mobileNumber[^4..] : mobileNumber;

            // Combine name and mobile suffix
            string username = $"{name.Replace(" ", "").ToLower()}{mobileSuffix}";

            // Ensure username length is within limits
            return username.Length > 10 ? username.Substring(0, 10) : username;
        }
        public async Task<ServiceResponse<string>> AddUpdateChairman(AddUpdateChairmanRequest request)
        {
            try
            {
                int chairmanID;

                // Insert or update the chairman in tblInstituteChairman
                if (request.ChairmanID == 0)
                {
                    string query = @"INSERT INTO tblInstituteChairman (Name, MobileNumber, EmailID, UserName, Password) 
                             VALUES (@Name, @MobileNumber, @EmailID, @UserName, @Password);
                             SELECT CAST(SCOPE_IDENTITY() as int);";  // Retrieve the newly inserted ChairmanID

                    chairmanID = await _dbConnection.ExecuteScalarAsync<int>(query, new
                    {
                        request.Name,
                        request.MobileNumber,
                        request.EmailID,
                        request.UserName,
                        request.Password
                    });
                    // SQL query to insert login information
                    string insertLoginSql = @"
                    INSERT INTO [tblLoginInformationMaster] 
                    ([UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity])
                    VALUES (@UserId, @UserType, @UserName, @Password, @InstituteId, NULL)";
                    foreach (var data in request.Institutes)
                    {
                        // Insert login information into the database
                        await _dbConnection.ExecuteAsync(insertLoginSql, new
                        {
                            UserId = chairmanID,
                            UserType = 3,
                            UserName = request.UserName,
                            Password = request.Password,
                            InstituteId = data.InstituteID
                        });
                    }
                }
                else
                {
                    string query = @"UPDATE tblInstituteChairman 
                             SET Name = @Name, MobileNumber = @MobileNumber, EmailID = @EmailID, 
                                 UserName = @UserName, Password = @Password 
                             WHERE ChairmanID = @ChairmanID";

                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        request.ChairmanID,
                        request.Name,
                        request.MobileNumber,
                        request.EmailID,
                        request.UserName,
                        request.Password
                    });

                    chairmanID = request.ChairmanID;  // Use the existing ChairmanID if updating
                }

                // Check if chairman insert/update was successful
                if (chairmanID > 0)
                {
                    // Delete existing associations for this ChairmanID
                    string deleteAssociationsQuery = @"DELETE FROM tblInstituteChairmanAssociation WHERE ChairmanID = @ChairmanID";
                    await _dbConnection.ExecuteAsync(deleteAssociationsQuery, new { ChairmanID = chairmanID });

                    // Insert the new associations into tblInstituteChairmanAssociation
                    foreach (var institute in request.Institutes)
                    {
                        string associationQuery = @"INSERT INTO tblInstituteChairmanAssociation (ChairmanID, InstituteID) 
                                            VALUES (@ChairmanID, @InstituteID)";

                        await _dbConnection.ExecuteAsync(associationQuery, new
                        {
                            ChairmanID = chairmanID,  // Use the correct ChairmanID
                            InstituteID = institute.InstituteID
                        });
                    }
                    //if (request.ChairmanID == 0)
                    //{
                    //    var userlog = await CreateUserLoginInfo(chairmanID, 3, request.Institutes);
                    //}
                    return new ServiceResponse<string>(true, "Chairman added/updated successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to add/update chairman.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }
        private async Task<bool> CreateUserLoginInfo(int userId, int userType, List<InstituteRequest> instituteIds)
        {
            try
            {
                _dbConnection.Open(); // Ensure async method for opening
                string commonPassword = "123456"; // Define common password

                // SQL query to fetch chairman details
                string chairmanSql = @"
            SELECT TOP (1) [ChairmanID], [Name]
            FROM [tblInstituteChairman]
            WHERE [ChairmanID] = @UserId";

                dynamic userDetails = null;

                // Fetch chairman details
                if (userType == 3) // Chairman
                {
                    userDetails = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(chairmanSql, new { UserId = userId });
                }

                if (userDetails == null || instituteIds == null || !instituteIds.Any())
                {
                    return false; // User details not found or no institutes provided
                }

                foreach (var institute in instituteIds)
                {
                    // Fetch institute name
                    string instituteSql = @"SELECT Institute_name FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id;";
                    var instituteName = await _dbConnection.QueryFirstOrDefaultAsync<string>(instituteSql, new { Institute_id = institute.InstituteID });

                    if (string.IsNullOrEmpty(instituteName))
                    {
                        continue; // Skip if institute not found
                    }

                    // Generate username for chairman
                    string username = await GenerateUsername(instituteName, institute.InstituteID, "C", userId);

                    if (!string.IsNullOrEmpty(username))
                    {
                        // Ensure the username is unique
                        username = await EnsureUniqueUsername(username);

                        // SQL query to insert login information
                        string insertLoginSql = @"
                    INSERT INTO [tblLoginInformationMaster] 
                    ([UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity])
                    VALUES (@UserId, @UserType, @UserName, @Password, @InstituteId, NULL)";

                        // Insert login information into the database
                        await _dbConnection.ExecuteAsync(insertLoginSql, new
                        {
                            UserId = userId,
                            UserType = userType,
                            UserName = username,
                            Password = commonPassword,
                            InstituteId = institute.InstituteID
                        });
                    }
                }

                return true; // Operation successful
            }
            catch (Exception ex)
            {
                // Log the exception and return false to indicate failure
                Console.WriteLine($"Error creating user login info: {ex.Message}");
                return false;
            }
        }
        //private async Task<bool> CreateUserLoginInfo(int userId, int userType, List<InstituteRequest> instituteIds)
        //{
        //    try
        //    {
        //       // var connection = new SqlConnection(_connectionString);
        //         _dbConnection.Open(); // Ensure async method for opening

        //        // Define common password
        //        string commonPassword = "123456";

        //        // SQL queries for fetching user details based on UserType
        //        string employeeSql = @"
        //    SELECT TOP (1) [Employee_id], [First_Name], [Last_Name]
        //    FROM [tbl_EmployeeProfileMaster]
        //    WHERE [Employee_id] = @UserId";

        //        string studentSql = @"
        //    SELECT TOP (1) [student_id], [First_Name], [Last_Name]
        //    FROM [tbl_StudentMaster]
        //    WHERE [student_id] = @UserId";

        //        // Initialize variables
        //        string username = null;
        //        dynamic userDetails = null;
        //        string institutesql = @"select Institute_name from tbl_InstituteDetails where Institute_id = @Institute_id;";
        //        var instituteName = await _dbConnection.QueryFirstOrDefaultAsync<string>(institutesql, new { Institute_id = instituteId });

        //        // Fetch user details based on the UserType
        //        if (userType == 1) // Employee
        //        {
        //            userDetails = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(employeeSql, new { UserId = userId });
        //            if (userDetails != null)
        //            {
        //                // Generate username for employee
        //                username = await GenerateUsername(instituteName, instituteId, "E", userId);
        //            }
        //        }
        //        else if (userType == 2) // Student
        //        {
        //            userDetails = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(studentSql, new { UserId = userId });
        //            if (userDetails != null)
        //            {
        //                // Generate username for student
        //                username = await GenerateUsername(instituteName, instituteId, "S", userId);
        //            }
        //        }

        //        if (username != null)
        //        {
        //            // Ensure the username is unique
        //            username = await EnsureUniqueUsername(username);

        //            // SQL query to insert login information
        //            string insertLoginSql = @"
        //        INSERT INTO [tblLoginInformationMaster] 
        //        ([UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity])
        //        VALUES (@UserId, @UserType, @UserName, @Password, @InstituteId, NULL)";

        //            // Insert login information into the database
        //            await _dbConnection.ExecuteAsync(insertLoginSql, new
        //            {
        //                UserId = userId,
        //                UserType = userType,
        //                UserName = username,
        //                Password = commonPassword,
        //                InstituteId = instituteId
        //            });

        //            return true; // Operation successful
        //        }

        //        return false; // User details not found or unable to create login info
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and return false to indicate failure
        //        Console.WriteLine($"Error creating user login info: {ex.Message}");
        //        return false;
        //    }
        //}
        private async Task<string> GenerateUsername(string instituteName, int instituteId, string roleIdentifier, int userId)
        {
            // Step 1: Get the first four letters of the institute name
            string institutePrefix = GetInstitutePrefix(instituteName);

            // Step 2: Concatenate the Institute ID and Role Identifier
            string baseUsername = $"{institutePrefix}{instituteId}{roleIdentifier}";

            // Step 3: Append the sequence number (dynamic based on user type and role)
            int sequenceNumber = await GetNextSequenceNumber(instituteId, roleIdentifier);

            // Return the generated username
            return $"{baseUsername}{sequenceNumber}";
        }
        private string GetInstitutePrefix(string instituteName)
        {
            // Logic to extract the first four meaningful letters from the institute name
            var words = instituteName.Split(' ');
            string prefix = string.Empty;

            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word) && prefix.Length < 4)
                {
                    prefix += word[0].ToString().ToUpper();
                }
            }

            // Ensure the prefix is exactly 4 characters
            return prefix.PadRight(4, 'X').Substring(0, 4);
        }
        private async Task<int> GetNextSequenceNumber(int instituteId, string roleIdentifier)
        {
           // var connection = new SqlConnection(_connectionString);

            // SQL query to get the current max sequence number for the given institute and role
            string sequenceSql = @"
        SELECT ISNULL(MAX(CAST(SUBSTRING(UserName, LEN(UserName) - LEN(@RoleIdentifier) + 1, LEN(@RoleIdentifier)) AS INT)), 0)
        FROM [tblLoginInformationMaster]
        WHERE InstituteId = @InstituteId AND UserName LIKE @Prefix + '%'";

            string prefix = $"{roleIdentifier}{instituteId}";

            int currentMaxSequence = await _dbConnection.ExecuteScalarAsync<int>(sequenceSql, new
            {
                InstituteId = instituteId,
                RoleIdentifier = roleIdentifier,
                Prefix = prefix
            });

            return currentMaxSequence + 1; // Return the next sequence number
        }
        private async Task<string> EnsureUniqueUsername(string baseUsername)
        {
           // var connection = new SqlConnection(_connectionString);
            //_dbConnection.Open();
            // Define the SQL query to check if the username exists
            string checkUsernameSql = @"
        SELECT COUNT(1)
        FROM [tblLoginInformationMaster]
        WHERE [UserName] = @UserName";

            string uniqueUsername = baseUsername;
            int suffix = 1;

            // Check if the username already exists
            while (await _dbConnection.ExecuteScalarAsync<int>(checkUsernameSql, new { UserName = uniqueUsername }) > 0)
            {
                // Append a numeric suffix to make the username unique
                uniqueUsername = $"{baseUsername}{suffix}";
                suffix++;
            }

            return uniqueUsername; // Return the unique username
        }
        public async Task<ServiceResponse<IEnumerable<GetAllChairmanResponse>>> GetAllChairman()
        {
            var query = @"
                SELECT c.Name, c.MobileNumber, c.EmailID, c.UserName, c.Password,
                    i.InstituteOnboardID AS InstituteID, i.InstituteOnboardName AS InstituteName
                FROM tblInstituteChairman c
                LEFT JOIN tblInstituteChairmanAssociation ca ON c.ChairmanID = ca.ChairmanID
                LEFT JOIN tblInstituteOnboard i ON ca.InstituteID = i.InstituteOnboardID";

            var chairmen = await _dbConnection.QueryAsync<GetAllChairmanResponse, InstituteResponse, GetAllChairmanResponse>(
                query,
                (chairman, institute) =>
                {
                    chairman.Institutes = chairman.Institutes ?? new List<InstituteResponse>();
                    chairman.Institutes.Add(institute);
                    return chairman;
                },
                splitOn: "InstituteID");

            return new ServiceResponse<IEnumerable<GetAllChairmanResponse>>(true, "Chairmen retrieved successfully", chairmen, 200);
        }
        public async Task<ServiceResponse<string>> DeleteChairman(int chairmanID)
        {
            try
            {
                string query = @"UPDATE tblInstituteChairman
                                 SET IsActive = 0
                                 WHERE ChairmanID = @ChairmanID";

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { ChairmanID = chairmanID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Chairman soft-deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to soft-delete chairman.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }
        public async Task<ServiceResponse<List<GetInstitutesDDLResponse>>> GetInstitutesDDL()
        {
            try
            {
                string sql = "SELECT InstituteOnboardID, InstituteOnboardName FROM tblInstituteOnboard";

                var institutes = await _dbConnection.QueryAsync<GetInstitutesDDLResponse>(sql);

                return new ServiceResponse<List<GetInstitutesDDLResponse>>(true, "Institutes retrieved successfully", institutes.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetInstitutesDDLResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}