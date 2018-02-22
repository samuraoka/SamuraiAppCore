using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Data;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SamuraiAppCore.CoreUI
{
    internal class Program
    {
        private static SamuraiContext _context;

        // Block main thread to exit
        // https://stackoverflow.com/questions/3840795/console-app-terminating-before-async-call-completion
        private static ManualResetEvent _resetEvent;

        private static void Main(string[] args)
        {
            Console.WriteLine("Program has started.");

            using (_context = new SamuraiContext())
            using (_resetEvent = new ManualResetEvent(false))
            {
                _context.Database.Migrate();

                //InsertNewPkFkGraph();
                //InsertNewOneToOneGraph();
                //AddChildToExistingObject();
                //AddOneToOneToExistingObjectWhileTracked();
                ReplaceOneToOneToExistingObjectWhileTracked();

                _resetEvent.WaitOne();
            }
            _resetEvent = null;
            _context = null;

            Console.WriteLine("Program is exiting.");
        }

        private static void InsertNewPkFkGraph()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" },
                    new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" },
                },
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewOneToOneGraph()
        {
            var samurai = new Samurai { Name = "Shichiroji" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            _context.Add(samurai);
            _context.SaveChanges();
        }

        private static async void AddChildToExistingObject()
        {
            _resetEvent.Reset();

            var samurai = await _context.Samurais.FirstAsync();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!",
            });
            await _context.SaveChangesAsync();

            _resetEvent.Set();
        }

        private static async void AddOneToOneToExistingObjectWhileTracked()
        {
            _resetEvent.Reset();

            var samurai = await _context.Samurais.FirstOrDefaultAsync(s => s.SecretIdentity == null);
            if (samurai != null)
            {
                samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
                await _context.SaveChangesAsync();
            }

            _resetEvent.Set();
        }

        private static async void ReplaceOneToOneToExistingObjectWhileTracked()
        {
            _resetEvent.Reset();

            var samurai = await _context.Samurais.FirstOrDefaultAsync(
                s => s.SecretIdentity.RealName != "Baba");
            if (samurai != null)
            {
                // Loading Related Data
                // https://docs.microsoft.com/en-us/ef/core/querying/related-data
                await _context.Entry(samurai).Reference(s => s.SecretIdentity).LoadAsync();
                samurai.SecretIdentity = new SecretIdentity { RealName = "Baba" };
                await _context.SaveChangesAsync();
            }

            _resetEvent.Set();
        }
    }
}
