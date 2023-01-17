using System.ComponentModel.DataAnnotations;

namespace VerifyEmailForgetPassword.Models
{
    public class UserRegisterRequest
    {


        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(6,ErrorMessage ="Please Enter Correct passord PRo!")]
        public string Password { get; set; } = string.Empty;

        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;


    }
}
