namespace LearnMVC.Models.ServiceModels
{
    public class AuthSession
    {
        public Guid SessionId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
