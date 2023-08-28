using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Loanapi1.Models
{
    public class Loantypes
    {
        [Key]
        [JsonIgnore]
        public int TypeId { get; set; }
        public string? loantype { get; set; }

    }
}
