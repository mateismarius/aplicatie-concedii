using Microsoft.AspNetCore.Mvc;
using Utilitar.AppContext;

namespace Utilitar.Controllers
{
    public class DaysOfByEmployeeController : Controller
    {
        private readonly UtilitiesContext _context;

        public DaysOfByEmployeeController(UtilitiesContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DaysOffList(int id)
        {
            if (_context.FreeDays == null)
            {
                return NotFound();
            }

            var freeDays = await _context.FreeDays.FindAsync(id);

            if (freeDays == null)
            {
                return NotFound();
            }
            return View(freeDays);
        }
    }
    
}
