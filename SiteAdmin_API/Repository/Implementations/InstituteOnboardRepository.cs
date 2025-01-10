using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;
using System.Globalization;

namespace SiteAdmin_API.Repository.Implementations
{
    public class InstituteOnboardRepository : IInstituteOnboardRepository
    {
        private readonly IDbConnection _connection;

        public InstituteOnboardRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<InstituteOnboard>> AddUpdateInstituteOnboard(InstituteOnboardRequest request)
        {
            _connection.Open();  // Ensure the connection is open

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    int instituteOnboardId;

                    if (request.InstituteOnboardID.HasValue && request.InstituteOnboardID.Value > 0)
                    {
                        // Update existing institute
                        instituteOnboardId = request.InstituteOnboardID.Value;

                        string updateInstituteSql = @"UPDATE tblInstituteOnboard 
                                              SET InstituteOnboardName = @InstituteOnboardName, 
                                                  AliasName = @AliasName, 
                                                  CountryID = @CountryID, 
                                                  StateID = @StateID, 
                                                  City = @City, 
                                                  Pincode = @Pincode
                                              WHERE InstituteOnboardID = @InstituteOnboardID";

                        await _connection.ExecuteAsync(updateInstituteSql, new { InstituteOnboardID = instituteOnboardId, request.InstituteOnboardName, request.AliasName, request.CountryID, request.StateID, request.City, request.Pincode }, transaction);

                        // Delete existing related records
                        await _connection.ExecuteAsync("DELETE FROM tblInstituteOnboardContact WHERE InstituteOnboardID = @InstituteOnboardID", new { InstituteOnboardID = instituteOnboardId }, transaction);
                        await _connection.ExecuteAsync("DELETE FROM tblInstituteOnboardCredentials WHERE InstituteOnboardID = @InstituteOnboardID", new { InstituteOnboardID = instituteOnboardId }, transaction);
                        await _connection.ExecuteAsync("DELETE FROM tblInstitutePackage WHERE InstituteOnboardID = @InstituteOnboardID", new { InstituteOnboardID = instituteOnboardId }, transaction);
                    }
                    else
                    {
                        // Insert new institute
                        string insertInstituteSql = @"INSERT INTO tblInstituteOnboard (InstituteOnboardName, AliasName, CountryID, StateID, City, Pincode) 
                                              VALUES (@InstituteOnboardName, @AliasName, @CountryID, @StateID, @City, @Pincode);
                                              SELECT CAST(SCOPE_IDENTITY() as int)";

                        instituteOnboardId = await _connection.QuerySingleAsync<int>(insertInstituteSql, new { request.InstituteOnboardName, request.AliasName, request.CountryID, request.StateID, request.City, request.Pincode }, transaction);
                    }

                    // Insert into tblInstituteOnboardContact
                    string insertContactSql = @"INSERT INTO tblInstituteOnboardContact (InstituteOnboardID, PrimaryContactName, PrimaryTelephoneNumber, PrimaryMobileNumber, PrimaryEmailID, SecondaryContactName, SecondaryTelephoneNumber, SecondaryMobileNumber, SecondaryEmailID) 
                                        VALUES (@InstituteOnboardID, @PrimaryContactName, @PrimaryTelephoneNumber, @PrimaryMobileNumber, @PrimaryEmailID, @SecondaryContactName, @SecondaryTelephoneNumber, @SecondaryMobileNumber, @SecondaryEmailID)";

                    foreach (var contact in request.InstituteOnboardContacts)
                    {
                        await _connection.ExecuteAsync(insertContactSql, new { InstituteOnboardID = instituteOnboardId, contact.PrimaryContactName, contact.PrimaryTelephoneNumber, contact.PrimaryMobileNumber, contact.PrimaryEmailID, contact.SecondaryContactName, contact.SecondaryTelephoneNumber, contact.SecondaryMobileNumber, contact.SecondaryEmailID }, transaction);
                    }

                    // Insert into tblInstituteOnboardCredentials
                    string insertCredentialsSql = @"INSERT INTO tblInstituteOnboardCredentials (InstituteOnboardID, UserName, Password) 
                                            VALUES (@InstituteOnboardID, @UserName, @Password)";

                    foreach (var credential in request.InstituteOnboardCredentials)
                    {
                        await _connection.ExecuteAsync(insertCredentialsSql, new { InstituteOnboardID = instituteOnboardId, credential.UserName, credential.Password }, transaction);
                    }

                    // Insert into tblInstitutePackage
                    string insertPackageSql = @"INSERT INTO tblInstitutePackage (InstituteOnboardID, PackageID, MSG, PSPA, GST, TotalDealValue, SignUpDate, ValidUpto) 
                                        VALUES (@InstituteOnboardID, @PackageID, @MSG, @PSPA, @GST, @TotalDealValue, @SignUpDate, @ValidUpto)";

                    foreach (var package in request.InstitutePackages)
                    {
                        // Convert SignUpDate and ValidUpto to DateTime
                        DateTime signUpDate = DateTime.ParseExact(package.SignUpDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        DateTime validUpto = DateTime.ParseExact(package.ValidUpto, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        await _connection.ExecuteAsync(insertPackageSql, new { InstituteOnboardID = instituteOnboardId, package.PackageID, package.MSG, package.PSPA, package.GST, package.TotalDealValue, SignUpDate = signUpDate, ValidUpto = validUpto }, transaction);
                    }

                    // Commit transaction
                    transaction.Commit();

                    var instituteOnboard = new InstituteOnboard
                    {
                        InstituteOnboardID = instituteOnboardId,
                        InstituteOnboardName = request.InstituteOnboardName,
                        AliasName = request.AliasName,
                        CountryID = request.CountryID,
                        StateID = request.StateID,
                        City = request.City,
                        Pincode = request.Pincode,
                        InstituteOnboardContacts = request.InstituteOnboardContacts.Select(c => new InstituteOnboardContact
                        {
                            InstituteOnboardID = instituteOnboardId,
                            PrimaryContactName = c.PrimaryContactName,
                            PrimaryTelephoneNumber = c.PrimaryTelephoneNumber,
                            PrimaryMobileNumber = c.PrimaryMobileNumber,
                            PrimaryEmailID = c.PrimaryEmailID,
                            SecondaryContactName = c.SecondaryContactName,
                            SecondaryTelephoneNumber = c.SecondaryTelephoneNumber,
                            SecondaryMobileNumber = c.SecondaryMobileNumber,
                            SecondaryEmailID = c.SecondaryEmailID
                        }).ToList(),
                        InstituteOnboardCredentials = request.InstituteOnboardCredentials.Select(c => new InstituteOnboardCredentials
                        {
                            InstituteOnboardID = instituteOnboardId,
                            UserName = c.UserName,
                            Password = c.Password
                        }).ToList(),
                        InstitutePackages = request.InstitutePackages.Select(p => new InstitutePackage
                        {
                            InstituteOnboardID = instituteOnboardId,
                            PackageID = p.PackageID,
                            MSG = p.MSG,
                            PSPA = p.PSPA,
                            GST = p.GST,
                            TotalDealValue = p.TotalDealValue,
                            SignUpDate = p.SignUpDate,
                            ValidUpto = p.ValidUpto
                        }).ToList()
                    };

                    return new ServiceResponse<InstituteOnboard>(true, request.InstituteOnboardID.HasValue ? "Institute onboard updated successfully" : "Institute onboarded successfully", instituteOnboard, request.InstituteOnboardID.HasValue ? 200 : 201);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<InstituteOnboard>(false, ex.Message, null, 500);
                }
                finally
                {
                    _connection.Close();  // Ensure the connection is closed
                }
            }
        }


