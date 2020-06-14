using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewBISReports.Models.Autorizacao;


namespace NewBISReports.Data
{
    public class DbContexHzLogin : IdentityDbContext<ApplicationUser>
    {

        public DbContexHzLogin(DbContextOptions<DbContexHzLogin> opts)
            :base(opts)
        { }
        protected override void OnModelCreating( ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
