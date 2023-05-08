using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Utilitar.AppContext;
using Utilitar.Models;

namespace Utilitar.CustomValidation
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        private readonly UtilitiesContext _context;
        public EmployeeValidator(UtilitiesContext context)
        {
            _context = context;
            RuleFor(x => x.FirstName).NotNull().WithName("Prenume").WithMessage("* Acest camp nu poate fi gol!");
            RuleFor(x => x.LastName).NotEmpty().WithName("Nume").WithMessage("* Acest camp nu poate fi gol!");
            RuleFor(x => x.DataFinal).GreaterThan(x => x.DataStart).WithMessage("* Data incetarii nu poate fi mai mica decat data inceperii");
        }

        protected bool EmployeeExists(Employee emp)
        {
            var user = _context.Employees.Where(x => x.FirstName == emp.FirstName && x.LastName == emp.LastName).ToList();
            if (user == null || user.Count <= 0) 
            {
                return true;
            }
            return false;
        }
    }
}
