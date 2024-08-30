using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;
using Institute_API.Models;
using System.Text;
using static Institute_API.Models.Enums;

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
                // Prepare a batch insert query
                string query = "INSERT INTO [dbo].[tbl_Gallery] (Event_id, Institute_id, FileName) VALUES ";
                var parameters = new DynamicParameters();

                // Iterate through each filename and build the query dynamically
                for (int i = 0; i < galleryDTO.FileName.Count; i++)
                {
                    string paramEventId = $"@EventId{i}";
                    string paramInstituteId = $"@InstituteId{i}";
                    string paramFileName = $"@FileName{i}";

                    query += $"({paramEventId}, {paramInstituteId}, {paramFileName}),";

                    parameters.Add(paramEventId, galleryDTO.EventId);
                    parameters.Add(paramInstituteId, galleryDTO.Institute_id);
                    parameters.Add(paramFileName, galleryDTO.FileName[i]);
                }

                // Remove the last comma and add a semicolon to end the query
                query = query.TrimEnd(',') + ";";

                // Execute the batch insert query
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
        public async Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, int Status, int userId)
        {
            try
            {

                if (!Enum.IsDefined(typeof(Status_Enum), Status))
                {
                    return new ServiceResponse<bool>(false, "Invalid status value", false, 400);
                }
                string query = @"
                UPDATE [dbo].[tbl_Gallery]
                SET Status = @Status , approvedBy = @UserId
                WHERE Gallery_id = @GalleryId";

                int rowsAffected = await _connection.ExecuteAsync(query, new { Status = Status, GalleryId = galleryId, UserId = userId });

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

        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent(int Institute_id, int Status ,int? pageSize = null, int? pageNumber = null)
        {

            try
            {
                string query = @"
            SELECT tbl_Gallery.Event_id, tbl_Gallery.FileName, tbl_Gallery.isApproved,tbl_Gallery.Status,ScheduleTime AS EventDateTime
            FROM [dbo].[tbl_Gallery]
INNER JOIN tbl_CreateEvent ON tbl_CreateEvent.Event_id = tbl_Gallery.Event_id
            WHERE Institute_id = @Institute_id AND Status = @Status AND isDelete = 0
            ORDER BY Event_id";

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    query += $@"
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;";
                }

                var images = await _connection.QueryAsync<Gallery>(query, new { Institute_id, Status, Offset = (pageNumber - 1) * pageSize, PageSize = pageSize });

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


        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent(int Institute_id, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                string query = @"
          SELECT tbl_Gallery.Event_id, tbl_Gallery.FileName, tbl_Gallery.isApproved,tbl_Gallery.Status,ScheduleTime AS EventDateTime
            FROM [dbo].[tbl_Gallery]
INNER JOIN tbl_CreateEvent ON tbl_CreateEvent.Event_id = tbl_Gallery.Event_id
            WHERE Institute_id = @Institute_id  AND isDelete = 0
            ORDER BY Event_id";


                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    query += $@"
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;";
                }

                var images = await _connection.QueryAsync<Gallery>(query, new { Institute_id, Offset = (pageNumber - 1) * pageSize, PageSize = pageSize });

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
        public async Task<ServiceResponse<bool>> DeleteGalleryImage(int Gallery_id)
        {
            try
            {
                string query = @"
                UPDATE [dbo].[tbl_Gallery]
                SET isDelete = 1
                WHERE Gallery_id = @Gallery_id";

                int rowsAffected = await _connection.ExecuteAsync(query, new { Gallery_id = Gallery_id });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Gallery image Deleted successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Issue Found while delete the image", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

    }
}
