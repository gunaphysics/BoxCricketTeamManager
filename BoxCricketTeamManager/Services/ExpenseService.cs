using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Data;
using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Services
{
    public class ExpenseService
    {
        public List<Expense> GetAllExpenses(int? year = null)
        {
            using var context = Program.CreateDbContext();
            var query = context.Expenses.Include(e => e.Category).AsQueryable();

            if (year.HasValue)
                query = query.Where(e => e.ExpenseYear == year.Value);

            return query
                .OrderByDescending(e => e.ExpenseYear)
                .ThenByDescending(e => e.ExpenseMonth)
                .ThenByDescending(e => e.ExpenseDate)
                .ToList();
        }

        public List<Expense> GetExpensesByMonth(int month, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Include(e => e.Category)
                .Where(e => e.ExpenseMonth == month && e.ExpenseYear == year)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();
        }

        public Expense? GetExpenseById(int expenseId)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Include(e => e.Category)
                .FirstOrDefault(e => e.ExpenseId == expenseId);
        }

        public void AddExpense(Expense expense)
        {
            using var context = Program.CreateDbContext();
            expense.CreatedAt = DateTime.Now;
            context.Expenses.Add(expense);
            context.SaveChanges();
        }

        public void UpdateExpense(Expense expense)
        {
            using var context = Program.CreateDbContext();
            var existing = context.Expenses.Find(expense.ExpenseId);
            if (existing != null)
            {
                existing.CategoryId = expense.CategoryId;
                existing.Description = expense.Description;
                existing.Amount = expense.Amount;
                existing.ExpenseMonth = expense.ExpenseMonth;
                existing.ExpenseYear = expense.ExpenseYear;
                existing.ExpenseDate = expense.ExpenseDate;
                existing.Notes = expense.Notes;
                context.SaveChanges();
            }
        }

        public void DeleteExpense(int expenseId)
        {
            using var context = Program.CreateDbContext();
            var expense = context.Expenses.Find(expenseId);
            if (expense != null)
            {
                context.Expenses.Remove(expense);
                context.SaveChanges();
            }
        }

        public decimal GetMonthlyExpenses(int month, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Where(e => e.ExpenseMonth == month && e.ExpenseYear == year)
                .AsEnumerable()
                .Sum(e => e.Amount);
        }

        public decimal GetYearlyExpenses(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Where(e => e.ExpenseYear == year)
                .AsEnumerable()
                .Sum(e => e.Amount);
        }

        public Dictionary<string, decimal> GetExpensesByCategory(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Include(e => e.Category)
                .Where(e => e.ExpenseYear == year)
                .AsEnumerable()
                .GroupBy(e => e.Category != null ? e.Category.CategoryName : "Uncategorized")
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        public Dictionary<int, decimal> GetMonthlyExpensesByYear(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Expenses
                .Where(e => e.ExpenseYear == year)
                .AsEnumerable()
                .GroupBy(e => e.ExpenseMonth)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        // Category operations
        public List<ExpenseCategory> GetAllCategories(bool includeInactive = false)
        {
            using var context = Program.CreateDbContext();
            var query = context.ExpenseCategories.AsQueryable();
            if (!includeInactive)
                query = query.Where(c => c.IsActive);
            return query.OrderBy(c => c.CategoryName).ToList();
        }

        public void AddCategory(ExpenseCategory category)
        {
            using var context = Program.CreateDbContext();
            context.ExpenseCategories.Add(category);
            context.SaveChanges();
        }

        public void UpdateCategory(ExpenseCategory category)
        {
            using var context = Program.CreateDbContext();
            var existing = context.ExpenseCategories.Find(category.CategoryId);
            if (existing != null)
            {
                existing.CategoryName = category.CategoryName;
                existing.IsActive = category.IsActive;
                context.SaveChanges();
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using var context = Program.CreateDbContext();
            var category = context.ExpenseCategories.Find(categoryId);
            if (category != null)
            {
                category.IsActive = false;
                context.SaveChanges();
            }
        }
    }
}
