using System.ComponentModel.DataAnnotations;

namespace LearnMVC.Models.ViewModels
{
    public class LoginVM
    {
        [Required (ErrorMessage = "Username is required.")]
        [MaxLength (20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string Username { get; set; }

        [Required (ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,12}$",
            ErrorMessage = "Invalid password.")]
        public string Password { get; set; }
    }
}
