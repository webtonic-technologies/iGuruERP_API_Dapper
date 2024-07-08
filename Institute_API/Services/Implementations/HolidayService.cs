using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO)
        {
            try
            {
                return await _holidayRepository.AddUpdateHoliday(holidayDTO);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId)
        {
            try
            {
                return await _holidayRepository.GetHolidayById(holidayId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HolidayDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(int Institute_id)
        {
            try
            {
                return await _holidayRepository.GetAllHolidays(Institute_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(int Institute_id)
        {
            try
            {
                return await _holidayRepository.GetApprovedHolidays(Institute_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHoliday(int holidayId)
        {
            try
            {
                return await _holidayRepository.DeleteHoliday(holidayId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy)
        {
            try
            {
                return await _holidayRepository.UpdateHolidayApprovalStatus(holidayId, isApproved, approvedBy);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
