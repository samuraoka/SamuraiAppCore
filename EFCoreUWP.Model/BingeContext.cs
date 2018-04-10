using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Windows.Storage;

namespace EFCoreUWP.Model
{
    // Getting Started with EF Core on Universal Windows Platform (UWP) with a New Database
    // https://docs.microsoft.com/en-us/ef/core/get-started/uwp/getting-started
    //
    // Microsoft.EntityFrameworkCore
    // https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/
    // Install-Package -Id Microsoft.EntityFrameworkCore -ProjectName EFCoreUWP.Model
    //
    // Microsoft.EntityFrameworkCore.Sqlite
    // https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/
    // Install-Package -Id Microsoft.EntityFrameworkCore.Sqlite -ProjectName EFCoreUWP.Model
    //
    // Microsoft.EntityFrameworkCore.Tools
    // https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/
    // Install-Package -Id Microsoft.EntityFrameworkCore.Tools -ProjectName EFCoreUWP.Model
    //
    // Add-Migration -Name Initial -Context BingeContext -Project EFCoreUWP.Model -StartupProject EFCoreUWP.Model
    public class BingeContext : DbContext
    {
        public DbSet<CookieBinge> Binges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databaseFilePath = "CookieBinge.db";
            try
            {
                databaseFilePath = Path.Combine(
                    ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }
    }
}
