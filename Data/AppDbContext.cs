using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data;

public class AppDbContext : DbContext
{
    // Construtor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Access> Accesses { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookGender> BookGenders { get; set; }
    public DbSet<BookPublisher> BookPublishers { get; set; }
    public DbSet<BookStatus> BookStatus { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<DepartmentUser> DepartmentUsers { get; set; } // Tabela de junção
    public DbSet<AccessPermission> AccessPermissions { get; set; } // Tabela de junção

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
