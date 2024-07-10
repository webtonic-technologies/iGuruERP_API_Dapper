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
                List<string> savedFilePaths = new List<string>();
                foreach (var item in galleryDTO.FileName)
                {
                    if (item != null && item != "")
                    {
                        var file = await _imageService.SaveImageAsync(item, "Gallery");
                        savedFilePaths.Add(file.relativePath);
                    }
                }
               galleryDTO.FileName = savedFilePaths;

                //galleryDTO.FileName = uniqueFileName;
                var data = await _galleryRepository.AddGalleryImage(galleryDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent(GetGalleryRequestModel model)
        {
            try
            {
                var response = await _galleryRepository.GetApprovedImagesByEvent(model.Institute_id, model.pageSize, model.pageNumber);
                if (response.Success)
                {
                    foreach (var galleryEvent in response.Data)
                    {
                        foreach (var item in galleryEvent.FileNames)
                        {
                            if (item != null && item != "")
                            {
                                galleryEvent.FileName.Add(_imageService.GetImageAsBase64(item));
                            }

                            //for (int i = 0; i < galleryEvent.FileNames.Count; i++)
                            //{
                            //    galleryEvent.FileNames[i] = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery", galleryEvent.FileNames[i]);
                            //}
                        }

                    }
                }

                //if (response.Success)
                //{
                //    foreach (var galleryEvent in response.Data)
                //    {
                //        for (int i = 0; i < galleryEvent.FileNames.Count; i++)
                //        {
                //            galleryEvent.FileName[i] = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery", galleryEvent.FileNames[i]);
                //        }
                //    }
                //}

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
        public async Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent(GetGalleryRequestModel model)
        {
            try
            {
                var response = await _galleryRepository.GetAllGalleryImagesByEvent(model.Institute_id, model.pageSize, model.pageNumber);

                if (response.Success)
                {
                    foreach (var galleryEvent in response.Data)
                    {
                        foreach(var item in galleryEvent.FileNames)
                        {
                            if(item != null && item != "")
                            {
                                galleryEvent.FileName.Add(_imageService.GetImageAsBase64(item));
                            }
                          
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

        public async Task<ServiceResponse<bool>> DeleteGalleryImage(int Gallery_id)
        {
            try
            {
                var data = await _galleryRepository.DeleteGalleryImage(Gallery_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}