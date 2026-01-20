using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Data;
using BoxCricketTeamManager.Models;

namespace BoxCricketTeamManager.Services
{
    public class MemberService
    {
        public List<Member> GetAllMembers(bool includeInactive = false)
        {
            using var context = Program.CreateDbContext();
            var query = context.Members.AsQueryable();
            if (!includeInactive)
                query = query.Where(m => m.IsActive);
            return query.OrderBy(m => m.Name).ToList();
        }

        public Member? GetMemberById(int memberId)
        {
            using var context = Program.CreateDbContext();
            return context.Members.Find(memberId);
        }

        public List<Member> SearchMembers(string searchTerm, bool includeInactive = false)
        {
            using var context = Program.CreateDbContext();
            var query = context.Members.AsQueryable();
            if (!includeInactive)
                query = query.Where(m => m.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(m =>
                    m.Name.ToLower().Contains(searchTerm) ||
                    (m.Phone != null && m.Phone.Contains(searchTerm)) ||
                    (m.Email != null && m.Email.ToLower().Contains(searchTerm)));
            }

            return query.OrderBy(m => m.Name).ToList();
        }

        public void AddMember(Member member)
        {
            using var context = Program.CreateDbContext();
            member.CreatedAt = DateTime.Now;
            context.Members.Add(member);
            context.SaveChanges();
        }

        public void UpdateMember(Member member)
        {
            using var context = Program.CreateDbContext();
            var existing = context.Members.Find(member.MemberId);
            if (existing != null)
            {
                existing.Name = member.Name;
                existing.Phone = member.Phone;
                existing.Email = member.Email;
                existing.JoinDate = member.JoinDate;
                existing.IsActive = member.IsActive;
                existing.Notes = member.Notes;
                context.SaveChanges();
            }
        }

        public void DeactivateMember(int memberId)
        {
            using var context = Program.CreateDbContext();
            var member = context.Members.Find(memberId);
            if (member != null)
            {
                member.IsActive = false;
                context.SaveChanges();
            }
        }

        public void ActivateMember(int memberId)
        {
            using var context = Program.CreateDbContext();
            var member = context.Members.Find(memberId);
            if (member != null)
            {
                member.IsActive = true;
                context.SaveChanges();
            }
        }

        public void DeleteMember(int memberId)
        {
            using var context = Program.CreateDbContext();
            var member = context.Members.Find(memberId);
            if (member != null)
            {
                context.Members.Remove(member);
                context.SaveChanges();
            }
        }

        public int GetActiveMembersCount()
        {
            using var context = Program.CreateDbContext();
            return context.Members.Count(m => m.IsActive);
        }

        public List<Member> GetMembersWithPendingPayments(int month, int year)
        {
            using var context = Program.CreateDbContext();
            var paidMemberIds = context.Payments
                .Where(p => p.PaymentMonth == month && p.PaymentYear == year)
                .Select(p => p.MemberId)
                .ToList();

            return context.Members
                .Where(m => m.IsActive && !paidMemberIds.Contains(m.MemberId))
                .OrderBy(m => m.Name)
                .ToList();
        }
    }
}
