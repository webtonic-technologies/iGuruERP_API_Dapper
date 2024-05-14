using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Institute_API.Repository.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;

        public GalleryService(IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
        }

        public async Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO)
        {
            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + galleryDTO.File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await galleryDTO.File.CopyToAsync(fileStream);
                }
                galleryDTO.FileName = uniqueFileName;
                var data = await _galleryRepository.AddGalleryImage(galleryDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent()
        {
            try
            {
                var response = await _galleryRepository.GetApprovedImagesByEvent();

                if (response.Success)
                {
                    foreach (var galleryEvent in response.Data)
                    {
                        for (int i = 0; i < galleryEvent.FileNames.Count; i++)
                        {
                            galleryEvent.FileNames[i] = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery", galleryEvent.FileNames[i]);
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GalleryEventDTO>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, bool isApproved, int userId)
        {
            try
            {
                var data = await _galleryRepository.UpdateGalleryImageApprovalStatus(galleryId, isApproved, userId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent()
        {
            try
            {
                var response = await _galleryRepository.GetAllGalleryImagesByEvent();

                if (response.Success)
                {
                    foreach (var galleryEvent in response.Data)
                    {
                        for (int i = 0; i < galleryEvent.FileNames.Count; i++)
                        {
                            galleryEvent.FileNames[i] = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery", galleryEvent.FileNames[i]);
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GalleryEventDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}