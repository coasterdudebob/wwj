using System.ComponentModel.DataAnnotations;

namespace WagerWiseJournal.Models;

public class Casino
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    [StringLength(50)]
    public string? City { get; set; }
    
    [StringLength(50)]
    public string? State { get; set; }
    
    [StringLength(20)]
    public string? ZipCode { get; set; }
    
    [StringLength(50)]
    public string? Country { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public ICollection<BettingSession> BettingSessions { get; set; } = new List<BettingSession>();
}
