using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

namespace SiteAdmin_API.Repository.Implementations
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IDbConnection _connection;

        public PackageRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<Package>> AddUpdatePackage(AddUpdatePackageRequest request)
        {
            _connection.Open();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    int packageId;
                    if (request.PackageID.HasValue && request.PackageID.Value > 0)
                    {
                        // Update existing package
                        string updatePackageSql = @"UPDATE tblPackage 
                                                    SET PackageName = @PackageName, 
                                                        IsActive = @IsActive
                                                    WHERE PackageID = @PackageID";

                        await _connection.ExecuteAsync(updatePackageSql, new { request.PackageName, request.IsActive, request.PackageID }, transaction);

                        packageId = request.PackageID.Value;

                        // Delete existing mappings
                        string deleteMappingSql = @"DELETE FROM tblPackageModuleMapping WHERE PackageID = @PackageID";
                        await _connection.ExecuteAsync(deleteMappingSql, new { PackageID = packageId }, transaction);
                    }
                    else
                    {
                        // Insert new package
                        string insertPackageSql = @"INSERT INTO tblPackage (PackageName, IsActive) 
                                                    VALUES (@PackageName, @IsActive);
                                                    SELECT CAST(SCOPE_IDENTITY() as int)";

                        packageId = await _connection.QuerySingleAsync<int>(insertPackageSql, new { request.PackageName, request.IsActive }, transaction);
                    }

                    // Insert new mappings
                    string insertMappingSql = @"INSERT INTO tblPackageModuleMapping (PackageID, ModuleID, SubModuleID) 
                                                VALUES (@PackageID, @ModuleID, @SubModuleID)";

                    foreach (var mapping in request.PackageModuleMappings)
                    {
                        await _connection.ExecuteAsync(insertMappingSql, new { PackageID = packageId, mapping.ModuleID, mapping.SubModuleID }, transaction);
                    }

                    transaction.Commit();

                    var package = new Package
                    {
                        PackageID = packageId,
                        PackageName = request.PackageName,
                        IsActive = request.IsActive,
                        PackageModuleMappings = request.PackageModuleMappings.Select(m => new PackageModuleMapping
                        {
                            PackageID = packageId,
                            ModuleID = m.ModuleID,
                            SubModuleID = m.SubModuleID
                        }).ToList()
                    };

                    return new ServiceResponse<Package>(true, "Package added/updated successfully", package, 200);
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

        public async Task<ServiceResponse<List<GetPackage>>> GetAllPackages()
        {
            try
            {
                // SQL query with JOIN to fetch packages and modules
                string sql = @"
            SELECT p.PackageID, p.PackageName, p.IsActive, 
                   pm.PMMID, pm.PackageID, pm.ModuleID, 
                   m.ModuleID, m.ModuleName
            FROM tblPackage p
            LEFT JOIN tblPackageModuleMapping pm ON p.PackageID = pm.PackageID
            LEFT JOIN tblModules m ON pm.ModuleID = m.ModuleID";

                var result = await _connection.QueryAsync<GetPackage, Modules, GetPackage>(
                    sql,
                    (package, module) =>
                    {
                        // Ensure the modules list is initialized
                        if (package.Modules == null)
                            package.Modules = new List<Modules>();

                        // Add module details to the package's Modules list
                        if (module != null)
                        {
                            // Add unique modules to avoid duplicates
                            if (!package.Modules.Any(m => m.ModuleID == module.ModuleID))
                            {
                                package.Modules.Add(module);
                            }
                        }

                        return package;  // Return the package object
                    },
                    splitOn: "ModuleID"  // Tell Dapper where to split the result set (for Modules)
                );

                // Group packages by PackageName to aggregate modules under the same package
                var groupedPackages = result
                    .GroupBy(p => p.PackageName)  // Group by PackageName
                    .Select(g => new GetPackage
                    {
                        PackageID = g.First().PackageID,
                        PackageName = g.Key,
                        IsActive = g.First().IsActive,
                        Modules = g.SelectMany(x => x.Modules).Distinct().ToList()  // Aggregate modules for each package and remove duplicates
                    })
                    .ToList();

                return new ServiceResponse<List<GetPackage>>(true, "Packages retrieved successfully", groupedPackages, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetPackage>>(false, ex.Message, null, 500);
            }
        }



        //public async Task<ServiceResponse<List<Package>>> GetAllPackages()
        //{
        //    try
        //    {
        //        string sql = @"SELECT * FROM tblPackage;
        //                       SELECT * FROM tblPackageModuleMapping";

        //        using (var multi = await _connection.QueryMultipleAsync(sql))
        //        {
        //            var packages = (await multi.ReadAsync<Package>()).ToList();
        //            var mappings = (await multi.ReadAsync<PackageModuleMapping>()).ToList();

        //            foreach (var package in packages)
        //            {
        //                package.PackageModuleMappings = mappings.Where(m => m.PackageID == package.PackageID).ToList();
        //            }

        //            return new ServiceResponse<List<Package>>(true, "Packages retrieved successfully", packages, 200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<Package>>(false, ex.Message, null, 500);
        //    }
        //}

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
