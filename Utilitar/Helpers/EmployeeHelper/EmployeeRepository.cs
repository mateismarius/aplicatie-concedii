using Microsoft.EntityFrameworkCore;
using Utilitar.AppContext;
using Utilitar.Models;

namespace Utilitar.Helpers.EmployeeHelper
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly UtilitiesContext _utilitiesContext;

        public EmployeeRepository(UtilitiesContext utilitiesContext)
        {
            _utilitiesContext = utilitiesContext;
        }
        // Această metodă returnează o lista a angajatilor
        public async Task<IEnumerable<Employee>> GetEmployeesList()
        {
            var employees = await _utilitiesContext.Employees.ToListAsync();
            return  employees;
        }
    }
}
