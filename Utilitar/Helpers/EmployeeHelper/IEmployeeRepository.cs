using Utilitar.Models;

namespace Utilitar.Helpers.EmployeeHelper
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesList();
    }
}