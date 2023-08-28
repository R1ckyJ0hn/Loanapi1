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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<Loantypes>().HasNoKey();
        //    //modelBuilder.Entity<IdentityUserLogin<string>>().HasNoKey();
        //    //modelBuilder.Entity<IdentityUserRole<string>>().HasNoKey();
        //    //modelBuilder.Entity<IdentityUserToken<string>>().HasNoKey();
        //}
        public  DbSet<Loanapplication> Loans { get; set; }
        public DbSet<Loantypes> types { get; set; }
    }

}
