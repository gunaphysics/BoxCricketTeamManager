using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<MonthlyDue> MonthlyDues { get; set; }
        public DbSet<YearlyBalance> YearlyBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint for MonthlyDue.Year
            modelBuilder.Entity<MonthlyDue>()
                .HasIndex(m => m.Year)
                .IsUnique();

            // Unique constraint for YearlyBalance.Year
            modelBuilder.Entity<YearlyBalance>()
                .HasIndex(y => y.Year)
                .IsUnique();

            // Unique constraint for ExpenseCategory.CategoryName
            modelBuilder.Entity<ExpenseCategory>()
                .HasIndex(e => e.CategoryName)
                .IsUnique();

            // Unique constraint for Payment (MemberId, PaymentMonth, PaymentYear)
            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.MemberId, p.PaymentMonth, p.PaymentYear })
                .IsUnique();

            // Configure Member-Payment relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ExpenseCategory-Expense relationship
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
