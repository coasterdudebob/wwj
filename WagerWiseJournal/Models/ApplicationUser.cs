using Microsoft.AspNetCore.Identity;

namespace WagerWiseJournal.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public ICollection<BettingSession> BettingSessions { get; set; } = new List<BettingSession>();
}
