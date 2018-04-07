namespace SamuraiAppCore.Domain
{
    public class SecretIdentity : ClientChangeTracker
    {
        public int Id { get; set; }
        public string RealName { get; set; }
        public Samurai Samurai { get; set; }
        public int SamuraiId { get; set; }
    }
}
