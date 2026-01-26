namespace LearnMVC.Models
{
    public class User : BaseEntity
    {
        public string name { get; set; }
        public string username { get; set; }
        public string mobile { get; set; }
        public string role { get; set; }
        public string password { get; set; }
    }
}
