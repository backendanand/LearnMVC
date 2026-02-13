using System.ComponentModel.DataAnnotations;

namespace LearnMVC.Models.ViewModels
{
    public class StudentListVM
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int age { get; set; }
        public string course { get; set; }
        public bool is_active { get; set; }
    }

    public class StudentCreateVM
    {
        public long Id { get; set; } = 0;

        [Required]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Phone]
        public string Phone { get; set; }
        [Range(1, 100)]
        public int Age { get; set; }
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public List<string> Hobbies { get; set; } = new();
        public string Course { get; set; }
        public List<string> Skills { get; set; } = new();
        public string Address { get; set; }
        public IFormFile ProfileImage { get; set; }
    }

}
