using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
                //MultipleOperations();
                //QueryAndUpdateSamuraiDisconnected();
                //QueryAndUpdateDisconnectedBattle();
                //AddSomeMoreSamurais();
                //DeleteWhileTracked();
                //DeleteMany();
                //DeleteWhileNotTracked();
                //RawSqlQuery();
                //RawSqlQueryStoredProcedure();
                //QueryWithNonSql();
                //RawSqlCommand();
                RawSqlCommandWithOutput();
            }
        }

        private static void RawSqlCommandWithOutput()
        {
            var procResult = new SqlParameter
            {
                ParameterName = "@procResult",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Output,
                Size = 50
            };
            _context.Database.ExecuteSqlCommand(
                "EXEC FindLongestName @procResult OUT", procResult);
            Console.WriteLine("========================================");
            Console.WriteLine($"Longest name: {procResult.Value}");
            Console.WriteLine("========================================");
        }

        private static void RawSqlCommand()
        {
            var affected = _context.Database.ExecuteSqlCommand(
                "update samurais set Name = REPLACE(Name, 'San', 'Nan')");
            Console.WriteLine("========================================");
            Console.WriteLine($"Affected rows: {affected}");
            Console.WriteLine("========================================");
        }

        private static void QueryWithNonSql()
        {
            var samurais = _context.Samurais
                .Select(s => new { newName = ReverseString(s.Name) })
                .ToList();
            Console.WriteLine("========================================");
            samurais.ForEach(s => Console.WriteLine(s));
            Console.WriteLine("========================================");
        }

        private static string ReverseString(string value)
        {
            var stringChar = value.AsEnumerable();
            return string.Concat(stringChar.Reverse());
        }

        private static void RawSqlQueryStoredProcedure()
        {
            var namePart = "San";
            var samurais = _context.Samurais
                .FromSql($"EXEC FilterSamuraiByNamePart {namePart}")
                .OrderByDescending(s => s.Name).ToList();

            Console.WriteLine("========================================");
            samurais.ForEach(s => Console.WriteLine(s));
            Console.WriteLine("========================================");
        }

        private static void RawSqlQuery()
        {
            var samurais = _context.Samurais.FromSql("Select * From Samurais")
                .Where(s => s.Name.Contains("San"))
                .OrderByDescending(s => s.Name).ToList();
            Console.WriteLine("========================================");
            samurais.ForEach(s => Console.WriteLine(s));
            Console.WriteLine("========================================");
        }

        private static void DeleteWhileNotTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Heihachi Hayashida");
            if (samurai != null)
            {
                using (var contextNewAppInstance = new SamuraiContext())
                {
                    contextNewAppInstance.Samurais.Remove(samurai);
                    contextNewAppInstance.SaveChanges();
                }
            }
        }

        private static void DeleteMany()
        {
            var samurais = _context.Samurais.Where(s => s.Name.Contains("oh"));
            _context.Samurais.RemoveRange(samurais);
            _context.SaveChanges();
        }

        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Shichirohji");
            if (samurai != null)
            {
                //_context.Samurais.Remove(samurai);
                _context.Entry(samurai).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }

        private static void AddSomeMoreSamurais()
        {
            _context.AddRange(
                new Samurai { Name = "Kambei Shimada" },
                new Samurai { Name = "Shichirohji" },
                new Samurai { Name = "Katsushiroh Okamoto" },
                new Samurai { Name = "Heihachi Hayashida" },
                new Samurai { Name = "Kyuhzoh" },
                new Samurai { Name = "Grohbei Katayama" }
            );
            _context.SaveChanges();
        }

        private static void QueryAndUpdateDisconnectedBattle()
        {
            var battle = _context.Battles.FirstOrDefault();
            battle.EndDate = new DateTime(1754, 12, 31);
            using (var contextNewAppInstance = new SamuraiContext())
            {
                contextNewAppInstance.Battles.Update(battle);
                contextNewAppInstance.SaveChanges();
            }
        }

        private static void QueryAndUpdateSamuraiDisconnected()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Kikuchiyo");
            if (samurai != null)
            {
                samurai.Name += "San";
                using (var contextNewAppInstance = new SamuraiContext())
                {
                    contextNewAppInstance.Samurais.Update(samurai);
                    contextNewAppInstance.SaveChanges();
                }
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
