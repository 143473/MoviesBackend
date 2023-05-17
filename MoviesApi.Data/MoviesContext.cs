using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Data;

public class MoviesContext : IdentityDbContext<IdentityUser>
{
    public MoviesContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}