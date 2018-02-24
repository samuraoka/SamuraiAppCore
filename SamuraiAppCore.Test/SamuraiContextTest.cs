using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.CoreUI;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Linq;
using Xunit;

namespace SamuraiAppCore.Test
{
    // Microsoft.EntityFrameworkCore.InMemory
    // Install-Package -Id Microsoft.EntityFrameworkCore.InMemory -ProjectName SamuraiAppCore.Data
    // https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/
    //
    // Writing tests
    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory#writing-tests
    //
    // Comparing xUnit.net to other frameworks
    // https://xunit.github.io/docs/comparisons
    public class SamuraiContextTest
    {
        private DbContextOptions<SamuraiContext> options;

        public SamuraiContextTest()
        {
            options = new DbContextOptionsBuilder<SamuraiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = new SamuraiContext(options))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }

        [Fact]
        public void ShouldInsertNewPkFkGraphSamurai()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                const string expectedSamuraiName = "Kambei Shimada";
                const int expectedQuoteCount = 2;

                var samurai = ctx.Samurais.Include(s => s.Quotes).FirstAsync(
                    s => s.Name == expectedSamuraiName).GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiName, samurai.Name);
                Assert.Equal(expectedQuoteCount, samurai.Quotes.Count);
                Assert.True(samurai.Quotes.All(q => q.SamuraiId == samurai.Id));
            }
        }

        [Fact]
        public void ShouldInsertNewPkFkGraphQUoteCount()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                const int expectedQuoteCount = 2;

                var samurai = ctx.Samurais.Include(s => s.Quotes).FirstAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Equal(expectedQuoteCount, samurai.Quotes.Count);
            }
        }

        [Fact]
        public void ShouldInsertNewPkFkGraphQuote()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedQuote1 = "I've come to save you";
                var expectedQuote2 = "I told you to watch out for the sharp sword! Oh well!";

                var quote1 = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote1).GetAwaiter().GetResult();
                var quote2 = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote2).GetAwaiter().GetResult();

                Assert.Equal(expectedQuote1, quote1.Text);
                Assert.Equal(expectedQuote2, quote2.Text);
            }
        }

        [Fact]
        public void ShouldInsertNewOneToOneGraphSamurai()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewOneToOneGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedSamuraiName = "Shichiroji";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == expectedSamuraiName).GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiName, samurai.Name);
                Assert.NotNull(samurai.SecretIdentity);
                Assert.Equal(samurai.Id, samurai.SecretIdentity.SamuraiId);
            }
        }

        [Fact]
        public void ShouldInsertNewOneToOneGraphSecretIdentity()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewOneToOneGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedRealName = "Julie";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.Equal(expectedRealName, samurai.SecretIdentity.RealName);
            }
        }

        [Fact]
        public void ShouldAddChildToExistingObjectQuoteCount()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.Include(s => s.Quotes).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                // Do not use equality check to check for collection size.
                // https://xunit.github.io/xunit.analyzers/rules/xUnit2013
                Assert.Single(samurai.Quotes);
            }
        }

        [Fact]
        public void ShouldAddChildToExistingObjectQuote()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedQuote = "I bet you're happy that I've saved you!";

                var quote = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote).GetAwaiter().GetResult();

                Assert.Equal(expectedQuote, quote.Text);
            }
        }

        [Fact]
        public void ShouldAddOneToOneToExistingObjectWhileTraked()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.NotNull(samurai.SecretIdentity);
                Assert.Equal(samurai.Id, samurai.SecretIdentity.SamuraiId);
            }
        }

        [Fact]
        public void ShouldAddOneToOneToExistingObjectWhileTrakedRealName()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedRealName = "Sampson";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Equal(expectedRealName, samurai.SecretIdentity.RealName);
            }
        }

        [Fact]
        public void ShouldReplaceOneToOneToExistingObjectWhileTracked()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.ReplaceOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.NotNull(samurai.SecretIdentity);
                Assert.Equal(samurai.Id, samurai.SecretIdentity.SamuraiId);
            }
        }

        [Fact]
        public void ShouldReplaceOneToOneToExistingObjectWhileTrackedRealName()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.ReplaceOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedRealName = "Baba";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.Equal(expectedRealName, samurai.SecretIdentity.RealName);
            }
        }

        [Fact]
        public void ShouldAddBattles()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddBattlesAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedBattleCount = 3;

                var battles = ctx.Battles.ToListAsync().GetAwaiter().GetResult();
                Assert.Equal(expectedBattleCount, battles.Count);
            }
        }

        [Fact]
        public void ShouldAddBattlesBattle1()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddBattlesAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedBattleName = "Battle of Shiroyama";
                var expectedStartDate = new DateTime(1877, 9, 24);
                var expectedEndDate = new DateTime(1877, 9, 24);

                var battle = ctx.Battles.SingleAsync(
                    b => b.Name == expectedBattleName).GetAwaiter().GetResult();

                Assert.Equal(expectedBattleName, battle.Name);
                Assert.Equal(expectedStartDate, battle.StartDate);
                Assert.Equal(expectedEndDate, battle.EndDate);
            }
        }

        [Fact]
        public void ShouldAddBattlesBattle2()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddBattlesAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedBattleName = "Siege of Osaka";
                var expectedStartDate = new DateTime(1614, 1, 1);
                var expectedEndDate = new DateTime(1615, 12, 31);

                var battle = ctx.Battles.SingleAsync(
                    b => b.Name == expectedBattleName).GetAwaiter().GetResult();

                Assert.Equal(expectedBattleName, battle.Name);
                Assert.Equal(expectedStartDate, battle.StartDate);
                Assert.Equal(expectedEndDate, battle.EndDate);
            }
        }

        [Fact]
        public void ShouldAddBattlesBattle3()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddBattlesAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var expectedBattleName = "Boshin War";
                var expectedStartDate = new DateTime(1868, 1, 1);
                var expectedEndDate = new DateTime(1869, 1, 1);

                var battle = ctx.Battles.SingleAsync(
                    b => b.Name == expectedBattleName).GetAwaiter().GetResult();

                Assert.Equal(expectedBattleName, battle.Name);
                Assert.Equal(expectedStartDate, battle.StartDate);
                Assert.Equal(expectedEndDate, battle.EndDate);
            }
        }

        [Fact]
        public void ShouldAddManyToManyWithFks()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddManyToManyWithFksAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.Include(s => s.SamuraiBattles).FirstAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Single(samurai.SamuraiBattles);
            }
        }

        [Fact]
        public void ShouldAddManyToManyWithFksRelation()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddManyToManyWithFksAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                int expectedSamuraiId = 1;
                int expectedBattleId = 1;

                var samuraiBattles = ctx.SamuraiBattles.FirstAsync().GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiId, samuraiBattles.SamuraiId);
                Assert.Equal(expectedBattleId, samuraiBattles.BattleId);
            }
        }

        [Fact]
        public void ShouldAddManyToManyWithObjects()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddManyToManyWithObjectsAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.Include(s => s.SamuraiBattles).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Single(samurai.SamuraiBattles);
            }
        }

        [Fact]
        public void CouldEagerLoadWithInclude()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samuraiWithQuotes = ctx.Samurais.Include(
                    s => s.Quotes).ToListAsync().GetAwaiter().GetResult();
                Assert.True(samuraiWithQuotes.All(
                    s => s.Quotes != null && s.Quotes.Count > 0));
            }
        }

        [Fact]
        public void ChouldEagerLoadManyToManyAkaChildrenGrandchildren()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddManyToManyWithObjectsAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samuraiWithBattles = ctx.Samurais
                    .Include(s => s.SamuraiBattles)
                    .ThenInclude(sb => sb.Battle)
                    .ToListAsync().GetAwaiter().GetResult();
                Assert.True(samuraiWithBattles.All(s => s.SamuraiBattles != null));
                Assert.True(samuraiWithBattles.All(
                    s => s.SamuraiBattles.All(sb => sb.Battle != null)));
            }
        }

        [Fact]
        public void CouldEagerLoadWithMultipleBranchesQuote()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Include(s => s.Quotes)
                    .Include(s => s.SecretIdentity)
                    .ToListAsync().GetAwaiter().GetResult();
                Assert.True(samurais.All(s => s.Quotes != null));
            }
        }

        [Fact]
        public void CouldEagerLoadWithMultipleBranchesSecretIdentity()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Include(s => s.Quotes)
                    .Include(s => s.SecretIdentity)
                    .ToListAsync().GetAwaiter().GetResult();
                Assert.True(samurais.All(s => s.SecretIdentity != null));
            }
        }

        [Fact]
        public void CouldGetAnonymousTypeViaProjection()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var quotes = ctx.Quotes.Select(
                    q => new { q.Id, q.Text }).ToList();
                Assert.NotNull(quotes);
                Assert.NotEmpty(quotes);
            }
        }

        [Fact]
        public void CouldGetAnonymousTypeViaProjectionWithRelated()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Select(s => new
                    {
                        s.Id,
                        s.SecretIdentity.RealName,
                        QuoteCount = s.Quotes.Count
                    }).ToList();
                Assert.Single(samurais);
            }
        }

        [Fact]
        public void CouldGetAnonymousTypeViaProjectionWithRelatedFirstItem()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Select(s => new
                    {
                        s.Id,
                        s.SecretIdentity.RealName,
                        QuoteCount = s.Quotes.Count
                    }).ToList();
                var samurai = samurais.First();
                Assert.Equal(2, samurai.QuoteCount);
                Assert.Equal("Sampson", samurai.RealName);
            }
        }

        [Fact]
        public void ShouldFixupRelatedObjectsBefore()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.First();
                Assert.Empty(samurai.Quotes);
            }
        }

        [Fact]
        public void ShouldFixupRelatedObjectsAfter()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.First();
                var quotes = ctx.Quotes.Where(q => q.Samurai == samurai).ToList();
                Assert.NotEmpty(samurai.Quotes);
            }
        }

        [Fact]
        public void ShouldEagerLoadViaProjectionNotQuite()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Select(s => new { Samurai = s, Quotes = s.Quotes })
                    .ToList();
                var samurai = samurais.First();
                Assert.NotNull(samurai.Quotes);
                Assert.NotNull(samurai.Samurai.Quotes);
                Assert.NotEmpty(samurai.Quotes);
                Assert.NotEmpty(samurai.Samurai.Quotes);
            }
        }

        [Fact]
        public void ShouldFilteredEagerLoadViaProjectionNope()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurais = ctx.Samurais
                    .Select(s => new
                    {
                        samurai = s,
                        Quotes = s.Quotes.Where(q => q.Text.Contains("happy")).ToList()
                    })
                    .ToList();
                var samurai = samurais.First();
                Assert.NotNull(samurai.Quotes);
                Assert.NotNull(samurai.samurai.Quotes);
                Assert.NotEmpty(samurai.Quotes);
                Assert.Empty(samurai.samurai.Quotes); // Filtered Eager Load relation is not tracked
            }
        }

        [Fact]
        public void CouldExplicitLoadCollection()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.First();
                Assert.Empty(samurai.Quotes);

                ctx.Entry(samurai).Collection(s => s.Quotes).Load();
                Assert.NotEmpty(samurai.Quotes);
            }
        }

        [Fact]
        public void CouldExplicitLoadReference()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.First();
                Assert.Null(samurai.SecretIdentity);

                ctx.Entry(samurai).Reference(s => s.SecretIdentity).Load();
                Assert.NotNull(samurai.SecretIdentity);
            }
        }

        [Fact]
        public void CouldExplicitLoadWithChildFilterCollection()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                var samurai = ctx.Samurais.First();
                Assert.Empty(samurai.Quotes);

                ctx.Entry(samurai)
                    .Collection(s => s.Quotes)
                    .Query()
                    .Where(q => q.Text.Contains("happy"))
                    .Load();

                Assert.Single(samurai.Quotes);
            }
        }

        [Fact]
        public void CouldUsingRelatedDataForFiltersAndMore()
        {
            using (var ctx = new SamuraiContext(options))
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext(options))
            {
                // There is two samurai in the database.
                // Where method filters out one samurai.
                // So the result contains only one samurai.
                var samurais = ctx.Samurais
                    .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                    .ToList();

                Assert.Single(samurais);
            }
        }

        [Fact]
        public void ShouldAddGraphAsAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Add(samuraiGraph);
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Added));
            }
        }

        [Fact]
        public void ShouldAddGraphWithKeyValuesAsAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Add(samuraiGraph);
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Added));
            }
        }

        [Fact]
        public void ShouldAttachGraphAsAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Attach(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries());
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Added));
            }
        }

        [Fact]
        public void ShouldAttachGraphWithKeyValuesAsAllUnchanged()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Attach(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries());
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Unchanged));
            }
        }

        [Fact]
        public void ShouldUpdateGraphAsAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Update(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries());
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Added));
            }
        }

        [Fact]
        public void ShouldUpdateGraphWithKeyValuesAsAllModified()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Update(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries());
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Modified));
            }
        }

        [Fact]
        public void ShouldDeleteGraphAsAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                // This behavior is different from the SQL Server provider
                // In the SQL Server provider exception raises, but this In-Memory Provider doesn't
                ctx.Samurais.Remove(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Deleted));
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Quote>());
                Assert.True(ctx.ChangeTracker.Entries<Quote>().All(e => e.State == EntityState.Added));
            }
        }

        [Fact]
        public void ShouldDeleteGraphWithKeyValuesAsAllModified()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Samurais.Remove(samuraiGraph);
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Deleted));
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Quote>());
                Assert.True(ctx.ChangeTracker.Entries<Quote>().All(e => e.State == EntityState.Unchanged));
            }
        }

        [Fact]
        public void CouldAddGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Added;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Added));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldAddGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Added;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Added));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldAttachGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Unchanged;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Unchanged));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldAttachGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Unchanged;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Unchanged));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldUpdateGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Modified;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Modified));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldUpdateGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Modified;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Modified));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldDeleteGraphViaEntryAllNew()
        {
            var samuraiGraph = new Samurai { Name = "Julie" };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new" });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Deleted;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Deleted));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void CouldDeleteGraphViaEntryWithKeyValues()
        {
            var samuraiGraph = new Samurai { Name = "Julie", Id = 1 };
            samuraiGraph.Quotes.Add(new Quote { Text = "This is new", Id = 1 });
            using (var ctx = new SamuraiContext(options))
            {
                ctx.Entry(samuraiGraph).State = EntityState.Deleted;
                Assert.NotEmpty(ctx.ChangeTracker.Entries<Samurai>());
                Assert.True(ctx.ChangeTracker.Entries<Samurai>().All(e => e.State == EntityState.Deleted));
                Assert.Empty(ctx.ChangeTracker.Entries<Quote>());
            }
        }

        [Fact]
        public void ShouldChangeStateUsingEntry()
        {
            var samurai = new Samurai { Name = "She Who Changes State", Id = 1 };

            using (var ctx = new SamuraiContext(options))
            {
                Assert.Empty(ctx.ChangeTracker.Entries());

                ctx.Entry(samurai).State = EntityState.Modified;
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Modified));

                ctx.Entry(samurai).State = EntityState.Added;
                Assert.True(ctx.ChangeTracker.Entries().All(e => e.State == EntityState.Added));

                // An exception happens because an entity with key value is going to be inserted
                Assert.Throws<DbUpdateException>(() => ctx.SaveChanges());
            }
        }
    }
}
