namespace ContactList.Web.Models
{
    // Models/SharedSession.cs
    public class SharedSession
    {
        public string SessionId { get; set; }
        public int? UserId { get; set; }
        public string AspNetData { get; set; }
        public string ClassicAspData { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