        public async Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard(int pageNumber, int pageSize)
        {
            try
            {
                // Calculate the OFFSET based on the page number and page size
                int offset = (pageNumber - 1) * pageSize;

                string sql = @"
            SELECT 
                io.InstituteOnboardID, 
                io.InstituteOnboardName, 
                io.AliasName, 
                io.CountryID, 
                io.StateID, 
                io.City, 
                io.Pincode
            FROM 
                tblInstituteOnboard io
            ORDER BY 
                io.InstituteOnboardID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT 
                ioc.ContactID, 
                ioc.InstituteOnboardID, 
                ioc.PrimaryContactName, 
                ioc.PrimaryTelephoneNumber, 
                ioc.PrimaryMobileNumber, 
                ioc.PrimaryEmailID, 
                ioc.SecondaryContactName, 
                ioc.SecondaryTelephoneNumber, 
                ioc.SecondaryMobileNumber, 
                ioc.SecondaryEmailID
            FROM 
                tblInstituteOnboardContact ioc;

            SELECT 
                iocr.CredentialID, 
                iocr.InstituteOnboardID, 
                iocr.UserName, 
                iocr.Password
            FROM 
                tblInstituteOnboardCredentials iocr;

            SELECT 
                ip.InstitutePackageID, 
                ip.InstituteOnboardID, 
                ip.PackageID, 
                ip.MSG, 
                ip.PSPA, 
                ip.GST, 
                ip.TotalDealValue, 
                ip.SignUpDate, 
                ip.ValidUpto
            FROM 
                tblInstitutePackage ip;
        ";

                using (var multi = await _connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize }))
                {
                    var institutes = (await multi.ReadAsync<InstituteOnboard>()).ToList();
                    var contacts = (await multi.ReadAsync<InstituteOnboardContact>()).ToList();
                    var credentials = (await multi.ReadAsync<InstituteOnboardCredentials>()).ToList();
                    var packages = (await multi.ReadAsync<InstitutePackage>()).ToList();

                    foreach (var institute in institutes)
                    {
                        institute.InstituteOnboardContacts = contacts.Where(c => c.InstituteOnboardID == institute.InstituteOnboardID).ToList();
                        institute.InstituteOnboardCredentials = credentials.Where(c => c.InstituteOnboardID == institute.InstituteOnboardID).ToList();
                        institute.InstitutePackages = packages.Where(p => p.InstituteOnboardID == institute.InstituteOnboardID).ToList();

                        // Format the SignUpDate and ValidUpto in 'DD-MM-YYYY' format for each InstitutePackage
                        foreach (var package in institute.InstitutePackages)
                        {
                            if (DateTime.TryParse(package.SignUpDate, out DateTime signUpDate))
                            {
                                package.SignUpDate = signUpDate.ToString("dd-MM-yyyy");
                            }

                            if (DateTime.TryParse(package.ValidUpto, out DateTime validUpto))
                            {
                                package.ValidUpto = validUpto.ToString("dd-MM-yyyy");
                            }
                        }
                    }

                    // Get the total count of institutes for pagination metadata
                    string countSql = "SELECT COUNT(*) FROM tblInstituteOnboard";
                    var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

                    return new ServiceResponse<List<InstituteOnboard>>(true, "Institutes onboard retrieved successfully", institutes, 200, totalCount);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InstituteOnboard>>(false, ex.Message, null, 500);
            }
        }



