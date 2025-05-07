using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Models
{
    public class SignalRUserDTO
    {
        [Key]
        public string ConnectionId { get; set; } // The Hub can generate a connectionId for each open session.

        [Required(ErrorMessage = C.USER_NAME_MANDATORY_STR)]
        [StringLength(16, MinimumLength = C.MINIMUM_USER_NAME_LENGTH)]
        public string Name { get; set; }


        public string Role { get; set; } // "Admin" or "Customer" 


        [Required(ErrorMessage = C.USER_EMAIL_MANDATORY_STR)]
        [EmailAddress(ErrorMessage = C.PLEASE_VALID_EMAIL_STR)]
        public string Email { get; set; }
    }
}
