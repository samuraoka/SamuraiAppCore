using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamuraiAppCore.CoreUI
{
    internal class Program
    {
        private static SamuraiContext _context;

        private static void Main(string[] args)
        {
            Console.WriteLine("Program has started.");
            DoAction().Wait();
            Console.WriteLine("Program is exiting.");
        }

        private static async Task DoAction()
        {
            using (_context = new SamuraiContext())
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();

                //await InsertNewPkFkGraphAsync();
                //await InsertNewOneToOneGraphAsync();
                //await AddChildToExistingObjectAsync();
                //await AddOneToOneToExistingObjectWhileTrackedAsync();
                //await ReplaceOneToOneToExistingObjectWhileTrackedAsync();
                //await AddManyToManyWithFksAsync();
                await AddManyToManyWithObjectsAsync();
            }
            _context = null;
        }

        private static async Task InsertNewPkFkGraphAsync()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" },
                    new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" },
                },
            };
            await _context.Samurais.AddAsync(samurai);
            await _context.SaveChangesAsync();
        }

        private static async Task InsertNewOneToOneGraphAsync()
        {
            var samurai = new Samurai { Name = "Shichiroji" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            await _context.AddAsync(samurai);
            await _context.SaveChangesAsync();
        }

        private static async Task AddChildToExistingObjectAsync()
        {
            await InsertNewOneToOneGraphAsync();

            var samurai = await _context.Samurais.FirstAsync();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!",
            });
            await _context.SaveChangesAsync();
        }

        private static async Task AddOneToOneToExistingObjectWhileTrackedAsync()
        {
            await InsertNewPkFkGraphAsync();

            var samurai = await _context.Samurais.FirstOrDefaultAsync(s => s.SecretIdentity == null);
            if (samurai != null)
            {
                samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
                await _context.SaveChangesAsync();
            }
        }

        private static async Task ReplaceOneToOneToExistingObjectWhileTrackedAsync()
        {
            await InsertNewOneToOneGraphAsync();

            var samurai = await _context.Samurais.FirstOrDefaultAsync(
                s => s.SecretIdentity.RealName != "Baba");
            if (samurai != null)
            {
                // Explicit loading - Loading Related Data
                // https://docs.microsoft.com/en-us/ef/core/querying/related-data#explicit-loading
                await _context.Entry(samurai).Reference(s => s.SecretIdentity).LoadAsync();
                samurai.SecretIdentity = new SecretIdentity { RealName = "Baba" };
                await _context.SaveChangesAsync();
            }
        }

        private static async Task AddBattlesAsync()
        {
            await _context.Battles.AddRangeAsync(new List<Battle> {
                new Battle
                {
                    Name = "Battle of Shiroyama",
                    StartDate = new DateTime(1877, 9, 24),
                    EndDate = new DateTime(1877, 9, 24)
                },
                new Battle
                {
                    Name = "Siege of Osaka",
                    StartDate = new DateTime(1614, 1, 1),
                    EndDate = new DateTime(1615, 12, 31)
                },
                new Battle
                {
                    Name = "Boshin War",
                    StartDate = new DateTime(1868, 1, 1),
                    EndDate = new DateTime(1869, 1, 1)
                }
            });
            await _context.SaveChangesAsync();
        }

        private static async Task AddManyToManyWithFksAsync()
        {
            // Add samurai and battles for subsequence processing
            await InsertNewPkFkGraphAsync();
            await AddBattlesAsync();

            const int targetSamuraiId = 1;
            const int targetBattleId = 1;

            // Best way to check if object exists in Entity Framework?
            // https://stackoverflow.com/questions/1802286/best-way-to-check-if-object-exists-in-entity-framework
            var existanceOfSamurai = await _context.Samurais.AnyAsync(x => x.Id == targetBattleId);
            var existanceOfBattle = await _context.Battles.AnyAsync(x => x.Id == targetBattleId);
            if (existanceOfSamurai && existanceOfBattle)
            {
                var sb = new SamuraiBattle { SamuraiId = targetSamuraiId, BattleId = targetBattleId };
                var existanceOfSamuraiBattle = await _context.SamuraiBattles.AnyAsync(
                    x => (x.SamuraiId == targetSamuraiId && x.BattleId == targetBattleId));

                if (existanceOfSamuraiBattle == false)
                {
                    await _context.SamuraiBattles.AddAsync(sb);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private static async Task AddManyToManyWithObjectsAsync()
        {
            // Add samurai and battles for subsequence processing
            await InsertNewPkFkGraphAsync();
            await AddBattlesAsync();

            // Eager loading - Loading Related Data
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data#eager-loading
            var samurai = await _context.Samurais.Include(s => s.SamuraiBattles).FirstOrDefaultAsync();
            var battle = await _context.Battles.FirstOrDefaultAsync();
            if (samurai != null && battle != null)
            {
                if (await _context.SamuraiBattles.AnyAsync(
                    x => x.Samurai.Equals(samurai) && x.Battle.Equals(battle)) == false)
                {
                    samurai.SamuraiBattles.Add(
                        new SamuraiBattle { Samurai = samurai, Battle = battle });
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
