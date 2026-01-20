using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BoxCricketTeamManager.Data;
using BoxCricketTeamManager.Forms;

namespace BoxCricketTeamManager;

static class Program
{
    public static IConfiguration? Configuration { get; private set; }
    public static string ConnectionString { get; private set; } = string.Empty;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Load configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BoxCricketTeamManager", "BoxCricketTeamManager.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        ConnectionString = Configuration.GetConnectionString("DefaultConnection") ??
            $"Data Source={dbPath}";

        // Initialize database
        using (var context = CreateDbContext())
        {
            DbInitializer.Initialize(context);
        }

        Application.Run(new MainForm());
    }

    public static AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(ConnectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
