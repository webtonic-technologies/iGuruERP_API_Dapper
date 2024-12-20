﻿using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IRoutePlanService
    {
        Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlanRequestDTO routePlan);
        Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request);
        Task<ServiceResponse<GetAllRoutePlanExportResponse>> ExportRoutePlansData(GetAllRoutePlanExportRequest request);

        Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int routePlanId);
        Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int routePlanId);
        Task<ServiceResponse<RouteDetailsResponseDTO>> GetRouteDetails(GetRouteDetailsRequest request);
        Task<ServiceResponse<byte[]>> GetRouteDetailsExportExcel(GetRouteDetailsRequest request);
        Task<ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>> GetRoutePlanVehicles(int instituteID);

    }
}
