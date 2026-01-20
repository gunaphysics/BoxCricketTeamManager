using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed default expense categories if none exist
            if (!context.ExpenseCategories.Any())
            {
                var categories = new ExpenseCategory[]
                {
                    new ExpenseCategory { CategoryName = "Equipment", IsActive = true },
                    new ExpenseCategory { CategoryName = "Turf", IsActive = true },
                    new ExpenseCategory { CategoryName = "Refreshments", IsActive = true },
                    new ExpenseCategory { CategoryName = "Maintenance", IsActive = true },
                    new ExpenseCategory { CategoryName = "Miscellaneous", IsActive = true }
                };

                context.ExpenseCategories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed default monthly due for current year if none exists
            int currentYear = DateTime.Now.Year;
            if (!context.MonthlyDues.Any(m => m.Year == currentYear))
            {
                var monthlyDue = new MonthlyDue
                {
                    Year = currentYear,
                    MonthlyAmount = 50,
                    EffectiveFrom = new DateTime(currentYear, 1, 1)
                };

                context.MonthlyDues.Add(monthlyDue);
                context.SaveChanges();
            }

            // Seed yearly balance for current year if none exists
            if (!context.YearlyBalances.Any(y => y.Year == currentYear))
            {
                var yearlyBalance = new YearlyBalance
                {
                    Year = currentYear,
                    OpeningBalance = 0
                };

                context.YearlyBalances.Add(yearlyBalance);
                context.SaveChanges();
            }
        }
    }
}
