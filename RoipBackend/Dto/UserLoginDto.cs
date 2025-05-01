using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Dto
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Valid email is mandatory.")]
        [EmailAddress]
        public string? Email { get; private set; }

        [Required]
        [StringLength(Consts.MAXIMUM_PASSWORD_LENGTH, MinimumLength = Consts.MINIMUM_PASSWORD_LENGTH)]
        public string? Password { get; private set; }

        public readonly int Role { get; private set; }
    }      
}
