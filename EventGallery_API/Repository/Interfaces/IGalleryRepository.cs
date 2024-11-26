using EventGallery_API.DTOs.Requests;
using EventGallery_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks; 


namespace EventGallery_API.Repository.Interfaces
{
    public interface IGalleryRepository
    {
        //Task<int> UploadGalleryImage(int eventID, GalleryImage galleryImage);
        Task<int> UploadGalleryImage(int eventID, GalleryImageRequest galleryImage);

        Task<GalleryImage> DownloadGalleryImage(int galleryID);
        Task<List<GalleryImage>> DownloadAllGalleryImages(int eventID);
        Task<bool> DeleteGalleryImage(int galleryID);

        Task<EventDetails> GetEventDetails(int eventID, int instituteID);

        Task<List<GalleryImage>> GetAllGalleryImages(int eventID, int instituteID);
        //Task<List<EventGallery_API.DTOs.Responses.EventDetails>> GetAllEvents(int instituteID); // Use full type name
        Task<List<EventDetailsList>> GetAllEvents(int instituteID, string academicYearCode);


    }
}
