using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Data;
using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Services
{
    public class PaymentService
    {
        public List<Payment> GetPaymentsByYear(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Include(p => p.Member)
                .Where(p => p.PaymentYear == year)
                .OrderBy(p => p.Member!.Name)
                .ThenBy(p => p.PaymentMonth)
                .ToList();
        }

        public List<Payment> GetPaymentsByMember(int memberId)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.PaymentYear)
                .ThenByDescending(p => p.PaymentMonth)
                .ToList();
        }

        public Payment? GetPayment(int memberId, int month, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .FirstOrDefault(p => p.MemberId == memberId &&
                                     p.PaymentMonth == month &&
                                     p.PaymentYear == year);
        }

        public bool IsPaymentMade(int memberId, int month, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments.Any(p => p.MemberId == memberId &&
                                             p.PaymentMonth == month &&
                                             p.PaymentYear == year);
        }

        public void AddPayment(Payment payment)
        {
            using var context = Program.CreateDbContext();
            payment.CreatedAt = DateTime.Now;
            context.Payments.Add(payment);
            context.SaveChanges();
        }

        public void RemovePayment(int memberId, int month, int year)
        {
            using var context = Program.CreateDbContext();
            var payment = context.Payments
                .FirstOrDefault(p => p.MemberId == memberId &&
                                     p.PaymentMonth == month &&
                                     p.PaymentYear == year);
            if (payment != null)
            {
                context.Payments.Remove(payment);
                context.SaveChanges();
            }
        }

        public void TogglePayment(int memberId, int month, int year, decimal amount)
        {
            using var context = Program.CreateDbContext();
            var existing = context.Payments
                .FirstOrDefault(p => p.MemberId == memberId &&
                                     p.PaymentMonth == month &&
                                     p.PaymentYear == year);

            if (existing != null)
            {
                context.Payments.Remove(existing);
            }
            else
            {
                var payment = new Payment
                {
                    MemberId = memberId,
                    PaymentMonth = month,
                    PaymentYear = year,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                context.Payments.Add(payment);
            }
            context.SaveChanges();
        }

        public decimal GetMonthlyCollection(int month, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Where(p => p.PaymentMonth == month && p.PaymentYear == year)
                .Sum(p => (decimal?)p.Amount) ?? 0;
        }

        public decimal GetYearlyCollection(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Where(p => p.PaymentYear == year)
                .Sum(p => (decimal?)p.Amount) ?? 0;
        }

        public decimal GetMemberYearlyTotal(int memberId, int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Where(p => p.MemberId == memberId && p.PaymentYear == year)
                .Sum(p => (decimal?)p.Amount) ?? 0;
        }

        public decimal GetMonthlyDueAmount(int year)
        {
            using var context = Program.CreateDbContext();
            var monthlyDue = context.MonthlyDues.FirstOrDefault(m => m.Year == year);
            return monthlyDue?.MonthlyAmount ?? 50;
        }

        public Dictionary<int, decimal> GetMonthlyCollectionsByYear(int year)
        {
            using var context = Program.CreateDbContext();
            return context.Payments
                .Where(p => p.PaymentYear == year)
                .GroupBy(p => p.PaymentMonth)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));
        }
    }
}
