using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

namespace SiteAdmin_API.Repository.Implementations
{
    public class CreatePackageRepository : ICreatePackageRepository
    {
        private readonly IDbConnection _connection;

        public CreatePackageRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<Package>> CreatePackage(CreatePackageRequest request)
        {
            _connection.Open();  // Ensure the connection is open

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Insert into tblPackage
                    string insertPackageSql = @"INSERT INTO tblPackage (PackageName, IsActive) 
                                                VALUES (@PackageName, 1);
                                                SELECT CAST(SCOPE_IDENTITY() as int)";

                    var packageId = await _connection.QuerySingleAsync<int>(insertPackageSql, new { request.PackageName }, transaction);

                    // Insert into tblPackageModuleMapping
                    string insertMappingSql = @"INSERT INTO tblPackageModuleMapping (PackageID, ModuleID, SubModuleID) 
                                                VALUES (@PackageID, @ModuleID, @SubModuleID)";

                    foreach (var mapping in request.PackageModuleMappings)
                    {
                        await _connection.ExecuteAsync(insertMappingSql, new { PackageID = packageId, mapping.ModuleID, mapping.SubModuleID }, transaction);
                    }

                    // Commit transaction
                    transaction.Commit();

                    var package = new Package
                    {
                        PackageID = packageId,
                        PackageName = request.PackageName,
                        IsActive = true,
                        PackageModuleMappings = request.PackageModuleMappings.Select(m => new PackageModuleMapping
                        {
                            PackageID = packageId,
                            ModuleID = m.ModuleID,
                            SubModuleID = m.SubModuleID
                        }).ToList()
                    };

                    return new ServiceResponse<Package>(true, "Package created successfully", package, 201);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<Package>(false, ex.Message, null, 500);
                }
                finally
                {
                    _connection.Close();  // Ensure the connection is closed
                }
            }
        }

        public async Task<ServiceResponse<Package>> UpdatePackage(UpdatePackageRequest request)
        {
            _connection.Open();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Update tblPackage
                    string updatePackageSql = @"UPDATE tblPackage 
                                                SET PackageName = @PackageName, 
                                                    IsActive = @IsActive
                                                WHERE PackageID = @PackageID";

                    await _connection.ExecuteAsync(updatePackageSql, new { request.PackageName, request.IsActive, request.PackageID }, transaction);

                    // Delete existing mappings
                    string deleteMappingSql = @"DELETE FROM tblPackageModuleMapping WHERE PackageID = @PackageID";
                    await _connection.ExecuteAsync(deleteMappingSql, new { PackageID = request.PackageID }, transaction);

                    // Insert new mappings
                    string insertMappingSql = @"INSERT INTO tblPackageModuleMapping (PackageID, ModuleID, SubModuleID) 
                                                VALUES (@PackageID, @ModuleID, @SubModuleID)";

                    foreach (var mapping in request.PackageModuleMappings)
                    {
                        await _connection.ExecuteAsync(insertMappingSql, new { PackageID = request.PackageID, mapping.ModuleID, mapping.SubModuleID }, transaction);
                    }

                    transaction.Commit();

                    var package = new Package
                    {
                        PackageID = request.PackageID,
                        PackageName = request.PackageName,
                        IsActive = request.IsActive,
                        PackageModuleMappings = request.PackageModuleMappings.Select(m => new PackageModuleMapping
                        {
                            PackageID = request.PackageID,
                            ModuleID = m.ModuleID,
                            SubModuleID = m.SubModuleID
                        }).ToList()
                    };

                    return new ServiceResponse<Package>(true, "Package updated successfully", package, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<Package>(false, ex.Message, null, 500);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePackageStatus(int packageId)
        {
            try
            {
                string query = "UPDATE tblPackage SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE PackageID = @PackageID";
                int rowsAffected = await _connection.ExecuteAsync(query, new { PackageID = packageId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Package status updated successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to update package status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
