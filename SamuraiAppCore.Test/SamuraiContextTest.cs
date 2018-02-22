using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                InsertNewPkFkGraphAsync(ctx).Wait();
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
                InsertNewPkFkGraphAsync(ctx).Wait();
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
                InsertNewPkFkGraphAsync(ctx).Wait();
            }

            using (var ctx = new SamuraiContext())
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

        private static async Task InsertNewPkFkGraphAsync(SamuraiContext context)
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" },
                    new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" },
                },
            };
            await context.Samurais.AddAsync(samurai);
            await context.SaveChangesAsync();
        }

    }
}
