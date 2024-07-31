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
    }
}
