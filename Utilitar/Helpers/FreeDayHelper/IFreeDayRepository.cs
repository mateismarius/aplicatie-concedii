using Utilitar.Models;

namespace Utilitar.Helpers.FreeDayHelper
{
    public interface IFreeDayRepository
    {
        Task<int> GetDaysOff(int id, string description);
        Task<string> GetDayDescription(int id, int day, int month, int year);
        Task<List<FreeDay>> CheckIfDayExists(DateTime dateOnly, int employeeId);
    }
}
