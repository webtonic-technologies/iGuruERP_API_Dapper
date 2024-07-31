using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

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
                        await _connection.ExecuteAsync(insertPackageSql, new { InstituteOnboardID = instituteOnboardId, package.PackageID, package.MSG, package.PSPA, package.GST, package.TotalDealValue, package.SignUpDate, package.ValidUpto }, transaction);
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

        public async Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard()
        {
            try
            {
                string sql = @"SELECT * FROM tblInstituteOnboard;
                               SELECT * FROM tblInstituteOnboardContact;
                               SELECT * FROM tblInstituteOnboardCredentials;
                               SELECT * FROM tblInstitutePackage";

                using (var multi = await _connection.QueryMultipleAsync(sql))
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
                    }

                    return new ServiceResponse<List<InstituteOnboard>>(true, "All institutes onboard retrieved successfully", institutes, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InstituteOnboard>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId)
        {
            try
            {
                string sql = @"SELECT * FROM tblInstituteOnboard WHERE InstituteOnboardID = @InstituteOnboardID;
                               SELECT * FROM tblInstituteOnboardContact WHERE InstituteOnboardID = @InstituteOnboardID;
                               SELECT * FROM tblInstituteOnboardCredentials WHERE InstituteOnboardID = @InstituteOnboardID;
                               SELECT * FROM tblInstitutePackage WHERE InstituteOnboardID = @InstituteOnboardID";

                using (var multi = await _connection.QueryMultipleAsync(sql, new { InstituteOnboardID = instituteOnboardId }))
                {
                    var institute = await multi.ReadSingleOrDefaultAsync<InstituteOnboard>();
                    if (institute != null)
                    {
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
    }
}
