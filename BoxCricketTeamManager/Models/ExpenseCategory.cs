using System.ComponentModel.DataAnnotations;

namespace BoxCricketTeamManager.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
