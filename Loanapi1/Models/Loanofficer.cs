using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loanapi1.Models
{
    public class Loanofficer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfficerID { get; set; }
        public string? Firstname  { get; set; }

        public string? Lastname { get; set; }

        public string? UserID { get; set; }

        public string? Pass { get; set; }

    }
    
}
