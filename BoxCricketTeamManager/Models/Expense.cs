using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxCricketTeamManager.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 12)]
        public int ExpenseMonth { get; set; }

        [Required]
        public int ExpenseYear { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("CategoryId")]
        public virtual ExpenseCategory? Category { get; set; }
    }
}
