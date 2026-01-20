using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxCricketTeamManager.Models
{
    public class YearlyBalance
    {
        [Key]
        public int BalanceId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal OpeningBalance { get; set; } = 0;
    }
}
