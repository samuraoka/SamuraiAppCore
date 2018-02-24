using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                //await ExplicitLoadWithChildFilter();
                //await UsingRelatedDataForFiltersAndMore();
                //AddGraphAllNew();
                //AddGraphWithKeyValues();
                //AttachGraphAllNew();
                //AttachGraphWithKeyValues();
                //UpdateGraphAllNew();
                //UpdateGraphWithKeyValues();
                //DeleteGraphAllNew();
                //DeleteGraphWithKeyValues();
                //AddGraphViaEntryAllNew();
                //AddGraphViaEntryWithKeyValues();
                //AttachGraphViaEntryAllNew();
                //AttachGraphViaEntryWithKeyValues();
                //UpdateGraphViaEntryAllNew();
                //UpdateGraphViaEntryWithKeyValues();
                //DeleteGraphViaEntryAllNew();
                //DeleteGraphViaEntryWithKeyValues();
                //ChangeStateUsingEntry();
                //AddGraphViaTrackGraphAllNew();
                //AddGraphViaTrackGraphWithKeyValues();
                //AttachGraphViaTrackGraphAllNew();
                //AttachGraphViaTrackGraphWithKeyValues();
                //UpdateGraphViaTrackGraphAllNew();
                //UpdateGraphViaTrackGraphWithKeyValues();
                //DeleteGraphViaTrackGraphAllNew();
                //DeleteGraphViaTrackGraphWithKeyValues();
                StartTrackingUsingCustomFunction();
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

        private static async Task UsingRelatedDataForFiltersAndMore()
        {
            // Insert samurai and quotes for subsequent processing
            await InsertNewPkFkGraphAsync();
            await AddChildToExistingObjectAsync();

            var samurais = await Context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToListAsync();
        }

        public static void DisplayState(List<EntityEntry> es, string method)
        {
            Console.WriteLine(method);
            es.ForEach(e => Console.WriteLine(
                $"{e.Entity.GetType().Name} : {e.State.ToString()}"));
            Console.WriteLine();
        }

        public static void AddGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphAllNew");
            }
        }

        public static void AddGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphWithKeyValues");
            }
        }

        public static void AttachGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Attach(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphAllNew");
            }
        }

        public static void AttachGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Attach(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphWithKeyValues");
            }
        }

        private static void UpdateGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Update(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphAllNew");
            }
        }

        private static void UpdateGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Update(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphWithKeyValues");
            }
        }

        private static void DeleteGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var context = new SamuraiContext())
            {
                try
                {
                    context.Samurais.Remove(samuraiGraph);
                    var es = context.ChangeTracker.Entries().ToList();
                    DisplayState(es, "DeleteGraphAllNew");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine();
                }
            }
        }

        private static void DeleteGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var context = new SamuraiContext())
            {
                context.Samurais.Remove(samuraiGraph);
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "DeleteGraphWithKeyValues");
            }
        }

        private static void AddGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Added;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphViaEntryAllNew");
            }
        }

        private static void AddGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Added;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphViaEntryWithKeyValues");
            }
        }

        private static void AttachGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Unchanged;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphViaEntryAllNew");
            }
        }

        private static void AttachGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Unchanged;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphViaEntryWithKeyValues");
            }
        }

        private static void UpdateGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Modified;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphViaEntryAllNew");
            }
        }

        private static void UpdateGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Modified;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphViaEntryWithKeyValues");
            }
        }

        private static void DeleteGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Deleted;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "DeleteGraphViaEntryAllNew");
            }
        }

        private static void DeleteGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samuraiGraph).State = EntityState.Deleted;
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "DeleteGraphViaEntryWithKeyValues");
            }
        }

        public static void ChangeStateUsingEntry()
        {
            var samurai = new Samurai { Name = "She Who Changes State", Id = 1 };
            using (var ctx = new SamuraiContext())
            {
                ctx.Entry(samurai).State = EntityState.Modified;
                Console.WriteLine("Change State Using Entry");
                DisplayState(ctx.ChangeTracker.Entries().ToList(), "Initial State");

                ctx.Entry(samurai).State = EntityState.Added;
                DisplayState(ctx.ChangeTracker.Entries().ToList(), "New State");
                ctx.SaveChanges();
            }
        }

        private static void AddGraphViaTrackGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Added);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphViaTrackGraphAllNew");
            }
        }

        private static void AddGraphViaTrackGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Added);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AddGraphViaTrackGraphWithKeyValues");
            }
        }

        private static void AttachGraphViaTrackGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Unchanged);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphViaTrackGraphAllNew");
            }
        }

        private static void AttachGraphViaTrackGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Unchanged);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "AttachGraphViaTrackGraphWithKeyValues");
            }
        }

        private static void UpdateGraphViaTrackGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Modified);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphViaTrackGraphAllNew");
            }
        }

        private static void UpdateGraphViaTrackGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Modified);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "UpdateGraphViaTrackGraphWithKeyValues");
            }
        }

        private static void DeleteGraphViaTrackGraphAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Deleted);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "DeleteGraphViaTrackGraphAllNew");
            }
        }

        private static void DeleteGraphViaTrackGraphWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext())
            {
                ctx.ChangeTracker.TrackGraph(samuraiGraph, e => e.Entry.State = EntityState.Deleted);
                var es = ctx.ChangeTracker.Entries().ToList();
                DisplayState(es, "DeleteGraphViaTrackGraphWithKeyValues");
            }
        }

        private static void StartTrackingUsingCustomFunction()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var context = new SamuraiContext())
            {
                context.ChangeTracker.TrackGraph(samuraiGraph, node => ApplyStateUsingIsKeySet(node.Entry));
                var es = context.ChangeTracker.Entries().ToList();
                DisplayState(es, "StartTrackingUsingCustomFunction");
            }
        }

        public static void ApplyStateUsingIsKeySet(EntityEntry entry)
        {
            if (entry.IsKeySet)
            {
                entry.State = EntityState.Unchanged;
            }
            else
            {
                entry.State = EntityState.Added;
            }
        }
    }
}
