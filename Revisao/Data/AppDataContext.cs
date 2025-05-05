using Microsoft.EntityFrameworkCore;
using Revisao.Models;

namespace Revisao.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions options) :
        base(options)
    { }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Usuarios { get; set; }
}