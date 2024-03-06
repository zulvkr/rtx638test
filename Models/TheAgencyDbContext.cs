using Microsoft.EntityFrameworkCore;

namespace TheAgencyApi.Models;

public class TheAgencyDbContext : DbContext
{
    public string DbPath { get; }

    public TheAgencyDbContext(DbContextOptions<TheAgencyDbContext> options)
        : base(options)
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
        var path = System.IO.Directory.GetCurrentDirectory();
        DbPath = System.IO.Path.Join(path, "agency.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");

    public DbSet<Customer> Customer { get; set; } = null!;
    public DbSet<Appointment> Appointment { get; set; } = null!;
}