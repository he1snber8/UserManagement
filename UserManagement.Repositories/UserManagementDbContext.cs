using Microsoft.EntityFrameworkCore;
using UserManagement.DTO;
using UserManagement.Facade;

namespace UserManagement.Repositories;

public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
             .HasKey(u => u.Id);

        modelBuilder.Entity<UserProfile>()
            .HasKey(up => up.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(30)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(
            p => p.HashData(),
            p => p);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.Firstname)
            .HasMaxLength(25)
            .IsRequired();

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.LastName)
            .HasMaxLength(35)
            .IsRequired();

        modelBuilder.Entity<User>()
           .Property(u => u.isActive)
           .HasDefaultValue(true);

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.PersonalNumber)
            .HasMaxLength(11)
            .HasColumnType("char(11)")
            .IsRequired();

        modelBuilder.Entity<User>()
           .HasOne<UserProfile>()
           .WithOne()
           .HasForeignKey<User>(u => u.Id);
    }
}