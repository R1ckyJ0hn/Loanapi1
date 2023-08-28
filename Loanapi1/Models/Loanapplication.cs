using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Loanapi1.Models
{

    public class Loanapplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int ApplicationID { get; set; }
        public string? Name { get; set; }
        public int Amount { get; set; }

        public string? Loantype { get; set; }

        [JsonIgnore]
        public string Loanstatus { get; set; }
        public Loanapplication()
        {
            
            Loanstatus = "Pending";
        }
    }

    public class PendingLoanapplicationDto
    {
        public int ApplicationID { get; set; }
        public string? Name { get; set; }
        public int Amount { get; set; }
        public string? Loantype { get; set; }
        public string? Loanstatus { get; set; }
    }


}
