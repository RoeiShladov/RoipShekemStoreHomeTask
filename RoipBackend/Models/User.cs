using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoipBackend.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Required]
        [Range(Consts.ID_MINIMUM_RANGE,Consts.ID_MAXIMUM_RANGE)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment
        public int Id { get; set; }


        [Required(ErrorMessage = Consts.USER_NAME_MANDATORY_STR)]
        [StringLength(16, MinimumLength = Consts.MINIMUM_USER_NAME_LENGTH)]
        public string Username { get; set; }


        //TODO: Hash the password before storing it in the database & check the password with the hashed one & check for strong password
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


        [Timestamp]
        public byte[] RowVersion { get; set; }


        public User(string username, string password, string role, int balance, string email, string phoneNumber, string address)
        { 
            this.Username = username;
            this.Password = password;
            this.Role = role;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
        }
    }
}
