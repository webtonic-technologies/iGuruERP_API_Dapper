using EventGallery_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Repository.Interfaces
{
    public interface IGalleryRepository
    {
        Task<int> UploadGalleryImage(int eventID, GalleryImage galleryImage);
        Task<GalleryImage> DownloadGalleryImage(int galleryID);
        Task<List<GalleryImage>> DownloadAllGalleryImages(int eventID);
        Task<bool> DeleteGalleryImage(int galleryID);
        Task<List<GalleryImage>> GetAllGalleryImages(int eventID);
    }
}
