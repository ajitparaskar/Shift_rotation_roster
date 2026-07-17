using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<ShiftPreference> ShiftPreferences => Set<ShiftPreference>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<SchedulingPolicy> SchedulingPolicies => Set<SchedulingPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShiftPreference>()
            .HasOne(p => p.Employee)
            .WithMany(e => e.ShiftPreferences)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShiftPreference>()
            .HasOne(p => p.Shift)
            .WithMany(s => s.ShiftPreferences)
            .HasForeignKey(p => p.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.ShiftAssignments)
            .HasForeignKey(a => a.EmployeeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(a => a.Shift)
            .WithMany(s => s.ShiftAssignments)
            .HasForeignKey(a => a.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent the same employee being assigned twice on the same day+shift
        modelBuilder.Entity<ShiftAssignment>()
            .HasIndex(a => new { a.EmployeeId, a.ShiftId, a.Date })
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();
    }
}
