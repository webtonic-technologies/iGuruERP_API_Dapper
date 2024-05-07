using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;
using Institute_API.Models;

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
                string query = @"
                INSERT INTO [dbo].[tbl_Gallery] (Event_id, FileName)
                VALUES (@EventId, @FileName);
                SELECT SCOPE_IDENTITY();";

                int galleryId = await _connection.ExecuteScalarAsync<int>(query, new { galleryDTO.EventId, galleryDTO.FileName });

                if (galleryId > 0)
                {
                    return new ServiceResponse<int>(true, "Gallery image added successfully", 0, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "Gallery image not added", 0, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, bool isApproved , int userId)
        {
            try
            {
                string query = @"
                UPDATE [dbo].[tbl_Gallery]
                SET isApproved = @IsApproved , approvedBy = @UserId
                WHERE Gallery_id = @GalleryId";

                int rowsAffected = await _connection.ExecuteAsync(query, new { IsApproved = isApproved, GalleryId = galleryId , UserId = userId });

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

        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent()
        {
            try
            {
                string query = @"
        SELECT Event_id, FileName
        FROM [dbo].[tbl_Gallery]
        WHERE isApproved = 1";

                var images = await _connection.QueryAsync<GalleryDTO>(query);

                var result = images.GroupBy(x => x.EventId)
                                   .Select(g => new GalleryEventDTO
                                   {
                                       EventId = g.Key,
                                       FileNames = g.Select(x => x.FileName).ToList()
                                   })
                                   .ToList();

                return new ServiceResponse<List<GalleryEventDTO>>(true, "Approved images retrieved successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GalleryEventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent()
        {
            try
            {
                string query = @"
        SELECT Event_id, FileName, isApproved
        FROM [dbo].[tbl_Gallery]";

                var images = await _connection.QueryAsync<Gallery>(query);

                var result = images.GroupBy(x => x.EventId)
                                   .Select(g => new GalleryEventDTO
                                   {
                                       EventId = g.Key,
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
