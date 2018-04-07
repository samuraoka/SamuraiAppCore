using System.Collections.Generic;

namespace SamuraiAppCore.Domain
{
    public class Samurai : ClientChangeTracker
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
        public List<SamuraiBattle> SamuraiBattles { get; set; }
        public SecretIdentity SecretIdentity { get; set; }

        public override string ToString()
        {
            return string.Format($"Id: {Id}, Name: {Name}");
        }
    }
}
