using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Utilitar.AppContext;
using Utilitar.Models;

namespace Utilitar.Controllers
{
    [Authorize]
    public class LegallDaysController : Controller
    {
        private readonly UtilitiesContext _context;

        public LegallDaysController(UtilitiesContext context)
        {
            _context = context;
        }

        // GET: LegallDays
        public async Task<IActionResult> Index()
        {
              return _context.LegallDay != null ? 
                          View(await _context.LegallDay.ToListAsync()) :
                          Problem("Entity set 'UtilitiesContext.LegallDay'  is null.");
        }

        // GET: LegallDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LegallDay == null)
            {
                return NotFound();
            }

            var legallDay = await _context.LegallDay
                .FirstOrDefaultAsync(m => m.Id == id);
            if (legallDay == null)
            {
                return NotFound();
            }

            return View(legallDay);
        }

        // GET: LegallDays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LegallDays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DateOff,Id")] LegallDay legallDay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(legallDay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(legallDay);
        }

        // GET: LegallDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LegallDay == null)
            {
                return NotFound();
            }

            var legallDay = await _context.LegallDay.FindAsync(id);
            if (legallDay == null)
            {
                return NotFound();
            }
            return View(legallDay);
        }

        // POST: LegallDays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,DateOff,Id")] LegallDay legallDay)
        {
            if (id != legallDay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(legallDay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LegallDayExists(legallDay.Id))
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
            return View(legallDay);
        }

        // GET: LegallDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LegallDay == null)
            {
                return NotFound();
            }

            var legallDay = await _context.LegallDay
                .FirstOrDefaultAsync(m => m.Id == id);
            if (legallDay == null)
            {
                return NotFound();
            }

            return View(legallDay);
        }

        // POST: LegallDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LegallDay == null)
            {
                return Problem("Entity set 'UtilitiesContext.LegallDay'  is null.");
            }
            var legallDay = await _context.LegallDay.FindAsync(id);
            if (legallDay != null)
            {
                _context.LegallDay.Remove(legallDay);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LegallDayExists(int id)
        {
          return (_context.LegallDay?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
