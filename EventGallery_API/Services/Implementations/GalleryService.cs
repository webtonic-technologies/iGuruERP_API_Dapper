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
            // Create a new GalleryImage object to pass to the repository
            var galleryImage = new GalleryImageRequest
            {
                EventID = eventID,
                InstituteID = request.InstituteID,
                FileName = request.FileName,
                AcademicYearCode = request.AcademicYearCode // Pass the AcademicYearCode to the repository
            };

            // Upload the gallery image and get the gallery ID
            var galleryID = await _galleryRepository.UploadGalleryImage(eventID, galleryImage);

            // Return a response indicating the result of the upload
            return new ServiceResponse<int>(true, "Image uploaded successfully.", galleryID, 200);
        }


        //public async Task<ServiceResponse<int>> UploadGalleryImage(int eventID, GalleryImageRequest request)
        //{
        //    var galleryImage = new GalleryImage
        //    {
        //        EventID = eventID,
        //        InstituteID = request.InstituteID,
        //        FileName = request.FileName
        //    };

        //    var galleryID = await _galleryRepository.UploadGalleryImage(eventID, galleryImage);
        //    return new ServiceResponse<int>(true, "Image uploaded successfully.", galleryID, 200);
        //}

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

        public async Task<ServiceResponse<EventGalleryResponse>> GetAllGalleryImages(GalleryImageRequest_Get request)
        {
            // Fetch event details
            var eventDetails = await _galleryRepository.GetEventDetails(request.EventID, request.InstituteID);

            // Fetch gallery images associated with the event
            var galleryImages = await _galleryRepository.GetAllGalleryImages(request.EventID, request.InstituteID);

            // Map gallery images to the response format
            var galleryImageResponses = galleryImages.Select(img => new GalleryImageResponse
            {
                GalleryID = img.GalleryID,
                ImageName = img.FileName,
                ImageData = System.IO.File.ReadAllBytes($"Assets/Gallery/{img.FileName}")
            }).ToList();

            // Create the full response object
            var response = new EventGalleryResponse
            {
                EventID = eventDetails.EventID,
                EventName = eventDetails.EventName,
                EventDate = eventDetails.EventDate,
                GalleryImages = galleryImageResponses
            };

            return new ServiceResponse<EventGalleryResponse>(true, "Event gallery details fetched successfully.", response, 200);
        }



        //public async Task<List<EventGallery_API.DTOs.Responses.EventDetails>> GetAllEvents(int instituteID)
        //{
        //    return await _galleryRepository.GetAllEvents(instituteID);
        //}

        public async Task<List<EventDetailsList>> GetAllEvents(int instituteID, string academicYearCode)
        {
            // Pass the parameters to the repository to fetch events
            return await _galleryRepository.GetAllEvents(instituteID, academicYearCode);
        }
    }
}
