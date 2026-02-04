namespace LearnMVC.Models.DomainModels
{
    public class Student : BaseEntity
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public int age { get; set; }
        public DateTime date_of_birth { get; set; }
        public string gender { get; set; }
        public bool is_active { get; set; }
        public List<string> hobbies { get; set; }
        public string course { get; set; }
        public List<string> skills { get; set; }
        public string address { get; set; }
        public IFormFile profile_image { get; set; }
    }

}
