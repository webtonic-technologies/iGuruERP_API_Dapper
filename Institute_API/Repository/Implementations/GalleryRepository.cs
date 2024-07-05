using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;
using Institute_API.Models;
using System.Text;

namespace Institute_API.Repository.Implementations
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly IDbConnection _connection;

        public GalleryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO)
        {
            try
            {
                // Validate the galleryDTO and its FileName list
                if (galleryDTO == null || galleryDTO.FileName == null || galleryDTO.FileName.Count == 0)
                {
                    return new ServiceResponse<int>(false, "No gallery images to add", 0, 400);
                }

                // Initialize a query builder for the batch insert
                var queryBuilder = new StringBuilder();
                queryBuilder.AppendLine("INSERT INTO [dbo].[tbl_Gallery] (Event_id, Institute_id, FileName) VALUES ");

                // Dynamic parameters for the query
                var parameters = new DynamicParameters();
                int paramIndex = 0;
                int recordsToInsert = 0;

                // Iterate through each filename
                foreach (var fileName in galleryDTO.FileName)
                {
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        // Append the SQL values clause with parameter placeholders
                        queryBuilder.AppendLine($"(@EventId{paramIndex}, @InstituteId{paramIndex}, @FileName{paramIndex}),");

                        // Add parameters
                        parameters.Add($"@EventId{paramIndex}", galleryDTO.EventId);
                        parameters.Add($"@InstituteId{paramIndex}", galleryDTO.Institute_id);
                        parameters.Add($"@FileName{paramIndex}", fileName);

                        paramIndex++;
                        recordsToInsert++;
                    }
                }

                if (recordsToInsert == 0)
                {
                    return new ServiceResponse<int>(false, "No valid gallery images to add", 0, 400);
                }

                // Remove the last comma
                queryBuilder.Length--;
                queryBuilder.Append(";");

                // Final SQL query
                string query = queryBuilder.ToString();

                // Execute the query
                int rowsAffected = await _connection.ExecuteAsync(query, parameters);

                // Check if the records were inserted successfully
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<int>(true, "Gallery images added successfully", rowsAffected, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "Gallery images not added", 0, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Error: {ex.Message}", 0, 500);
            }
            //try
            //{
            //    string query = @"
            //    INSERT INTO [dbo].[tbl_Gallery] (Event_id, FileName)
            //    VALUES (@EventId, @FileName);
            //    SELECT SCOPE_IDENTITY();";

            //    int galleryId = await _connection.ExecuteScalarAsync<int>(query, new { galleryDTO.EventId, galleryDTO.FileName });

            //    if (galleryId > 0)
            //    {
            //        return new ServiceResponse<int>(true, "Gallery image added successfully", galleryId, 200);
            //    }
            //    else
            //    {
            //        return new ServiceResponse<int>(false, "Gallery image not added", 0, 500);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return new ServiceResponse<int>(false, ex.Message, 0, 500);
            //}
        }
        public async Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, bool isApproved, int userId)
        {
            try
            {
                string query = @"
                UPDATE [dbo].[tbl_Gallery]
                SET isApproved = @IsApproved , approvedBy = @UserId
                WHERE Gallery_id = @GalleryId";

                int rowsAffected = await _connection.ExecuteAsync(query, new { IsApproved = isApproved, GalleryId = galleryId, UserId = userId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Gallery image approval status updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Gallery image not found", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent(int Institute_id)
        {
            try
            {
                string query = @"
        SELECT Event_id, FileName , isApproved
        FROM [dbo].[tbl_Gallery]
        WHERE isApproved = 1 AND Institute_id=@Institute_id";

                var images = await _connection.QueryAsync<Gallery>(query, new { Institute_id });

                var result = images.GroupBy(x => x.Event_id)
                                   .Select(g => new GalleryEventDTO
                                   {
                                       Event_id = g.Key,
                                       FileNames = g.Select(x => x.FileName).ToList(),
                                       IsApproved = g.All(x => x.IsApproved)
                                   })
                                   .ToList();

                return new ServiceResponse<List<GalleryEventDTO>>(true, "Approved images retrieved successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GalleryEventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent(int Institute_id)
        {
            try
            {
                string query = @"
        SELECT Event_id, FileName, isApproved
        FROM [dbo].[tbl_Gallery] WHERE Institute_id =@Institute_id";

                var images = await _connection.QueryAsync<Gallery>(query, new { Institute_id });

                var result = images.GroupBy(x => x.Event_id)
                                   .Select(g => new GalleryEventDTO
                                   {
                                       Event_id = g.Key,
                                       FileNames = g.Select(x => x.FileName).ToList(),
                                       IsApproved = g.All(x => x.IsApproved)
                                   })
                                   .ToList();

                return new ServiceResponse<List<GalleryEventDTO>>(true, "Gallery images retrieved successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GalleryEventDTO>>(false, ex.Message, null, 500);
            }
        }

    }
}
