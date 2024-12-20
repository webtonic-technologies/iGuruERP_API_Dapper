using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<string>> AddUpdateAppointment(Appointment appointment);
        Task<ServiceResponse<IEnumerable<AppointmentResponse>>> GetAllAppointments(GetAllAppointmentsRequest request);
        Task<ServiceResponse<AppointmentResponse>> GetAppointmentById(int appointmentId);
        Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId);
        Task<ServiceResponse<byte[]>> GetAppointmentsExport(GetAppointmentsExportRequest request);

    }
}
