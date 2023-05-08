using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using Utilitar.AppContext;
using Utilitar.Areas.Identity.Data;
using Utilitar.Helpers.ScheduleHelper;
using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _repository;
        private readonly UtilitiesContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ScheduleController(IScheduleRepository repository, UtilitiesContext context, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
                        var user = await _userManager.GetUserAsync(User);
            var gref = _context.Employees != null ? await _context.Employees
                .Include(e => e.Roles)
                .Where(e => e.Location == user.Location && e.Roles.RoleName.Contains("Grefier"))
                .ToListAsync() : null;
            var proc = _context.Employees != null ? await _context.Employees
                .Include(e => e.Roles)
                .Where(e => e.Location == user.Location && e.Roles.RoleName.Contains("Procuror"))
                .ToListAsync() : null;
            
            if (gref != null || proc != null)
            {
                foreach (var employee in gref)
                    {
                        employee.FullName = $"{employee.LastName} {employee.FirstName}";
                    }
                foreach (var employee in proc)
                {
                    employee.FullName = $"{employee.LastName} {employee.FirstName}";
                }

                ViewData["GrefierId"] = new SelectList(gref.AsEnumerable(), "Id", "FullName");
                ViewData["ProcurorId"] = new SelectList(proc.AsEnumerable(), "FullName", "FullName");
            }
            ViewData["Location"] = user.Location;
            ViewData["User"] = $"{user.LastName} {user.FirstName}";

            return View();
        }

        public async Task<FileResult> SaveExcel([Bind("DateMonth, DateYear, Location, RegNo, MadeBy, SignedBy")] ExportVM export)
        {
            var directoryPath = @"C:\TempDocuments\";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            var filePath = @"C:\TempDocuments\Pontaje.xlsx";

            if (ModelState.IsValid)
            {
                var lastDay = DateTime.DaysInMonth(export.DateYear, export.DateMonth);
                var selectedDateFinal = new DateTime(export.DateYear, export.DateMonth, lastDay);
                var selectedDateStart = new DateTime(export.DateYear, export.DateMonth, 1);

                var file = new FileInfo(filePath);
                var emp = await _context.Employees
                                .Include(i => i.Roles)
                                .Where(i => (i.DataStart <= selectedDateFinal) 
                                            && (i.DataFinal >= selectedDateStart)).ToListAsync();

                await _repository.SaveExcelFile(file, export.Location, emp, export);
            }
            byte[] filebytes = System.IO.File.ReadAllBytes(filePath);
            string filename = $"Pontaje_{export.DateMonth}_{export.DateYear}.xlsx";
            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        private static string GetMonthName(int i)
        {
            List<string> list = new()
            {
                "Ianuarie",
                "Februarie",
                "Martie",
                "Aprilie",
                "Mai",
                "Iunie",
                "Iulie",
                "August",
                "Septembrie",
                "Octombrie",
                "Noiembrie",
                "Decembrie"
            };

            return list[i - 1];
        }

    }
}
