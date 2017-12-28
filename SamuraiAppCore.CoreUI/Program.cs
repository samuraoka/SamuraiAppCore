using System;
using System.Linq;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;

namespace SamuraiAppCore.CoreUI
{
    class Program
    {
        private static SamuraiContext _context;

        static void Main(string[] args)
        {
            using (_context = new SamuraiContext())
            {
                //InsertSamurai(); // Uncomment this line if you want to insert a data.
                //InsertMultipleSamurais(); // Uncomment this line if you want to insert some data.
                //SimpleSamuraiQuery();
                //MoreQueries();
                //MoreQueriesFirst();
                //MoreQueriesById();
                //RetrieveAndUpdateSamurai();
                //RetrieveAndUpdateMultipleSamurais();
                MultipleOperations();
            }
        }

        private static void MultipleOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            if (samurai != null)
            {
                samurai.Name += "San";
                _context.SaveChanges();
            }
        }

        private static void MoreQueriesById()
        {
            var samurai = _context.Samurais.Find(60);
            if (samurai != null)
            {
                Console.WriteLine(samurai);
            }
        }

        private static void MoreQueriesFirst()
        {
            var names = new[] { "Sampson", "Cheese" };
            foreach (var name in names)
            {
                try
                {
                    var samurai = _context.Samurais.First(s => s.Name == name);
                    Console.WriteLine(samurai);
                }
                catch (Exception)
                {
                    Console.WriteLine($"There is no such Samurai named \"{name}\".");
                }
            }
        }

        private static void MoreQueries()
        {
            var name = "Sampson";
            var samurais = _context.Samurais.Where(s => s.Name == name).ToList();
            samurais.ForEach(samurai => Console.WriteLine(samurai));
        }

        private static void SimpleSamuraiQuery()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(samurai => Console.WriteLine(samurai));
        }

        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Julie" };
            var samuraiSammy = new Samurai { Name = "Sampson" };
            _context.Samurais.AddRange(samurai, samuraiSammy);
            _context.SaveChanges();
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
    }
}
