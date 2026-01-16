using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WagerWiseJournal.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Casino> Casinos { get; set; }
    public DbSet<BettingSession> BettingSessions { get; set; }
    public DbSet<Bet> Bets { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Casino>(entity =>
        {
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
            entity.HasIndex(e => e.Name);
        });
        
        modelBuilder.Entity<BettingSession>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CasinoId);
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.IsActive);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.BettingSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Casino)
                .WithMany(c => c.BettingSessions)
                .HasForeignKey(e => e.CasinoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.GameType);
            
            entity.HasOne(e => e.Session)
                .WithMany(s => s.Bets)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
