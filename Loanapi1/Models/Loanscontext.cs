using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Loanapi1.Models.Authentication.Login;
using Microsoft.AspNetCore.Identity;
using Loanapi1.Models.Authentication;

namespace Loanapi1.Models
{
    
    public partial class Loanscontext : IdentityDbContext<IdentityUser>
    {
        public Loanscontext(DbContextOptions<Loanscontext> options) : base(options)
        {
           

        }
        
       
        public  DbSet<Loanapplication> Loans { get; set; } 
    }

}
