using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseAppPOE.Data;
using EventEaseAppPOE.Models;

namespace EventEaseAppPOE.Controllers
{
    public class EventController : Controller
    {
        private readonly EventeaseDbContext _context;

        public EventController(EventeaseDbContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(Request.Query["search"]))
            {
                var searchTerm = Request.Query["search"].ToString();
                bookings = bookings.Where(b => b.Event.EventName.Contains(searchTerm) || b.Venue.VenueName.Contains(searchTerm));
            }

            return View(await _context.Events.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        //filters through bookings based on event type, date range, and venue availability
        public async Task<IActionResult> Filter(string? selectedEventType, DateTime? startDate, DateTime? endDate, bool? venueAvailable)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(selectedEventType))
                bookingsQuery = bookingsQuery.Where(b => b.Event.EventType.EventTypeName == selectedEventType);

            if (startDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate);

            if (endDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate);

            if (venueAvailable.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.Venue.IsAvailable == venueAvailable.Value);

            var model = new BookingFilterViewModel
            {
                SelectedEventType = selectedEventType,
                StartDate = startDate,
                EndDate = endDate,
                VenueAvailable = venueAvailable,
                EventTypes = await _context.EventTypes.Select(et => et.EventTypeName).ToListAsync(),
                FilteredBookings = await bookingsQuery.ToListAsync()
            };

            return View(model);
        }



        // GET: Event/Create
        public IActionResult Create()
        {
            ViewBag.EventTypeId = new SelectList(_context.EventTypes.ToList(), "EventTypeId", "EventTypeName");
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,EventDate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populates dropdown list if form submission fails
            ViewBag.EventTypeId = new SelectList(_context.EventTypes.ToList(), "EventTypeId", "EventTypeName", @event.EventTypeId);
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,EventDate")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues
        .Include(v => v.Bookings)
        .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            if (venue.Bookings != null && venue.Bookings.Any())
            {
                ModelState.AddModelError("", "Cannot delete a venue with active bookings.");
                return View("Delete", venue);
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
