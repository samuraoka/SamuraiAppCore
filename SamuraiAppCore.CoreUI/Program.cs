using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamuraiAppCore.CoreUI
{
    public class Program
    {
        public static SamuraiContext Context { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("Program has started.");
            DoAction().Wait();
            Console.WriteLine("Program is exiting.");
        }

        private static async Task DoAction()
        {
            using (Context = new SamuraiContext())
            {
                await Context.Database.EnsureDeletedAsync();
                await Context.Database.MigrateAsync();

                //await InsertNewPkFkGraphAsync();
                //await InsertNewOneToOneGraphAsync();
                //await AddChildToExistingObjectAsync();
                //await AddOneToOneToExistingObjectWhileTrackedAsync();
                //await ReplaceOneToOneToExistingObjectWhileTrackedAsync();
                //await AddManyToManyWithFksAsync();
                await AddManyToManyWithObjectsAsync();
            }
            Context = null;
        }

        public static async Task InsertNewPkFkGraphAsync()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" },
                    new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" },
                },
            };
            await Context.Samurais.AddAsync(samurai);
            await Context.SaveChangesAsync();
        }

        public static async Task InsertNewOneToOneGraphAsync()
        {
            var samurai = new Samurai { Name = "Shichiroji" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            await Context.AddAsync(samurai);
            await Context.SaveChangesAsync();
        }

        public static async Task AddChildToExistingObjectAsync()
        {
            await InsertNewOneToOneGraphAsync();

            var samurai = await Context.Samurais.FirstAsync();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!",
            });
            await Context.SaveChangesAsync();
        }

        public static async Task AddOneToOneToExistingObjectWhileTrackedAsync()
        {
            await InsertNewPkFkGraphAsync();

            var samurai = await Context.Samurais.FirstOrDefaultAsync(s => s.SecretIdentity == null);
            if (samurai != null)
            {
                samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
                await Context.SaveChangesAsync();
            }
        }

        public static async Task ReplaceOneToOneToExistingObjectWhileTrackedAsync()
        {
            await InsertNewOneToOneGraphAsync();

            var samurai = await Context.Samurais.FirstOrDefaultAsync(
                s => s.SecretIdentity.RealName != "Baba");
            if (samurai != null)
            {
                // Explicit loading - Loading Related Data
                // https://docs.microsoft.com/en-us/ef/core/querying/related-data#explicit-loading
                await Context.Entry(samurai).Reference(s => s.SecretIdentity).LoadAsync();
                samurai.SecretIdentity = new SecretIdentity { RealName = "Baba" };
                await Context.SaveChangesAsync();
            }
        }

        public static async Task AddBattlesAsync()
        {
            await Context.Battles.AddRangeAsync(new List<Battle> {
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
            await Context.SaveChangesAsync();
        }

        public static async Task AddManyToManyWithFksAsync()
        {
            // Add samurai and battles for subsequence processing
            await InsertNewPkFkGraphAsync();
            await AddBattlesAsync();

            // Best way to check if object exists in Entity Framework?
            // https://stackoverflow.com/questions/1802286/best-way-to-check-if-object-exists-in-entity-framework
            var samurai = await Context.Samurais.FirstOrDefaultAsync();
            var battle = await Context.Battles.FirstOrDefaultAsync();
            if (samurai != null && battle != null)
            {
                var sb = new SamuraiBattle { SamuraiId = samurai.Id, BattleId = battle.Id };
                var existanceOfSamuraiBattle = await Context.SamuraiBattles.AnyAsync(
                    x => (x.SamuraiId == samurai.Id && x.BattleId == battle.Id));
                if (existanceOfSamuraiBattle == false)
                {
                    await Context.SamuraiBattles.AddAsync(sb);
                    await Context.SaveChangesAsync();
                }
            }
        }

        public static async Task AddManyToManyWithObjectsAsync()
        {
            // Add samurai and battles for subsequence processing
            await InsertNewPkFkGraphAsync();
            await AddBattlesAsync();

            // Eager loading - Loading Related Data
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data#eager-loading
            var samurai = await Context.Samurais.Include(s => s.SamuraiBattles).FirstOrDefaultAsync();
            var battle = await Context.Battles.FirstOrDefaultAsync();
            if (samurai != null && battle != null)
            {
                if (await Context.SamuraiBattles.AnyAsync(
                    x => x.Samurai.Equals(samurai) && x.Battle.Equals(battle)) == false)
                {
                    samurai.SamuraiBattles.Add(
                        new SamuraiBattle { Samurai = samurai, Battle = battle });
                    await Context.SaveChangesAsync();
                }
            }
        }
    }
}
