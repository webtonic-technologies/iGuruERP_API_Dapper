﻿using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Interfaces
{
    public interface IGalleryService
    {
        Task<ServiceResponse<int>> UploadGalleryImage(int eventID, GalleryImageRequest request);
        Task<ServiceResponse<GalleryImageResponse>> DownloadGalleryImage(int galleryID);
        Task<ServiceResponse<List<GalleryImageResponse>>> DownloadAllGalleryImages(int eventID);
        Task<ServiceResponse<bool>> DeleteGalleryImage(int galleryID);
        Task<ServiceResponse<List<GalleryImageResponse>>> GetAllGalleryImages(GalleryImageRequest request);
    }
}
