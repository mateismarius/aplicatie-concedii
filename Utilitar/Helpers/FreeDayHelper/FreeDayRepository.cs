using Microsoft.EntityFrameworkCore;
using Utilitar.AppContext;
using Utilitar.Models;

namespace Utilitar.Helpers.FreeDayHelper
{
    public class FreeDayRepository : IFreeDayRepository
    {
        private readonly UtilitiesContext _utilitiesContext;

        public FreeDayRepository(UtilitiesContext utilitiesContext)
        {
            _utilitiesContext = utilitiesContext;
        }


        // Această metodă returnează numărul de zile de odihnă pe care un angajat le-a luat într-un an 
        // pentru un tip specific de zi liberă
        public async Task<int> GetDaysOff(int id, string description)
        {
            // Se obține anul curent
            int year = DateTime.Now.Year;
            // Variabilă pentru numărul de zile de odihnă
            int counter = 0;
            // Se preiau toate zilele libere de tipul specificat pentru angajatul și anul specificate
            var freeDays = await _utilitiesContext.FreeDays
                .Where(d => d.EmployeeId == id &&
                            d.DayOffType.Description == description &&
                            ((d.StartDate.Year == year && d.FinishDate.Year == year)))
                .ToListAsync();
            List<FreeDay> days = freeDays;

            // Se parcurg zilele libere și se adaugă durata fiecăreia la contor
            foreach (var day in freeDays)
            {
                counter += day.Duration;
            }

            // Se returnează numărul total de zile de odihnă
            return counter;
        }

        // Această metodă returnează tipul de zi liberă a unui angajat într-o anumită dată
        public async Task<string> GetDayDescription(int id, int day, int month, int year)
        {
            // Se inițializează variabila pentru rezultat
            string result = String.Empty;
            // Se preiau toate zilele libere pentru angajatul specificat
            var temp = await _utilitiesContext.FreeDays
                .Where(d => d.EmployeeId == id)
                .Include(f => f.DayOffType)
                .ToListAsync();
            // Se parcurg zilele libere și se verifică dacă data specificată se află în intervalul lor
            foreach (var tempDay in temp)
            {
                if (tempDay.StartDate.Month == tempDay.FinishDate.Month)
                {
                    if (day >= tempDay.StartDate.Day && day <= tempDay.FinishDate.Day)
                        result = tempDay.DayOffType.Description;
                }
                else if (tempDay.StartDate.Month < month)
                {
                    if (day >= 1 && day <= tempDay.FinishDate.Day)
                        result = tempDay.DayOffType.Description;
                }
                else
                {
                    if (day >= tempDay.StartDate.Day && day <= DateTime.DaysInMonth(year, month))
                        result = tempDay.DayOffType.Description;
                }
            }

            // Se returnează descrierea tipului de zi liberă sau un șir vid dacă nu s-a găsit niciunul
            return result;
        }

        // Această metodă verifică dacă există o zi liberă pentru un angajat la o anumită dată
        public async Task<List<FreeDay>> CheckIfDayExists(DateTime dateOnly, int employeeId)
        {
            // Se preiau toate zilele libere pentru angajatul specificat
            var daysList = await _utilitiesContext.FreeDays
                .Where(w => w.EmployeeId == employeeId)
                .ToListAsync();
            return daysList;

        }
    }
}
