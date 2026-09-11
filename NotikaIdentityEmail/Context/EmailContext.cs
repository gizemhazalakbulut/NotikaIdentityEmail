using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NotikaIdentityEmail.Context
{
    public class EmailContext:IdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost; initial Catalog=NotikaEmailDb; integrated security=true; trust server certificate=true;");
        }
    }
}
