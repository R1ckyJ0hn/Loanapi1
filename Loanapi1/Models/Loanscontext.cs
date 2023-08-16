using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Loanapi1.Models.Authentication.Login;
using Microsoft.AspNetCore.Identity;
using Loanapi1.Models.Authentication.AppUser;

namespace Loanapi1.Models
{
    public class Loanscontext : DbContext
    {
        public Loanscontext(DbContextOptions<Loanscontext> options) : base(options)
        {

        }

        public virtual DbSet<Loanapplication> loanapplications {get; set;}=null!;
        

        
    }
    public partial class ApplicationDbcontext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {
            //{
            //    Database.EnsureCreated();
            //}

        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //SeedRoles(builder);
        }
        //private static void SeedRoles(ModelBuilder builder)
        //{
        //    builder.Entity<IdentityRole>().HasData(
        //        new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
        //        new IdentityRole() { Name = "Loanofficer", ConcurrencyStamp = "2", NormalizedName = "Loanofficer" },
        //        new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" });
        //}
        
    }

}
