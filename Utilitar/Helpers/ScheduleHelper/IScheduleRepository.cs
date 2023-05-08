using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Helpers.ScheduleHelper
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<FreeDay>> GetDaysOff(int id);
        Task<bool> IsLegallDayOff(DateTime dateToCheck);
        Task SaveExcelFile(FileInfo file, string sheetName, List<Employee> emp, ExportVM export);

    }
}
