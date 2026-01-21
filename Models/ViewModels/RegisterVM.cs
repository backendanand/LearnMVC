using System.ComponentModel.DataAnnotations;

namespace LearnMVC.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "Name cannot exceed 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,12}$",
            ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }
    }
}
