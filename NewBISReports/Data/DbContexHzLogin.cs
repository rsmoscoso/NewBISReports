using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace NewBISReports.Data
{
    public class DbContexHzLogin : IdentityDbContext<IdentityUser>
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
