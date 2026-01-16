using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WagerWiseJournal.Models;

namespace WagerWiseJournal.Controllers;

[Authorize]
public class SessionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SessionController> _logger;

    public SessionController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<SessionController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var sessions = await _context.BettingSessions
            .Include(s => s.Casino)
            .Include(s => s.Bets)
            .Where(s => s.UserId == user.Id)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        return View(sessions);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Casinos = new SelectList(await _context.Casinos.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BettingSession session)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        session.UserId = user.Id;
        session.StartTime = DateTime.UtcNow;
        session.IsActive = true;

        if (ModelState.IsValid)
        {
            _context.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = session.Id });
        }

        ViewBag.Casinos = new SelectList(await _context.Casinos.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", session.CasinoId);
        return View(session);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var session = await _context.BettingSessions
            .Include(s => s.Casino)
            .Include(s => s.Bets)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

        if (session == null)
        {
            return NotFound();
        }

        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EndSession(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var session = await _context.BettingSessions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

        if (session == null)
        {
            return NotFound();
        }

        session.EndTime = DateTime.UtcNow;
        session.IsActive = false;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Active()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var activeSession = await _context.BettingSessions
            .Include(s => s.Casino)
            .Include(s => s.Bets)
            .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsActive);

        if (activeSession == null)
        {
            return RedirectToAction(nameof(Create));
        }

        return RedirectToAction(nameof(Details), new { id = activeSession.Id });
    }
}