        //public async Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId)
        //{
        //    try
        //    {
        //        string sql = @"SELECT * FROM tblInstituteOnboard WHERE InstituteOnboardID = @InstituteOnboardID;
        //                       SELECT * FROM tblInstituteOnboardContact WHERE InstituteOnboardID = @InstituteOnboardID;
        //                       SELECT * FROM tblInstituteOnboardCredentials WHERE InstituteOnboardID = @InstituteOnboardID;
        //                       SELECT * FROM tblInstitutePackage WHERE InstituteOnboardID = @InstituteOnboardID";

        //        using (var multi = await _connection.QueryMultipleAsync(sql, new { InstituteOnboardID = instituteOnboardId }))
        //        {
        //            var institute = await multi.ReadSingleOrDefaultAsync<InstituteOnboard>();
        //            if (institute != null)
        //            {
        //                institute.InstituteOnboardContacts = (await multi.ReadAsync<InstituteOnboardContact>()).ToList();
        //                institute.InstituteOnboardCredentials = (await multi.ReadAsync<InstituteOnboardCredentials>()).ToList();
        //                institute.InstitutePackages = (await multi.ReadAsync<InstitutePackage>()).ToList();
        //            }

        //            return new ServiceResponse<InstituteOnboard>(true, "Institute onboard retrieved successfully", institute, 200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<InstituteOnboard>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId)
        {
            try
            {
                string sql = @"
            SELECT 
                io.InstituteOnboardID, 
                io.InstituteOnboardName, 
                io.AliasName, 
                io.CountryID, 
                io.StateID, 
                io.City, 
                io.Pincode
            FROM 
                tblInstituteOnboard io
            WHERE 
                io.InstituteOnboardID = @InstituteOnboardID;

            -- Join tblInstituteOnboardContact
            SELECT 
                ioc.ContactID, 
                ioc.InstituteOnboardID, 
                ioc.PrimaryContactName, 
                ioc.PrimaryTelephoneNumber, 
                ioc.PrimaryMobileNumber, 
                ioc.PrimaryEmailID, 
                ioc.SecondaryContactName, 
                ioc.SecondaryTelephoneNumber, 
                ioc.SecondaryMobileNumber, 
                ioc.SecondaryEmailID
            FROM 
                tblInstituteOnboardContact ioc
            WHERE 
                ioc.InstituteOnboardID = @InstituteOnboardID;

            -- Join tblInstituteOnboardCredentials
            SELECT 
                iocr.CredentialID, 
                iocr.InstituteOnboardID, 
                iocr.UserName, 
                iocr.Password
            FROM 
                tblInstituteOnboardCredentials iocr
            WHERE 
                iocr.InstituteOnboardID = @InstituteOnboardID;

            -- Join tblInstitutePackage and format dates
            SELECT 
                ip.InstitutePackageID, 
                ip.InstituteOnboardID, 
                ip.PackageID, 
                ip.MSG, 
                ip.PSPA, 
                ip.GST, 
                ip.TotalDealValue, 
                FORMAT(ip.SignUpDate, 'dd-MM-yyyy') AS SignUpDate, 
                FORMAT(ip.ValidUpto, 'dd-MM-yyyy') AS ValidUpto
            FROM 
                tblInstitutePackage ip
            WHERE 
                ip.InstituteOnboardID = @InstituteOnboardID;
        ";

                using (var multi = await _connection.QueryMultipleAsync(sql, new { InstituteOnboardID = instituteOnboardId }))
                {
                    // Read the first result set for InstituteOnboard
                    var institute = await multi.ReadSingleOrDefaultAsync<InstituteOnboard>();

                    if (institute != null)
                    {
                        // Read the remaining result sets and assign them to the InstituteOnboard object
                        institute.InstituteOnboardContacts = (await multi.ReadAsync<InstituteOnboardContact>()).ToList();
                        institute.InstituteOnboardCredentials = (await multi.ReadAsync<InstituteOnboardCredentials>()).ToList();
                        institute.InstitutePackages = (await multi.ReadAsync<InstitutePackage>()).ToList();
                    }

                    return new ServiceResponse<InstituteOnboard>(true, "Institute onboard retrieved successfully", institute, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteOnboard>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<string>> UpgradePackage(UpgradePackageRequest request)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Set the previous package to inactive
                    string updatePackageSql = @"UPDATE tblInstitutePackage 
                                                SET IsActive = 0 
                                                WHERE InstituteOnboardID = @InstituteOnboardID AND IsActive = 1";

                    await _connection.ExecuteAsync(updatePackageSql, new { request.InstituteOnboardID }, transaction);

                    // Insert the new package
                    string insertPackageSql = @"INSERT INTO tblInstitutePackage 
                                                (InstituteOnboardID, PackageID, MSG, PSPA, GST, TotalDealValue, SignUpDate, ValidUpto, Comment, IsActive) 
                                                VALUES 
                                                (@InstituteOnboardID, @PackageID, @MSG, @PSPA, @GST, @TotalDealValue, @SignUpDate, @ValidUpto, @Comment, 1)";

                    DateTime signUpDate = DateTime.ParseExact(request.SignUpDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    DateTime validUpto = DateTime.ParseExact(request.ValidUpto, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    await _connection.ExecuteAsync(insertPackageSql, new
                    {
                        request.InstituteOnboardID,
                        request.PackageID,
                        request.MSG,
                        request.PSPA,
                        request.GST,
                        request.TotalDealValue,
                        SignUpDate = signUpDate,
                        ValidUpto = validUpto,
                        request.Comment
                    }, transaction);

                    transaction.Commit();

                    return new ServiceResponse<string>(true, "Package upgraded successfully", "Success", 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, ex.Message, null, 500);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ServiceResponse<List<GetPackageDDLResponse>>> GetPackageDDL()
        {
            try
            {
                string sql = "SELECT PackageID, PackageName FROM tblPackage WHERE IsActive = 1";

                var packages = await _connection.QueryAsync<GetPackageDDLResponse>(sql);

                return new ServiceResponse<List<GetPackageDDLResponse>>(true, "Packages retrieved successfully", packages.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetPackageDDLResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetAllInstituteInfoResponse>> GetAllInstituteInfo(int instituteOnboardId)
        {
            try
            {
                string sql = @"
                    SELECT InstituteOnboardName, AliasName, CountryID, StateID, City, Pincode 
                    FROM tblInstituteOnboard
                    WHERE InstituteOnboardID = @InstituteOnboardID";

                var result = await _connection.QuerySingleOrDefaultAsync<GetAllInstituteInfoResponse>(sql, new { InstituteOnboardID = instituteOnboardId });

                if (result == null)
                {
                    return new ServiceResponse<GetAllInstituteInfoResponse>(false, "Institute not found", null, 404);
                }

                return new ServiceResponse<GetAllInstituteInfoResponse>(true, "Institute information retrieved successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetAllInstituteInfoResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddAdmissionURL(AddAdmissionURLRequest request)
        {
            _connection.Open();

            try
            {
                string sqlQuery = @"INSERT INTO tblInstituteAdmissionURL 
                                    (InstituteOnboardID, URL) 
                                    VALUES 
                                    (@InstituteOnboardID, @URL);
                                    SELECT CAST(SCOPE_IDENTITY() as INT);";

                var admissionURLID = await _connection.ExecuteScalarAsync<int>(sqlQuery, new
                { 
                    request.InstituteOnboardID,
                    request.URL
                });

                return new ServiceResponse<int>(true, "Admission URL added successfully.", admissionURLID, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<ServiceResponse<IEnumerable<ActivityLogsResponse>>> GetActivityLogs(int instituteOnboardId)
        {
            try
            {
                string sql = @"SELECT URL, FORMAT(CreationDate, 'dd MMM yyyy, hh:mm tt') AS CreationDate 
                       FROM tblInstituteAdmissionURL
                       WHERE InstituteOnboardID = @InstituteOnboardID AND IsActive = 1";

                // Get all matching activity logs
                var activityLogs = await _connection.QueryAsync<ActivityLogsResponse>(sql, new { InstituteOnboardID = instituteOnboardId });

                // Check if there are any results
                if (!activityLogs.Any())
                {
                    return new ServiceResponse<IEnumerable<ActivityLogsResponse>>(false, "No activity logs found", null, 404);
                }

                return new ServiceResponse<IEnumerable<ActivityLogsResponse>>(true, "Activity logs fetched successfully", activityLogs, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ActivityLogsResponse>>(false, ex.Message, null, 500);
            }
        }



    }
}
