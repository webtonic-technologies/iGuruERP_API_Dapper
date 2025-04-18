﻿using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface IHomeworkService
    {
        Task<ServiceResponse<string>> AddUpdateHomework(HomeworkRequest request);
        Task<ServiceResponse<List<GetAllHomeworkResponse>>> GetAllHomework(GetAllHomeworkRequest request);
        Task<ServiceResponse<Homework>> GetHomeworkById(int id);
        Task<ServiceResponse<bool>> DeleteHomework(int id);
        Task<ServiceResponse<GetHomeworkHistoryResponse>> GetHomeworkHistory(GetHomeworkHistoryRequest request);
        Task<ServiceResponse<byte[]>> GetAllHomeworkExport(GetAllHomeworkExportRequest request);

    }
}
