using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Data;
using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Services
{
    public class ReportService
    {
        public class YearlySummary
        {
            public int Year { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal TotalCollections { get; set; }
            public decimal TotalExpenses { get; set; }
            public decimal ClosingBalance => OpeningBalance + TotalCollections - TotalExpenses;
        }

        public class MonthlySummary
        {
            public int Month { get; set; }
            public string MonthName => new DateTime(2000, Month, 1).ToString("MMMM");
            public decimal Collections { get; set; }
            public decimal Expenses { get; set; }
            public decimal NetBalance => Collections - Expenses;
        }

        public class MemberPaymentSummary
        {
            public int MemberId { get; set; }
            public string MemberName { get; set; } = string.Empty;
            public bool[] MonthsPaid { get; set; } = new bool[12];
            public decimal TotalPaid { get; set; }
            public int MonthsPaidCount { get; set; }
        }

        public YearlySummary GetYearlySummary(int year)
        {
            using var context = Program.CreateDbContext();

            var yearlyBalance = context.YearlyBalances.FirstOrDefault(y => y.Year == year);
            decimal openingBalance = yearlyBalance?.OpeningBalance ?? 0;

            decimal totalCollections = context.Payments
                .Where(p => p.PaymentYear == year)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            decimal totalExpenses = context.Expenses
                .Where(e => e.ExpenseYear == year)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            return new YearlySummary
            {
                Year = year,
                OpeningBalance = openingBalance,
                TotalCollections = totalCollections,
                TotalExpenses = totalExpenses
            };
        }

        public List<MonthlySummary> GetMonthlySummaries(int year)
        {
            using var context = Program.CreateDbContext();

            var monthlyCollections = context.Payments
                .Where(p => p.PaymentYear == year)
                .GroupBy(p => p.PaymentMonth)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            var monthlyExpenses = context.Expenses
                .Where(e => e.ExpenseYear == year)
                .GroupBy(e => e.ExpenseMonth)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var summaries = new List<MonthlySummary>();
            for (int month = 1; month <= 12; month++)
            {
                summaries.Add(new MonthlySummary
                {
                    Month = month,
                    Collections = monthlyCollections.GetValueOrDefault(month, 0),
                    Expenses = monthlyExpenses.GetValueOrDefault(month, 0)
                });
            }

            return summaries;
        }

        public List<MemberPaymentSummary> GetMemberPaymentSummaries(int year)
        {
            using var context = Program.CreateDbContext();

            var members = context.Members
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToList();

            var payments = context.Payments
                .Where(p => p.PaymentYear == year)
                .ToList();

            var summaries = new List<MemberPaymentSummary>();
            foreach (var member in members)
            {
                var memberPayments = payments.Where(p => p.MemberId == member.MemberId).ToList();
                var monthsPaid = new bool[12];

                foreach (var payment in memberPayments)
                {
                    if (payment.PaymentMonth >= 1 && payment.PaymentMonth <= 12)
                        monthsPaid[payment.PaymentMonth - 1] = true;
                }

                summaries.Add(new MemberPaymentSummary
                {
                    MemberId = member.MemberId,
                    MemberName = member.Name,
                    MonthsPaid = monthsPaid,
                    TotalPaid = memberPayments.Sum(p => p.Amount),
                    MonthsPaidCount = monthsPaid.Count(m => m)
                });
            }

            return summaries;
        }

        public List<Expense> GetExpenseReport(int year, int? categoryId = null)
        {
            using var context = Program.CreateDbContext();
            var query = context.Expenses
                .Include(e => e.Category)
                .Where(e => e.ExpenseYear == year);

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId);

            return query
                .OrderBy(e => e.ExpenseMonth)
                .ThenBy(e => e.ExpenseDate)
                .ToList();
        }

        public decimal GetCurrentBalance(int year)
        {
            var summary = GetYearlySummary(year);
            return summary.ClosingBalance;
        }

        public List<int> GetAvailableYears()
        {
            using var context = Program.CreateDbContext();

            var paymentYears = context.Payments.Select(p => p.PaymentYear).Distinct().ToList();
            var expenseYears = context.Expenses.Select(e => e.ExpenseYear).Distinct().ToList();
            var balanceYears = context.YearlyBalances.Select(y => y.Year).Distinct().ToList();

            var allYears = paymentYears.Union(expenseYears).Union(balanceYears).ToList();

            // Add current year if not present
            int currentYear = DateTime.Now.Year;
            if (!allYears.Contains(currentYear))
                allYears.Add(currentYear);

            return allYears.OrderByDescending(y => y).ToList();
        }

        public void SetOpeningBalance(int year, decimal balance)
        {
            using var context = Program.CreateDbContext();
            var existing = context.YearlyBalances.FirstOrDefault(y => y.Year == year);

            if (existing != null)
            {
                existing.OpeningBalance = balance;
            }
            else
            {
                context.YearlyBalances.Add(new YearlyBalance
                {
                    Year = year,
                    OpeningBalance = balance
                });
            }

            context.SaveChanges();
        }

        public void SetMonthlyDueAmount(int year, decimal amount)
        {
            using var context = Program.CreateDbContext();
            var existing = context.MonthlyDues.FirstOrDefault(m => m.Year == year);

            if (existing != null)
            {
                existing.MonthlyAmount = amount;
                existing.EffectiveFrom = DateTime.Now;
            }
            else
            {
                context.MonthlyDues.Add(new MonthlyDue
                {
                    Year = year,
                    MonthlyAmount = amount,
                    EffectiveFrom = new DateTime(year, 1, 1)
                });
            }

            context.SaveChanges();
        }

        public decimal GetMonthlyDueAmount(int year)
        {
            using var context = Program.CreateDbContext();
            var monthlyDue = context.MonthlyDues.FirstOrDefault(m => m.Year == year);
            return monthlyDue?.MonthlyAmount ?? 50;
        }
    }
}
