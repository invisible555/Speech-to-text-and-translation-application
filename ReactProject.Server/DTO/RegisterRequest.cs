using System.ComponentModel.DataAnnotations;

namespace ReactProject.Server.DTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Login jest wymagany.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Login może zawierać tylko litery, cyfry, myślnik i podkreślenie.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Hasło musi zawierać co najmniej jedną wielką literę i jedną cyfrę.")]
        public string Password { get; set; }
    }
}
