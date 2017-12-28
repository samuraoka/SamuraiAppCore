using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SamuraiAppCore.Domain;

namespace SamuraiAppCore.Data
{
    /// <summary>
    /// To create a data model, add following NuGet package using the below command in Package Manager
    /// PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer -ProjectName SamuraiAppCore.Data -Version 2.0.0
    /// https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/
    /// 
    /// To create migration files, add following NuGet package using the below command in Package Manager
    /// PM> Install-Package Microsoft.EntityFrameworkCore.Tools -ProjectName SamuraiAppCore.Data -Version 2.0.0
    /// PM> Install-Package Microsoft.EntityFrameworkCore.Design -ProjectName SamuraiAppCore.CoreUI -Version 2.0.0
    /// https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/
    /// 
    /// To get help messages, use a following command in Package Manager.
    /// PM> get-help entityframeworkcore
    /// 
    /// To add a new migration, execute a following command
    /// PM> Add-Migration -Name init -Context SamuraiContext -Project SamuraiAppCore.Data -StartupProject SamuraiAppCore.CoreUI
    /// 
    /// To Create or Update database, run following command in the Package Manager Console
    /// PM> Update-Database -Context SamuraiContext -Project SamuraiAppCore.Data -StartupProject SamuraiAppCore.CoreUI
    /// 
    /// To generate a SQL script from migrations. Run following command in the Package Manager Console
    /// PM> Script-Migration -Idempotent -Context SamuraiContext -Project SamuraiAppCore.Data -StartupProject SamuraiAppCore.CoreUI
    /// </summary>
    public class SamuraiContext : DbContext
    {
        // Logging
        // https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging
        //
        // Microsoft.Extensions.Logging.Console
        // https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console/
        // Install-Package Microsoft.Extensions.Logging.Console -ProjectName SamuraiAppCore.Data -Version 2.0.0
        public static readonly LoggerFactory SamuraiLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        /// <summary>
        /// The following URL shows how to write connection strings
        /// https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring(v=vs.110).aspx
        /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/sqlclient-support-for-localdb
        /// https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-2016-express-localdb
        /// https://docs.microsoft.com/en-us/sql/odbc/reference/syntax/sqldriverconnect-function
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;Database=SamuraiDataCore;AttachDbFileName=E:\sato\MSSQLLocalDB\SamuraiDataCore\SamuraiDataCore.mdf";
            optionsBuilder.UseSqlServer(connectionString, options => options.MaxBatchSize(30));

            // Logging
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging
            optionsBuilder.UseLoggerFactory(SamuraiLoggerFactory);
            // Sensitive data logging is enabled.
            // Log entries and exception messages may include sensitive application data,
            // this mode should only be enabled during development.
            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// Keys (primary)
        /// https://docs.microsoft.com/en-us/ef/core/modeling/keys
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>().HasKey(sb => new { sb.SamuraiId, sb.BattleId });
        }
    }
}
