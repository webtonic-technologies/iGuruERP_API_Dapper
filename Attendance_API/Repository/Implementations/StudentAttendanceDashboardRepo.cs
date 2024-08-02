using Attendance_API.Repository.Interfaces;
using System.Data;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceDashboardRepo : IStudentAttendanceDashboardRepo
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceDashboardRepo(IDbConnection connection)
        {
            _connection = connection;
        }
    }
}
