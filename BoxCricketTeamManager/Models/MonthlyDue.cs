using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxCricketTeamManager.Models
{
    public class MonthlyDue
    {
        [Key]
        public int DueId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyAmount { get; set; } = 50;

        public DateTime EffectiveFrom { get; set; } = DateTime.Now;
    }
}
