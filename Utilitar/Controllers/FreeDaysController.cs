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
    public class FreeDaysController : Controller
    {
        private readonly UtilitiesContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFreeDayRepository _freedayRepository;
        private readonly IValidator<FreeDay> _validator;

        public FreeDaysController(UtilitiesContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager, IFreeDayRepository freeDayRepository, IValidator<FreeDay> validator)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _freedayRepository = freeDayRepository;
            _validator = validator;
        }



        // GET: FreeDays
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                return RedirectToAction("GetFreeDayByLocation", new { location = user.Location });
            return View();
        }

        public async Task<IActionResult> GetFreeDayByLocation(string location)
        {
            var result = await _context.FreeDays.Include(f => f.DayOffType)
                                                .Include(i => i.Employee)
                                                .Where(w => w.Employee.Location == location)
                                                .OrderByDescending(o => o.StartDate)
                                                .ToListAsync();
            var mappedDays = _mapper.Map<IEnumerable<FreeDayVM>>(result).AsEnumerable();
            return _context.FreeDays != null ?
                          View("Selected", mappedDays) :
                          Problem("Nu exista nici o inregistrare");
        }

        [HttpGet, ActionName("DaysOffList")]
        public async Task<IActionResult> DaysOffList(int id)
        {
            if (_context.FreeDays == null)
            {
                return NotFound();
            }

            var freeDays = await _context.FreeDays.Where(f => f.EmployeeId == id)
                            .Include(f => f.DayOffType).Include(f => f.Employee).OrderByDescending(o => o.StartDate).ToListAsync();
            var result = _mapper.Map<IEnumerable<FreeDayVM>>(freeDays);
            var user = await _userManager.GetUserAsync(User);
            if (result == null)
            {
                return NotFound();
            }
            return View("DaysOffList", result);
        }

        // GET: FreeDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FreeDays == null)
            {
                return NotFound();
            }

            var freeDay = await _context.FreeDays
                .Include(f => f.DayOffType)
                .Include(f => f.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freeDay == null)
            {
                return NotFound();
            }

            return View(freeDay);
        }

        // GET: FreeDays/Create
        public async Task<IActionResult> Create()
        {
            var fdList = _context.FreeDays != null ? await _context.FreeDays.ToListAsync() : null;
            var empList = _context.Employees != null ? await _context.Employees.ToListAsync() : null;

            if (empList != null)
                ViewData["empList"] = empList;
            if (fdList != null)
                ViewData["fdaysList"] = fdList.AsEnumerable();

            var user = await _userManager.GetUserAsync(User);
            
            if (user != null)
                return RedirectToAction("CreateByLocation", new { location = user.Location });
            return View();
        }

        public async Task<IActionResult> CreateByLocation(string location)
        {
            var empList = await _context.Employees
                            .Where(s => s.Location == location)
                            .OrderBy(s => s.LastName)
                            .ToListAsync();
            foreach (var employee in empList)
            {
                employee.FullName = $"{employee.LastName} {employee.FirstName}";
            }  
            ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes, "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(empList.AsEnumerable(), "Id", "FullName"); 
            return View(nameof(Create));
        }

        // POST: FreeDays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestNumber,RequestDate,StartDate,FinishDate,Duration,EmployeeId,DayOffTypeId,Id")] FreeDay freeDay)
        {
            ValidationResult result = await _validator.ValidateAsync(freeDay);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                var user = await _userManager.GetUserAsync(User);
                var empList = await _context.Employees
                            .Where(s => s.Location == user.Location)
                            .ToListAsync();
                foreach (var employee in empList)
                {
                    employee.FullName = $"{employee.LastName} {employee.FirstName}";
                }
                ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes, "Id", "Name");
                ViewData["EmployeeId"] = new SelectList(empList.AsEnumerable(), "Id", "FullName");
                return View(freeDay);
            }
            if (ModelState.IsValid)
            {
                _context.Add(freeDay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes.OrderBy(o => o.Name), "Id", "Name", freeDay.DayOffTypeId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.OrderBy(o => o.LastName), "Id", "LastName", freeDay.EmployeeId);
            return View(freeDay);
        }

        // GET: FreeDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FreeDays == null)
            {
                return NotFound();
            }

            var freeDay = await _context.FreeDays.FindAsync(id);
            if (freeDay == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var emp = _context.Employees != null ? await _context.Employees.Where(e => e.Location == user.Location).ToListAsync() : null;
            if (emp != null)
            {
                foreach (var employee in emp)
                {
                    employee.FullName = $"{employee.LastName} {employee.FirstName}";
                }
            }
            ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes, "Id", "Name", freeDay.DayOffTypeId);
            ViewData["EmployeeId"] = new SelectList(emp.AsEnumerable(), "Id", "FullName", freeDay.EmployeeId);
            return View(freeDay);
        }

        // POST: FreeDays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestNumber,RequestDate,StartDate,FinishDate,Duration,EmployeeId,DayOffTypeId,Id")] FreeDay freeDay)
        {
            ValidationResult result = await _validator.ValidateAsync(freeDay);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                var user = await _userManager.GetUserAsync(User);
                var emp = _context.Employees != null ? await _context.Employees.Where(e => e.Location == user.Location).OrderBy(o => o.LastName).ToListAsync() : null;
                if (emp != null)
                {
                    foreach (var employee in emp)
                    {
                        employee.FullName = $"{employee.LastName} {employee.FirstName}";
                    }
                }
                ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes, "Id", "Name", freeDay.DayOffTypeId);
                ViewData["EmployeeId"] = new SelectList(emp.AsEnumerable(), "Id", "FullName", freeDay.EmployeeId);
                return View(freeDay);
            }
            if (id != freeDay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freeDay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreeDayExists(freeDay.Id))
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

            ViewData["DayOffTypeId"] = new SelectList(_context.DayOffTypes, "Id", "Name", freeDay.DayOffTypeId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", freeDay.EmployeeId);
            return View(freeDay);
        }

        // GET: FreeDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FreeDays == null)
            {
                return NotFound();
            }

            var freeDay = await _context.FreeDays
                .Include(f => f.DayOffType)
                .Include(f => f.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freeDay == null)
            {
                return NotFound();
            }

            return View(freeDay);
        }

        // POST: FreeDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FreeDays == null)
            {
                return Problem("Entity set 'UtilitiesContext.FreeDays'  is null.");
            }
            var freeDay = await _context.FreeDays.FindAsync(id);
            if (freeDay != null)
            {
                _context.FreeDays.Remove(freeDay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreeDayExists(int id)
        {
            return (_context.FreeDays?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpGet]

        public async Task<JsonResult> CheckIfHolidayLeft( int requestDuration, int EmployeeId)
        {
            int taken = 0;
            var employee = await _context.Employees.Where(e => e.Id == EmployeeId).FirstOrDefaultAsync();

             if (employee != null)
            {
                taken = await _freedayRepository.GetDaysOff(employee.Id, "CO");
            }
             var result = ((employee.DrepturiCurente + employee.DrepturiRestante) - taken) > requestDuration;

            return Json(result);
        }

        public async Task<PartialViewResult> GetSearchDay(string searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);
            List<FreeDay> result = new();
            if (searchTerm == null)
            {
                result =  await _context.FreeDays.Include(i => i.Employee)
                                                .Include(i => i.DayOffType)
                                                .OrderBy(o => o.Employee.LastName)
                                                .Where(s => s.Employee.Location == user.Location).ToListAsync();
            }
            else
            {
                result = await _context.FreeDays.Include(i => i.Employee)
                                                .Include(i => i.DayOffType)
                                                .OrderBy(o => o.Employee.LastName)
                                                .Where(e => e.Employee.Location == user.Location && (e.Employee.FirstName.Contains(searchTerm) || e.Employee.LastName.Contains(searchTerm)))
                                                .ToListAsync();
            }
            var mappedDays = _mapper.Map<IEnumerable<FreeDayVM>>(result).AsEnumerable();
            

            return PartialView("_SearchDayPartial", mappedDays);
        }
    }
}
