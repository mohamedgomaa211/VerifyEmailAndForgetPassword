global using Microsoft.EntityFrameworkCore;
using VerifyEmailForgetPassword.Models;

namespace VerifyEmailForgetPassword.Data
{
    public class DataContext   :DbContext
    {

        public DataContext(DbContextOptions<DataContext> options):base(options)
        {


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-F2O98HA;Database=UserDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        public DbSet<User> Users =>Set<User>();
    }
}
