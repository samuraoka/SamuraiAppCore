using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.CoreUI;
using SamuraiAppCore.Data;
using Xunit;

namespace SamuraiAppCore.Test
{
    public class SamuraiContextTest
    {
        // Comparing xUnit.net to other frameworks
        // https://xunit.github.io/docs/comparisons
        public SamuraiContextTest()
        {
            using (var ctx = new SamuraiContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.Migrate();
            }
        }

        [Fact]
        public void ShouldInsertNewPkFkGraphSamurai()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                const string expectedSamuraiName = "Kambei Shimada";
                const int expectedQuoteCount = 2;

                var samurai = ctx.Samurais.Include(s => s.Quotes).FirstAsync(
                    s => s.Name == expectedSamuraiName).GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiName, samurai.Name);
                Assert.Equal(expectedQuoteCount, samurai.Quotes.Count);
            }
        }

        [Fact]
        public void ShouldInsertNewPkFkGraphQUoteCount()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
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
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.InsertNewPkFkGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedQuote1 = "I've come to save you";
                var expectedQuote2 = "I told you to watch out for the sharp sword! Oh well!";
                var expectedSamuraiId = 1;

                var quote1 = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote1).GetAwaiter().GetResult();
                var quote2 = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote2).GetAwaiter().GetResult();

                Assert.Equal(expectedQuote1, quote1.Text);
                Assert.Equal(expectedQuote2, quote2.Text);

                Assert.Equal(expectedSamuraiId, quote1.SamuraiId);
                Assert.Equal(expectedSamuraiId, quote2.SamuraiId);
            }
        }

        [Fact]
        public void ShouldInsertNewOneToOneGraphSamurai()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.InsertNewOneToOneGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedSamuraiName = "Shichiroji";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == expectedSamuraiName).GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiName, samurai.Name);
                Assert.NotNull(samurai.SecretIdentity);
            }
        }

        [Fact]
        public void ShouldInsertNewOneToOneGraphSecretIdentity()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.InsertNewOneToOneGraphAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
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
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
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
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.AddChildToExistingObjectAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedQuote = "I bet you're happy that I've saved you!";
                var expectedSamuraiId = 1;

                var quote = ctx.Quotes.FirstAsync(
                    q => q.Text == expectedQuote).GetAwaiter().GetResult();

                Assert.Equal(expectedQuote, quote.Text);
                Assert.Equal(expectedSamuraiId, quote.SamuraiId);
            }
        }

        [Fact]
        public void ShouldAddOneToOneToExistingObjectWhileTraked()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.NotNull(samurai.SecretIdentity);
            }
        }

        [Fact]
        public void ShouldAddOneToOneToExistingObjectWhileTrakedRealName()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedRealName = "Sampson";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Equal(expectedRealName, samurai.SecretIdentity.RealName);
            }
        }

        [Fact]
        public void ShouldAddOneToOneToExistingObjectWhileTrakedRelation()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.AddOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedSamuraiId = 1;

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).SingleAsync(
                    s => s.Name == "Kambei Shimada").GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiId, samurai.SecretIdentity.SamuraiId);
            }
        }

        [Fact]
        public void ShouldReplaceOneToOneToExistingObjectWhileTracked()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.ReplaceOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.NotNull(samurai.SecretIdentity);
            }
        }

        [Fact]
        public void ShouldReplaceOneToOneToExistingObjectWhileTrackedRealName()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.ReplaceOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedRealName = "Baba";

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.Equal(expectedRealName, samurai.SecretIdentity.RealName);
            }
        }

        [Fact]
        public void ShouldReplaceOneToOneToExistingObjectWhileTrackedRelation()
        {
            using (var ctx = new SamuraiContext())
            {
                Program.Context = ctx;
                Program.ReplaceOneToOneToExistingObjectWhileTrackedAsync().Wait();
            }

            using (var ctx = new SamuraiContext())
            {
                var expectedSamuraiId = 1;

                var samurai = ctx.Samurais.Include(s => s.SecretIdentity).FirstAsync(
                    s => s.Name == "Shichiroji").GetAwaiter().GetResult();

                Assert.Equal(expectedSamuraiId, samurai.SecretIdentity.SamuraiId);
            }
        }
    }
}
