using System;
using System.Linq;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;

namespace SamuraiAppCore.CoreUI
{
    class Program
    {
        static void Main(string[] args)
        {
            InsertSamurai();
            InsertMultipleSamurais();
            SimpleSamuraiQuery();
        }

        private static void SimpleSamuraiQuery()
        {
            using (var context = new SamuraiContext())
            {
                var samurais = context.Samurais.Where(s => s.Name == "Sampson").ToList();
                samurais.ForEach(samurai => Console.WriteLine(samurai));

                var julies = context.Samurais.Where(s => s.Name == "Julie").ToList();
                julies.ForEach(julie => Console.WriteLine(julie));
            }
        }

        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Julie" };
            var samuraiSammy = new Samurai { Name = "Sampson" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.AddRange(samurai, samuraiSammy);
                context.SaveChanges();
            }
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }
    }
}
