﻿using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request);
        Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request);
        Task<ServiceResponse<Assignment>> GetAssignmentById(int id);
        Task<ServiceResponse<bool>> DeleteAssignment(int id);
        Task<ServiceResponse<byte[]>> DownloadDocument(int documentId);
        Task<ServiceResponse<byte[]>> GetAssignmentsExport(GetAssignmentsExportRequest request);
        Task<ServiceResponse<List<GetTypeWiseResponse>>> GetTypeWise();

    }
}
