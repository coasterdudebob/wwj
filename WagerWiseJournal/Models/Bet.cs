using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WagerWiseJournal.Models;

public class Bet
{
    public int Id { get; set; }
    
    [Required]
    public int SessionId { get; set; }
    
    [ForeignKey("SessionId")]
    public BettingSession Session { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string GameType { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Winnings { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [NotMapped]
    public decimal NetResult => Winnings - Amount;
}
