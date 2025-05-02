using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoipBackend.Dto
{
    public class UserDto
    {
        [Required]
        [Range(Consts.ID_MINIMUM_RANGE, Consts.ID_MAXIMUM_RANGE)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment
        public int Id { get; set; }

        [Required(ErrorMessage = Consts.USER_NAME_MANDATORY_STR)]
        [StringLength(16, MinimumLength = Consts.MINIMUM_USER_NAME_LENGTH)]
        public string Username { get; set; }

        [Required(ErrorMessage = Consts.USER_PASSWORD_MANDATORY_STR)]
        [StringLength(Consts.MAXIMUM_PASSWORD_LENGTH, MinimumLength = Consts.MINIMUM_PASSWORD_LENGTH)]
        public string Password { get; set; }

        public readonly string Role; // "Admin" or "Customer" 

        [Required(ErrorMessage = Consts.USER_EMAIL_MANDATORY_STR)]
        [EmailAddress(ErrorMessage = Consts.PLEASE_VALID_EMAIL_STR)]
        public string Email { get; set; }

        [Required(ErrorMessage = Consts.USER_PHONE_MANDATORY_STR)]
        [Phone(ErrorMessage = Consts.PLEASE_VALID_PHONE_STR)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = Consts.USER_ADDRESS_MANDATORY_STR)]
        [RegularExpression(Consts.ADDRESS_REGEX_STR, ErrorMessage = Consts.INVALID_ADDRESS_STR)]
        public string Address { get; set; }
    }
}
