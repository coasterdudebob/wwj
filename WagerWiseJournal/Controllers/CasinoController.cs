using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WagerWiseJournal.Models;

namespace WagerWiseJournal.Controllers;

[Authorize]
public class CasinoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CasinoController> _logger;

    public CasinoController(ApplicationDbContext context, ILogger<CasinoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var casinos = await _context.Casinos.OrderBy(c => c.Name).ToListAsync();
        return View(casinos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Casino casino)
    {
        if (ModelState.IsValid)
        {
            _context.Add(casino);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(casino);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var casino = await _context.Casinos
            .Include(c => c.BettingSessions)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (casino == null)
        {
            return NotFound();
        }

        return View(casino);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var casino = await _context.Casinos.FindAsync(id);
        if (casino == null)
        {
            return NotFound();
        }
        return View(casino);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Casino casino)
    {
        if (id != casino.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(casino);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CasinoExists(casino.Id))
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
        return View(casino);
    }

    [HttpGet]
    public async Task<IActionResult> GetNearby(double latitude, double longitude, double radiusKm = 50)
    {
        var casinos = await _context.Casinos.ToListAsync();
        
        var nearbyCasinos = casinos.Where(c =>
        {
            var distance = CalculateDistance(latitude, longitude, c.Latitude, c.Longitude);
            return distance <= radiusKm;
        }).Select(c => new
        {
            c.Id,
            c.Name,
            c.Address,
            c.Latitude,
            c.Longitude,
            Distance = CalculateDistance(latitude, longitude, c.Latitude, c.Longitude)
        }).OrderBy(c => c.Distance).ToList();

        return Json(nearbyCasinos);
    }

    private bool CasinoExists(int id)
    {
        return _context.Casinos.Any(e => e.Id == id);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Radius of the Earth in kilometers
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
