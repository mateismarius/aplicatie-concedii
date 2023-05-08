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
    public class DayOffTypesController : Controller
    {
        private readonly UtilitiesContext _context;

        public DayOffTypesController(UtilitiesContext context)
        {
            _context = context;
        }

        // GET: DayOffTypes
        public async Task<IActionResult> Index()
        {
              return _context.DayOffTypes != null ? 
                          View(await _context.DayOffTypes.ToListAsync()) :
                          Problem("Entity set 'UtilitiesContext.DayOffTypes'  is null.");
        }

        // GET: DayOffTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DayOffTypes == null)
            {
                return NotFound();
            }

            var dayOffType = await _context.DayOffTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dayOffType == null)
            {
                return NotFound();
            }

            return View(dayOffType);
        }

        // GET: DayOffTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsPaid,Id")] DayOffType dayOffType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dayOffType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dayOffType);
        }

        // GET: DayOffTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DayOffTypes == null)
            {
                return NotFound();
            }

            var dayOffType = await _context.DayOffTypes.FindAsync(id);
            if (dayOffType == null)
            {
                return NotFound();
            }
            return View(dayOffType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,IsPaid,Id")] DayOffType dayOffType)
        {
            if (id != dayOffType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dayOffType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DayOffTypeExists(dayOffType.Id))
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
            return View(dayOffType);
        }

        // GET: DayOffTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DayOffTypes == null)
            {
                return NotFound();
            }

            var dayOffType = await _context.DayOffTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dayOffType == null)
            {
                return NotFound();
            }

            return View(dayOffType);
        }

        // POST: DayOffTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DayOffTypes == null)
            {
                return Problem("Entity set 'UtilitiesContext.DayOffTypes'  is null.");
            }
            var dayOffType = await _context.DayOffTypes.FindAsync(id);
            if (dayOffType != null)
            {
                _context.DayOffTypes.Remove(dayOffType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DayOffTypeExists(int id)
        {
          return (_context.DayOffTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
