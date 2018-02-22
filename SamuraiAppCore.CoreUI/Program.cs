using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System.Collections.Generic;

namespace SamuraiAppCore.CoreUI
{
    internal class Program
    {
        private static SamuraiContext _context;

        private static void Main(string[] args)
        {
            using (_context = new SamuraiContext())
            {
                _context.Database.Migrate();

                InsertNewPkFkGraph();
            }
        }

        private static void InsertNewPkFkGraph()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" },
                },
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
    }
}
