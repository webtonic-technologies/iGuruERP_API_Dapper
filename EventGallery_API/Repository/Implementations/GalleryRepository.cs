using Dapper;
using EventGallery_API.Models;
using EventGallery_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EventGallery_API.Repository.Implementations
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly IDbConnection _dbConnection;

        public GalleryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> UploadGalleryImage(int eventID, GalleryImage galleryImage)
        {
            var query = @"INSERT INTO tblGallery (EventID, InstituteID, FileName, IsActive) 
                  VALUES (@EventID, @InstituteID, @FileName, 1);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var galleryID = await _dbConnection.QuerySingleAsync<int>(query, new
            {
                EventID = eventID, // Use the eventID passed from the method argument
                InstituteID = galleryImage.InstituteID,
                FileName = galleryImage.FileName
            });

            return galleryID;
        }


        public async Task<GalleryImage> DownloadGalleryImage(int galleryID)
        {
            var query = "SELECT * FROM tblGallery WHERE GalleryID = @GalleryID AND IsActive = 1";
            return await _dbConnection.QuerySingleOrDefaultAsync<GalleryImage>(query, new { GalleryID = galleryID });
        }

        public async Task<List<GalleryImage>> DownloadAllGalleryImages(int eventID)
        {
            var query = @"SELECT * FROM tblGallery WHERE EventID = @EventID AND IsActive = 1";
            return (await _dbConnection.QueryAsync<GalleryImage>(query, new { EventID = eventID })).ToList();
        }

        public async Task<bool> DeleteGalleryImage(int galleryID)
        {
            var query = "UPDATE tblGallery SET IsActive = 0 WHERE GalleryID = @GalleryID";
            var result = await _dbConnection.ExecuteAsync(query, new { GalleryID = galleryID });
            return result > 0;
        }

        public async Task<List<GalleryImage>> GetAllGalleryImages(int eventID, int instituteID)
        {
            var query = "SELECT * FROM tblGallery WHERE EventID = @EventID AND InstituteID = @InstituteID AND IsActive = 1";
            return (await _dbConnection.QueryAsync<GalleryImage>(query, new { EventID = eventID, InstituteID = instituteID })).ToList();
        }

        public async Task<EventDetails> GetEventDetails(int eventID, int instituteID)
        {
            var query = @"
            SELECT e.EventID, e.EventName, 
            CONCAT(FORMAT(e.ScheduleDate, 'dd MMM, yyyy'), ' at ', FORMAT(e.ScheduleTime, 'hh:mm tt')) AS EventDate
            FROM tblEvent e
            WHERE e.EventID = @EventID AND e.InstituteID = @InstituteID";

            return await _dbConnection.QuerySingleOrDefaultAsync<EventDetails>(query, new { EventID = eventID, InstituteID = instituteID });
        }

        public async Task<List<EventGallery_API.DTOs.Responses.EventDetails>> GetAllEvents(int instituteID)
        {
            var query = @"
                SELECT EventID, EventName 
                FROM tblEvent 
                WHERE InstituteID = @InstituteID AND IsActive = 1 AND IsDelete = 0";

            var events = await _dbConnection.QueryAsync<EventGallery_API.DTOs.Responses.EventDetails>(query, new { InstituteID = instituteID });
            return events.ToList();
        }

    }
}
