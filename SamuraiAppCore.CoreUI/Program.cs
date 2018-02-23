using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //await AddManyToManyWithObjectsAsync();
                //await EagerLoadWithIncludeAsync();
                //await EagerLoadManyToManyAkaChildrenGrandchildren();
                //await EagerLoadWithMultipleBranches();
                //await AnonymousTypeViaProjectionAsync();
                //await AnonymousTypeViaProjectionWithRelatedAsync();
                //await RelatedObjectsFixupAsync();
                //await EagerLoadViaProjectionNotQuiteAsync();
                //await FilteredEagerLoadViaProjectionNopeAsync();
                //await ExplicitLoadAsync();
                await ExplicitLoadWithChildFilter();
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

        private static async Task EagerLoadWithIncludeAsync()
        {
            // Add samurai and quotes for subsequent processing
            await InsertNewPkFkGraphAsync();

            var samuraiWithQuotes = await Context.Samurais.Include(s => s.Quotes).ToListAsync();
        }

        private static async Task EagerLoadManyToManyAkaChildrenGrandchildren()
        {
            // Add samurai and battle for subsequent processing
            await AddManyToManyWithObjectsAsync();

            var samuraiWithBattles = await Context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle).ToListAsync();
        }

        private static async Task EagerLoadWithMultipleBranches()
        {
            // Insert samurai, quotes and secretIdentities for subsequent processing
            await AddOneToOneToExistingObjectWhileTrackedAsync();

            var samurais = await Context.Samurais
                .Include(s => s.SecretIdentity)
                .Include(s => s.Quotes)
                .ToListAsync();
        }

        private static async Task AnonymousTypeViaProjectionAsync()
        {
            // Insert quotes for subsequent processing
            await AddOneToOneToExistingObjectWhileTrackedAsync();

            var quotes = Context.Quotes
                .Select(q => new { q.Id, q.Text })
                .ToList();
        }

        private static async Task AnonymousTypeViaProjectionWithRelatedAsync()
        {
            // Insert quotes for subsequent processing
            await AddOneToOneToExistingObjectWhileTrackedAsync();

            var samurais = Context.Samurais
                .Select(s => new
                {
                    s.Id,
                    s.SecretIdentity.RealName,
                    QuoteCount = s.Quotes.Count
                }).ToList();
        }

        private static async Task RelatedObjectsFixupAsync()
        {
            // Insert quotes for subsequent processing
            await AddOneToOneToExistingObjectWhileTrackedAsync();

            var samurai = Context.Samurais.First();
            var quotes = Context.Quotes.Where(q => q.Samurai == samurai).ToList();
        }

        private static async Task EagerLoadViaProjectionNotQuiteAsync()
        {
            // Insert quotes for subsequent processing
            await AddOneToOneToExistingObjectWhileTrackedAsync();

            var samurais = await Context.Samurais
                .Select(s => new { Samurai = s, Quotes = s.Quotes })
                .ToListAsync();
        }

        private static async Task FilteredEagerLoadViaProjectionNopeAsync()
        {
            // Insert samurai and quotes for subsequent processing
            await AddChildToExistingObjectAsync();

            var samurais = Context.Samurais.Select(s => new
            {
                samurai = s,
                Quotes = s.Quotes.Where(q => q.Text.Contains("happy")).ToList()
            }).ToList();
        }

        private static async Task ExplicitLoadAsync()
        {
            // Insert samurai and quotes for subsequent processing
            await AddChildToExistingObjectAsync();

            using (var ctx = new SamuraiContext())
            {
                var samurai = await ctx.Samurais.FirstAsync();
                await ctx.Entry(samurai).Collection(s => s.Quotes).LoadAsync();
                await ctx.Entry(samurai).Reference(s => s.SecretIdentity).LoadAsync();
            }
        }

        private static async Task ExplicitLoadWithChildFilter()
        {
            // Insert samurai and quotes for subsequent processing
            await AddChildToExistingObjectAsync();

            using (var ctx = new SamuraiContext())
            {
                var samurai = await ctx.Samurais.FirstAsync();
                await ctx.Entry(samurai)
                    .Collection(s => s.Quotes)
                    .Query()
                    .Where(q => q.Text.Contains("happy"))
                    .LoadAsync();
            }

        }
    }
}
