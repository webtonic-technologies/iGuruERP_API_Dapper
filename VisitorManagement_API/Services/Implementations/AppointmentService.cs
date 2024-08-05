using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAppointment(Appointment appointment)
        {
            return await _appointmentRepository.AddUpdateAppointment(appointment);
        }

        public async Task<ServiceResponse<IEnumerable<AppointmentResponse>>> GetAllAppointments(GetAllAppointmentsRequest request)
        {
            return await _appointmentRepository.GetAllAppointments(request);
        }

        public async Task<ServiceResponse<AppointmentResponse>> GetAppointmentById(int appointmentId)
        {
            return await _appointmentRepository.GetAppointmentById(appointmentId);
        }

        public async Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId)
        {
            return await _appointmentRepository.UpdateAppointmentStatus(appointmentId);
        }
    }
}
