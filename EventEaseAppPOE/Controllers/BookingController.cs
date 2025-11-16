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
    public class BookingController : Controller
    {
        private readonly EventeaseDbContext _context;

        public BookingController(EventeaseDbContext context)
        {
            _context = context;
        }

        // GET: Booking
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // Applying search filter
            if (!string.IsNullOrEmpty(Request.Query["search"]))
            {
                var searchTerm = Request.Query["search"].ToString();
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchTerm) ||
                    b.Event.EventName.Contains(searchTerm));

            }

            // Return filtered or full list to the view
            return View(await bookings.ToListAsync());
        }

        //filters through bookings based on event type, date range, and venue availability
        [HttpGet]
        public async Task<IActionResult> Filter(string? selectedEventType, DateTime? startDate, DateTime? endDate, bool? venueAvailable)
        {
            var bookingsQuery = _context.Bookings
            .Include(b => b.Event).ThenInclude(e => e.EventType)
            .Include(b => b.Venue)
            .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(selectedEventType))
                bookingsQuery = bookingsQuery.Where(b => b.Event.EventType.EventTypeName == selectedEventType);

            if (startDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate.Value);

            if (endDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate.Value);

            if (venueAvailable.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.Venue.IsAvailable == venueAvailable.Value);

            var model = new BookingFilterViewModel
            {
                SelectedEventType = selectedEventType,
                StartDate = startDate,
                EndDate = endDate,
                VenueAvailable = venueAvailable,
                EventTypes = await _context.EventTypes.Select(e => e.EventTypeName).ToListAsync(),
                FilteredBookings = await bookingsQuery.ToListAsync()
            };

            return View(model);
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
