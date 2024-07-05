using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Repository.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<ServiceResponse<string>> AddUpdateAppointment(Appointment appointment);
        Task<ServiceResponse<IEnumerable<Appointment>>> GetAllAppointments(GetAllAppointmentsRequest request);
        Task<ServiceResponse<Appointment>> GetAppointmentById(int appointmentId);
        Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId);
    }
}
