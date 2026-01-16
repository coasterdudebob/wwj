using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WagerWiseJournal.Models;

public class BettingSession
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;
    
    [Required]
    public int CasinoId { get; set; }
    
    [ForeignKey("CasinoId")]
    public Casino Casino { get; set; } = null!;
    
    [Required]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    
    [NotMapped]
    public decimal TotalWagered => Bets.Sum(b => b.Amount);
    
    [NotMapped]
    public decimal TotalWinnings => Bets.Sum(b => b.Winnings);
    
    [NotMapped]
    public decimal NetProfit => TotalWinnings - TotalWagered;
}
