﻿using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class RouteMappingService : IRouteMappingService
    {
        private readonly IRouteMappingRepository _routeMappingRepository;

        public RouteMappingService(IRouteMappingRepository routeMappingRepository)
        {
            _routeMappingRepository = routeMappingRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            return await _routeMappingRepository.AddUpdateEmployeeStopMapping(request);
        }
        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            // Map RouteMapping to RouteMappingRequest
            var routeMappingRequest = new RouteMappingRequest
            {
                AssignRouteID = routeMapping.AssignRouteID,
                RoutePlanID = routeMapping.RoutePlanID,
                VehicleID = routeMapping.VehicleID,
                DriverID = routeMapping.DriverID,
                TransportStaffID = routeMapping.TransportStaffID,
                IsActive = routeMapping.IsActive,
                StopID = routeMapping.StopID, // Map StopID
                StudentIDs = routeMapping.StudentIDs, // Map StudentIDs
                EmployeeIDs = routeMapping.EmployeeIDs // Map EmployeeIDs
            };

            // Call the repository method with the mapped object
            return await _routeMappingRepository.AddUpdateRouteMapping(routeMappingRequest);
        }


        public async Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request)
        {
            return await _routeMappingRepository.AddUpdateStudentStopMapping(request);
        }
        public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            return await _routeMappingRepository.GetAllRouteMappings(request);
        }
        public async Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId)
        {
            return await _routeMappingRepository.GetEmployeeStopMappings(RoutePlanId);
        }
        public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int routeMappingId)
        {
            return await _routeMappingRepository.GetRouteMappingById(routeMappingId);
        }
        public async Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId)
        {
            return await _routeMappingRepository.GetStudentStopMappings(RoutePlanId);
        }
        public async Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            return await _routeMappingRepository.RemoveEmployeeStopMapping(request);
        }
        public async Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request)
        {
            return await _routeMappingRepository.RemoveStudentStopMapping(request);
        }
        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId)
        {
            return await _routeMappingRepository.UpdateRouteMappingStatus(routeMappingId);
        }

        public async Task<ServiceResponse<RouteVehicleDriverInfoResponse>> GetRouteVehicleDriverInfo(RouteVehicleDriverInfoRequest request)
        {
            var routeInfo = await _routeMappingRepository.GetRouteVehicleDriverInfo(request.RoutePlanID);

            if (routeInfo == null)
            {
                return new ServiceResponse<RouteVehicleDriverInfoResponse>(false, "No data found", null, 404);
            }

            return new ServiceResponse<RouteVehicleDriverInfoResponse>(true, "Data found", routeInfo, 200);
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentsForRouteMappingResponse>>> GetStudentsForRouteMapping(GetStudentsForRouteMappingRequest request)
        {
            var students = await _routeMappingRepository.GetStudentsForRouteMapping(request.ClassID, request.SectionID, request.InstituteID, request.Search);

            if (students == null || !students.Any())
            {
                return new ServiceResponse<IEnumerable<GetStudentsForRouteMappingResponse>>(false, "No students found", null, 404);
            }

            return new ServiceResponse<IEnumerable<GetStudentsForRouteMappingResponse>>(true, "Students found", students, 200);
        }

        public async Task<ServiceResponse<IEnumerable<GetEmployeesForRouteMappingResponse>>> GetEmployeesForRouteMapping(GetEmployeesForRouteMappingRequest request)
        {
            var employees = await _routeMappingRepository.GetEmployeesForRouteMapping(request.DepartmentID, request.DesignationID, request.InstituteID, request.Search);

            if (employees == null || !employees.Any())
            {
                return new ServiceResponse<IEnumerable<GetEmployeesForRouteMappingResponse>>(false, "No employees found", null, 404);
            }

            return new ServiceResponse<IEnumerable<GetEmployeesForRouteMappingResponse>>(true, "Employees found", employees, 200);
        }


        public async Task<ServiceResponse<IEnumerable<GetRouteListResponse>>> GetRouteList(int instituteID)
        {
            return await _routeMappingRepository.GetRouteList(instituteID);
        }

        public async Task<ServiceResponse<GetAllRouteAssignedInfoResponse>> GetAllRouteAssignedInfo(GetAllRouteAssignedInfoRequest request)
        {
            try
            {
                // Query to fetch route and stop details along with student and employee counts
                var routeDetails = await _routeMappingRepository.GetRouteDetailsWithStopInfo(request);

                if (routeDetails != null)
                {
                    // Calculate Total Student Count and Total Employee Count across all stops
                    var totalStudentCount = routeDetails.Stops.Sum(stop => stop.StudentCount);
                    var totalEmployeeCount = routeDetails.Stops.Sum(stop => stop.EmployeeCount);

                    // Return the response with the total counts and route details
                    var response = new GetAllRouteAssignedInfoResponse
                    {
                        TotalStudentCount = totalStudentCount,
                        TotalEmployeeCount = totalEmployeeCount,
                        RouteDetails = routeDetails
                    };

                    return new ServiceResponse<GetAllRouteAssignedInfoResponse>(true, "Route Assigned Info Retrieved Successfully", response, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<GetAllRouteAssignedInfoResponse>(false, "No records found", null, StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetAllRouteAssignedInfoResponse>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetTransportStaffResponse>>> GetTransportStaff(GetTransportStaffRequest request)
        {
            // Pass the InstituteID from the request object
            var transportStaff = await _routeMappingRepository.GetTransportStaff(request.InstituteID);

            if (transportStaff == null || !transportStaff.Any())
            {
                return new ServiceResponse<IEnumerable<GetTransportStaffResponse>>(false, "No transport staff found", new List<GetTransportStaffResponse>(), StatusCodes.Status404NotFound);
            }

            return new ServiceResponse<IEnumerable<GetTransportStaffResponse>>(true, "Transport staff found", transportStaff, StatusCodes.Status200OK);
        }


    }
}
