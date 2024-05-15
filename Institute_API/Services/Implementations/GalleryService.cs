using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Institute_API.Repository.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Immutable;

namespace Institute_API.Services.Implementations
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IImageService _imageService;

        public GalleryService(IGalleryRepository galleryRepository, IImageService imageService)
        {
            _galleryRepository = galleryRepository;
            _imageService = imageService;   
        }

        public async Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO)
        {
            try
            {
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery");

                //if (!Directory.Exists(uploadsFolder))
                //{
                //    Directory.CreateDirectory(uploadsFolder);
                //}

                //string uniqueFileName = Guid.NewGuid().ToString() + "_" + galleryDTO.File.FileName;
                //string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await galleryDTO.File.CopyToAsync(fileStream);
                //}
                if (galleryDTO.Base64File != null && galleryDTO.Base64File != "")
                {
                    var file = await _imageService.SaveImageAsync(galleryDTO.Base64File, "Gallery");
                    galleryDTO.FileName = file.relativePath;
                }

                //galleryDTO.FileName = uniqueFileName;
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
                        foreach(var item in galleryEvent.FileNames)
                        {
                            galleryEvent.Base64Files.Add(_imageService.GetImageAsBase64(item));
                            //for (int i = 0; i < galleryEvent.FileNames.Count; i++)
                            //{
                            //    galleryEvent.FileNames[i] = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery", galleryEvent.FileNames[i]);
                            //}
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