﻿using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Implementations;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentQRService : IStudentQRService
    {
        private readonly IStudentQRRepository _studentORRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageService _imageService;
        public StudentQRService(IImageService imageService, IStudentQRRepository studentORRepository, IWebHostEnvironment webHostEnvironment)
        {
            _studentORRepository = studentORRepository;
            _hostingEnvironment = webHostEnvironment;
            _imageService = imageService;
        }

        public async Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int section_id, int class_id, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var data = await _studentORRepository.GetAllStudentQR(section_id, class_id, pageNumber, pageSize);
                if (data.Success)
                {
                    foreach (var item in data.Data)
                    {
                        item.QR_code = _imageService.GetImageAsBase64(item.QR_code);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentQRDTO>>(false, ex.Message, [], 500);
            }
        }
    }
}
