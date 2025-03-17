using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeeReceiptRepository : IFeeReceiptRepository
    {
        private readonly IDbConnection _connection;

        public FeeReceiptRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptComponentResponse>>> GetReceiptComponents(GetReceiptComponentRequest request)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                // This query joins the main component table with a subquery that returns mapped ComponentIDs for the given InstituteID.
                var query = @"
                    SELECT 
                        c.ComponentID, 
                        c.ComponentType,
                        CASE 
                            WHEN m.ComponentID IS NOT NULL THEN CAST(1 AS BIT) 
                            ELSE CAST(0 AS BIT) 
                        END AS [Status]
                    FROM tblFeeReceiptComponent c
                    LEFT JOIN (
                        SELECT DISTINCT m.ComponentID
                        FROM tblFeeReceiptComponentMapping m
                        INNER JOIN tblFeeReceiptConfiguration conf 
                            ON m.ConfigurationID = conf.ConfigurationID
                        WHERE conf.InstituteID = @InstituteID
                    ) m ON c.ComponentID = m.ComponentID
                ";

                var components = await _connection.QueryAsync<GetReceiptComponentResponse>(
                    query,
                    new { InstituteID = request.InstituteID }
                );

                return new ServiceResponse<IEnumerable<GetReceiptComponentResponse>>(
                    true,
                    "Components retrieved successfully",
                    components,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetReceiptComponentResponse>>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    500
                );
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptPropertyResponse>>> GetReceiptProperties(GetReceiptPropertyRequest request)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                var query = @"
                    SELECT 
                        p.PropertyID, 
                        p.PropertyType,
                        CASE 
                            WHEN m.PropertyID IS NOT NULL THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT)
                        END AS [Status]
                    FROM tblFeeReceiptProperty p
                    LEFT JOIN (
                        SELECT DISTINCT m.PropertyID
                        FROM tblFeeReceiptPropertyMapping m
                        INNER JOIN tblFeeReceiptConfiguration conf ON m.ConfigurationID = conf.ConfigurationID
                        WHERE conf.InstituteID = @InstituteID
                    ) m ON p.PropertyID = m.PropertyID";

                var properties = await _connection.QueryAsync<GetReceiptPropertyResponse>(
                    query,
                    new { InstituteID = request.InstituteID }
                );

                return new ServiceResponse<IEnumerable<GetReceiptPropertyResponse>>(
                    true,
                    "Properties retrieved successfully",
                    properties,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetReceiptPropertyResponse>>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    500
                );
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptLayoutResponse>>> GetReceiptLayouts()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                var query = @"SELECT LayoutID, LayoutType FROM tblFeeReceiptLayout";
                var layouts = await _connection.QueryAsync<GetReceiptLayoutResponse>(query);

                return new ServiceResponse<IEnumerable<GetReceiptLayoutResponse>>(
                    success: true,
                    message: "Layouts retrieved successfully",
                    data: layouts,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetReceiptLayoutResponse>>(
                    success: false,
                    message: $"Error: {ex.Message}",
                    data: null,
                    statusCode: 500
                );
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetReceiptTypeResponse>>> GetReceiptTypes()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                var query = @"SELECT ReceiptTypeID, ReceiptType FROM tblFeeReceiptType";
                var receiptTypes = await _connection.QueryAsync<GetReceiptTypeResponse>(query);

                return new ServiceResponse<IEnumerable<GetReceiptTypeResponse>>(
                    success: true,
                    message: "Receipt types retrieved successfully",
                    data: receiptTypes,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetReceiptTypeResponse>>(
                    success: false,
                    message: $"Error: {ex.Message}",
                    data: null,
                    statusCode: 500
                );
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }


        public async Task<ServiceResponse<bool>> ConfigureReceipt(ConfigureReceiptRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Remove existing mappings for the given InstituteID

                    // Delete component mappings for existing configurations
                    var deleteComponentMappingQuery = @"
                        DELETE FROM tblFeeReceiptComponentMapping 
                        WHERE ConfigurationID IN (
                            SELECT ConfigurationID FROM tblFeeReceiptConfiguration WHERE InstituteID = @InstituteID
                        )";
                    await _connection.ExecuteAsync(deleteComponentMappingQuery, new { request.InstituteID }, transaction);

                    // Delete property mappings for existing configurations
                    var deletePropertyMappingQuery = @"
                        DELETE FROM tblFeeReceiptPropertyMapping 
                        WHERE ConfigurationID IN (
                            SELECT ConfigurationID FROM tblFeeReceiptConfiguration WHERE InstituteID = @InstituteID
                        )";
                    await _connection.ExecuteAsync(deletePropertyMappingQuery, new { request.InstituteID }, transaction);

                    // Delete existing receipt configuration for the institute
                    var deleteConfigurationQuery = @"
                        DELETE FROM tblFeeReceiptConfiguration 
                        WHERE InstituteID = @InstituteID";
                    await _connection.ExecuteAsync(deleteConfigurationQuery, new { request.InstituteID }, transaction);

                    // Insert the new receipt configuration
                    var insertConfigurationQuery = @"
                        INSERT INTO tblFeeReceiptConfiguration (LayoutID, ReceiptTypeID, ReceiptLogo, InstituteID)
                        VALUES (@LayoutID, @ReceiptTypeID, @ReceiptLogo, @InstituteID);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
                    var configurationID = await _connection.ExecuteScalarAsync<int>(insertConfigurationQuery, new
                    {
                        request.LayoutID,
                        request.ReceiptTypeID,
                        request.ReceiptLogo,
                        request.InstituteID
                    }, transaction);

                    // Insert new component mappings
                    foreach (var componentID in request.Components)
                    {
                        var insertComponentMappingQuery = @"
                            INSERT INTO tblFeeReceiptComponentMapping (ConfigurationID, ComponentID)
                            VALUES (@ConfigurationID, @ComponentID)";
                        await _connection.ExecuteAsync(insertComponentMappingQuery, new { ConfigurationID = configurationID, ComponentID = componentID }, transaction);
                    }

                    // Insert new property mappings
                    foreach (var propertyID in request.Properties)
                    {
                        var insertPropertyMappingQuery = @"
                            INSERT INTO tblFeeReceiptPropertyMapping (ConfigurationID, PropertyID)
                            VALUES (@ConfigurationID, @PropertyID)";
                        await _connection.ExecuteAsync(insertPropertyMappingQuery, new { ConfigurationID = configurationID, PropertyID = propertyID }, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<bool>(true, "Receipt configuration updated successfully.", true, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<bool>(false, $"Error: {ex.Message}", false, 500);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }
    }
}
