using Microsoft.EntityFrameworkCore;

namespace TrackerDeFavorisApi.Models;

public class ApplicationContext : DbContext
{
    private string _dataSource;
    public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration) : base(options)
    {
        string? dataSource = configuration.GetConnectionString("Sqlite");
        if (dataSource == null)
        {
            throw new InvalidAppSettingsException();
        }
        _dataSource = dataSource;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_dataSource);
    }

    public DbSet<User> UserList { get; set; } = null!;
    public DbSet<Film> FilmList { get; set; } = null!;
    public DbSet<Favorite> FavoriteList { get; set; } = null!;
}
