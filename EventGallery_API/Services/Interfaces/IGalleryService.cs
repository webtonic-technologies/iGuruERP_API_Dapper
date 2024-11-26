using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Interfaces
{
    public interface IGalleryService
    {
        //Task<ServiceResponse<int>> UploadGalleryImage(int eventID, GalleryImageRequest request);
        Task<ServiceResponse<int>> UploadGalleryImage(int eventID, GalleryImageRequest request);

        Task<ServiceResponse<GalleryImageResponse>> DownloadGalleryImage(int galleryID);
        Task<ServiceResponse<List<GalleryImageResponse>>> DownloadAllGalleryImages(int eventID);
        Task<ServiceResponse<bool>> DeleteGalleryImage(int galleryID);
        Task<ServiceResponse<EventGalleryResponse>> GetAllGalleryImages(GalleryImageRequest_Get request);
        //Task<List<EventGallery_API.DTOs.Responses.EventDetails>> GetAllEvents(int instituteID); // Specify full namespace
        Task<List<EventDetailsList>> GetAllEvents(int instituteID, string academicYearCode);

    }
}
