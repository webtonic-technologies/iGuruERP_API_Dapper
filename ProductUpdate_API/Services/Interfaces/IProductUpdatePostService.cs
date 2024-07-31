﻿using ProductUpdates_API.DTOs.Requests;
using ProductUpdates_API.DTOs.Response;
using ProductUpdates_API.DTOs.ServiceResponse;

namespace ProductUpdates_API.Services.Interfaces
{
    public interface IProductUpdatePostService
    {
        Task<ServiceResponse<int>> CreatePost(CreatePostRequest request);
        Task<ServiceResponse<List<PostResponse>>> GetAllPosts(int moduleId);
    }
}
