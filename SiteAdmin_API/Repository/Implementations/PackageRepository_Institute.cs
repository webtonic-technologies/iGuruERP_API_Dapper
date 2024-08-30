using Dapper;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

namespace SiteAdmin_API.Repository.Implementations
{
    public class PackageRepository_Institute : IPackageRepository_Institute
    {
        private readonly IDbConnection _connection;

        public PackageRepository_Institute(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<PackageResponse_Institute>>> GetAllPackagesForInstitute()
        {
            try
            {
                string sql = "SELECT PackageID, PackageName, IsActive FROM tblPackage";
                var packages = await _connection.QueryAsync<PackageResponse_Institute>(sql);

                return new ServiceResponse<List<PackageResponse_Institute>>(true, "Packages retrieved successfully", packages.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PackageResponse_Institute>>(false, ex.Message, null, 500);
            }
        }
    }
}
