using Microsoft.EntityFrameworkCore;
using TheAgencyApi.Models;

namespace TheAgencyApi.Data;

public class TheAgencyDbContext : DbContext
{
    public string DbPath { get; }

    public TheAgencyDbContext(DbContextOptions<TheAgencyDbContext> options)
        : base(options)
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
        var path = Directory.GetCurrentDirectory();
        DbPath = Path.Join(path, "agency.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");

    public DbSet<Customer> Customer { get; set; } = null!;
    public DbSet<Appointment> Appointment { get; set; } = null!;
    public DbSet<DateConfiguration> DateConfiguration { get; set; } = null!;
    public DbSet<DefaultDateConfiguration> DefaultDateConfiguration { get; set; } = null!;


    // Seed
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "John Doe", Email = "johndoe@mail.com" },
            new Customer { Id = 2, FullName = "Donald Duck", Email = "donaldduck@mail.com" }
        );

        modelBuilder.Entity<DateConfiguration>().HasData(
            // Full day example (Friday)
            new DateConfiguration { Date = new DateTime(2030, 1, 4), IsOffDay = false, MaxAppointments = 8 },

            // Off day example (Monday)
            new DateConfiguration { Date = new DateTime(2030, 1, 7), IsOffDay = true }
        );

        modelBuilder.Entity<DefaultDateConfiguration>().HasData(
            new DefaultDateConfiguration
            {
                Id = 1,
                WeeklyHolidays = [DayOfWeek.Saturday, DayOfWeek.Sunday],
                YearlyHolidays = [new DateTime(2030, 1, 4)],
            }
        );

        modelBuilder.Entity<Appointment>().HasData(
            // Full day example
            new Appointment { Id = 1, CustomerId = 1, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 2, CustomerId = 2, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 3, CustomerId = 1, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 4, CustomerId = 2, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 5, CustomerId = 2, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 6, CustomerId = 1, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 7, CustomerId = 1, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },
            new Appointment { Id = 8, CustomerId = 1, Date = new DateTime(2030, 1, 4), Token = Guid.NewGuid() },

            // Regular day example
            new Appointment { Id = 9, CustomerId = 1, Date = new DateTime(2030, 1, 8), Token = Guid.NewGuid() }
        );

    }
}