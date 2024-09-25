using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.Models;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Implementations
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;

        public GalleryService(IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
        }

        public async Task<ServiceResponse<int>> UploadGalleryImage(int eventID, GalleryImageRequest request)
        {
            var galleryImage = new GalleryImage
            {
                EventID = eventID,
                InstituteID = request.InstituteID,
                FileName = request.FileName
            };

            var galleryID = await _galleryRepository.UploadGalleryImage(eventID, galleryImage);
            return new ServiceResponse<int>(true, "Image uploaded successfully.", galleryID, 200);
        }

        public async Task<ServiceResponse<GalleryImageResponse>> DownloadGalleryImage(int galleryID)
        {
            var galleryImage = await _galleryRepository.DownloadGalleryImage(galleryID);
            if (galleryImage == null) return new ServiceResponse<GalleryImageResponse>(false, "Image not found.", null, 404);

            var response = new GalleryImageResponse
            {
                GalleryID = galleryImage.GalleryID,
                ImageName = galleryImage.FileName,
                ImageData = System.IO.File.ReadAllBytes($"Assets/Gallery/{galleryImage.FileName}")
            };

            return new ServiceResponse<GalleryImageResponse>(true, "Image downloaded successfully.", response, 200);
        }

        public async Task<ServiceResponse<List<GalleryImageResponse>>> DownloadAllGalleryImages(int eventID)
        {
            // Fetch all images from the repository
            var galleryImages = await _galleryRepository.DownloadAllGalleryImages(eventID);

            if (galleryImages == null || !galleryImages.Any())
            {
                return new ServiceResponse<List<GalleryImageResponse>>(false, "No images found.", null, 404);
            }

            // Convert the images to byte arrays for download
            var imageResponses = galleryImages.Select(img => new GalleryImageResponse
            {
                GalleryID = img.GalleryID,
                ImageName = img.FileName,
                ImageData = File.ReadAllBytes($"Assets/Gallery/{img.FileName}")
            }).ToList();

            return new ServiceResponse<List<GalleryImageResponse>>(true, "Images fetched successfully.", imageResponses, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteGalleryImage(int galleryID)
        {
            var result = await _galleryRepository.DeleteGalleryImage(galleryID);
            if (!result) return new ServiceResponse<bool>(false, "Image not found.", false, 404);

            return new ServiceResponse<bool>(true, "Image deleted successfully.", true, 200);
        }

        public async Task<ServiceResponse<List<GalleryImageResponse>>> GetAllGalleryImages(GalleryImageRequest request)
        {
            var galleryImages = await _galleryRepository.GetAllGalleryImages(request.EventID);
            var response = galleryImages.Select(img => new GalleryImageResponse
            {
                GalleryID = img.GalleryID,
                ImageName = img.FileName,
                ImageData = System.IO.File.ReadAllBytes($"Assets/Gallery/{img.FileName}")
            }).ToList();

            return new ServiceResponse<List<GalleryImageResponse>>(true, "All gallery images fetched successfully.", response, 200);
        }
    }
}
