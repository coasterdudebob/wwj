using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WagerWiseJournal.Models;

namespace WagerWiseJournal.Controllers;

[Authorize]
public class BetController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<BetController> _logger;

    public BetController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<BetController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bet bet)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var session = await _context.BettingSessions
            .FirstOrDefaultAsync(s => s.Id == bet.SessionId && s.UserId == user.Id);

        if (session == null)
        {
            return NotFound();
        }

        bet.Timestamp = DateTime.UtcNow;

        if (ModelState.IsValid)
        {
            _context.Add(bet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Session", new { id = bet.SessionId });
        }

        return RedirectToAction("Details", "Session", new { id = bet.SessionId });
    }

    public async Task<IActionResult> Edit(int? id)
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

        var bet = await _context.Bets
            .Include(b => b.Session)
            .FirstOrDefaultAsync(b => b.Id == id && b.Session.UserId == user.Id);

        if (bet == null)
        {
            return NotFound();
        }

        return View(bet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Bet bet)
    {
        if (id != bet.Id)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var existingBet = await _context.Bets
            .Include(b => b.Session)
            .FirstOrDefaultAsync(b => b.Id == id && b.Session.UserId == user.Id);

        if (existingBet == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingBet.GameType = bet.GameType;
                existingBet.Amount = bet.Amount;
                existingBet.Winnings = bet.Winnings;
                existingBet.Description = bet.Description;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BetExists(bet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", "Session", new { id = existingBet.SessionId });
        }
        return View(bet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var bet = await _context.Bets
            .Include(b => b.Session)
            .FirstOrDefaultAsync(b => b.Id == id && b.Session.UserId == user.Id);

        if (bet == null)
        {
            return NotFound();
        }

        var sessionId = bet.SessionId;
        _context.Bets.Remove(bet);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Session", new { id = sessionId });
    }

    private bool BetExists(int id)
    {
        return _context.Bets.Any(e => e.Id == id);
    }
}
