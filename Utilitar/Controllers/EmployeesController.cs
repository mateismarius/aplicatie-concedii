using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Utilitar.AppContext;
using Utilitar.Areas.Identity.Data;
using Utilitar.Helpers.FreeDayHelper;
using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly UtilitiesContext _context;
        private readonly IFreeDayRepository _freeDayRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<Employee> _validator;
        public EmployeesController(UtilitiesContext context, IMapper mapper,
            IFreeDayRepository freeDayRepository, UserManager<ApplicationUser> userManager, IValidator<Employee> validator)
        {
            _context = context;
            _freeDayRepository = freeDayRepository;
            _mapper = mapper;
            _userManager = userManager;
            _validator = validator;
        }
        
        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employee = await _context.Employees.Include(i => i.Roles).OrderBy(o => o.Roles.PriorityNo).ToListAsync();
            var mappedEmployee = _mapper.Map<IEnumerable<EmployeeVM>>(employee);

            var user = await _userManager.GetUserAsync(User);
            if(user != null) 
                return RedirectToAction("GetEmployeesByLocation",new {location = user.Location});
           return View(mappedEmployee);
        }
        
        public async Task<IActionResult> GetEmployeesByLocation( string location)
        {
            var result = await _context.Employees.Include(i => i.Roles)
                                                .OrderBy(o => o.LastName)
                                                .Where(s => s.Location == location).ToListAsync();
            var mappedEmployee = _mapper.Map<IEnumerable<EmployeeVM>>(result);
            foreach (var emp in mappedEmployee)
            {
                var countDays = await _freeDayRepository.GetDaysOff(emp.Id, "CO");
                var countCSDays = await _freeDayRepository.GetDaysOff(emp.Id, "CS");
                emp.Efectuat = countDays;
                emp.DaysLeft = (emp.DrepturiCurente + emp.DrepturiRestante) - countDays;
                emp.CSEfectuat = countCSDays;
            }
            return _context.Employees != null ?
                          View("Selected", mappedEmployee) :
                          Problem("Nu exista nici o inregistrare");
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["RoleId"] = new SelectList(await _context.Roles.ToListAsync(), "Id", "RoleName");
            ViewData["Location"] = user.Location;
                
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Role,Location,PermanentLocation,IsDelegate,Marca,DataStart,DataFinal,Id, DrepturiCurente, DrepturiRestante, RoleId, FullName")] Employee employee)
        {
            ValidationResult result = await _validator.ValidateAsync(employee);
            var emp = _context.Employees.Where(x => x.FirstName == employee.FirstName && x.LastName == employee.LastName).FirstOrDefault();
            if (emp != null)
            {
                ModelState.AddModelError("", "Persoana este deja inregistrata");
            }
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                ViewData["RoleId"] = new SelectList(await _context.Roles.ToListAsync(), "Id", "RoleName");
                var user = await _userManager.GetUserAsync(User);
                ViewData["Location"] = user.Location;
                return View(nameof(Create), employee);
            }
            _context.Add(employee);
            await _context.SaveChangesAsync();
            TempData["notice"] = "Persoana a fost adaugata cu succes!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Include(i => i.Roles).Where(i => i.Id == id).FirstOrDefaultAsync();
            ViewData["RoleId"] = new SelectList(await _context.Roles.ToListAsync(), "Id", "RoleName");
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Role,Location,PermanentLocation,IsDelegate,Marca,DataStart,DataFinal,Id, DrepturiCurente, DrepturiRestante, RoleId")] Employee employee)
        {
            ValidationResult result = await _validator.ValidateAsync(employee);
            if (id != employee.Id)
            {
                return NotFound();
            }
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return View(employee);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'UtilitiesContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<PartialViewResult> GetSearchRecord(string searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);
            List<Employee> result = new();
            if (searchTerm == null)
            {
                result = await _context.Employees.Include(i => i.Roles)
                                                .OrderBy(o => o.LastName)
                                                .Where(s => s.Location == user.Location).ToListAsync();
            }
            else
            {
                result = await _context.Employees.Include(i => i.Roles)
                                                .OrderBy(o => o.LastName)
                                                .Where(e => e.Location == user.Location && (e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm)))
                                                .ToListAsync();
            }
            var mappedEmployee = _mapper.Map<IEnumerable<EmployeeVM>>(result);
            foreach (var emp in mappedEmployee)
            {
                var countDays = await _freeDayRepository.GetDaysOff(emp.Id, "CO");
                var countCSDays = await _freeDayRepository.GetDaysOff(emp.Id, "CS");
                emp.Efectuat = countDays;
                emp.DaysLeft = (emp.DrepturiCurente + emp.DrepturiRestante) - countDays;
                emp.CSEfectuat = countCSDays;
            }

            return PartialView("_SearchPartial", mappedEmployee);
        }
    }
}
