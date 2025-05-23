﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace RoipBackend.Models
{
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]        
        [Range(C.ID_MINIMUM_RANGE, C.ID_MAXIMUM_RANGE)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment
        public string Id { get; set; }

        [Required(ErrorMessage = C.USER_NAME_MANDATORY_STR)]
        [StringLength(16, MinimumLength = C.MINIMUM_USER_NAME_LENGTH)]
        public string Username { get; set; }

        [Required(ErrorMessage = C.USER_PASSWORD_MANDATORY_STR)]
        [StringLength(C.MAXIMUM_PASSWORD_LENGTH, MinimumLength = C.MINIMUM_PASSWORD_LENGTH)]
        public string Password { get; set; }

        public string Role { get; set; } // "Admin" or "Customer" 

        [Required(ErrorMessage = C.USER_EMAIL_MANDATORY_STR)]
        [EmailAddress(ErrorMessage = C.PLEASE_VALID_EMAIL_STR)]
        public string Email { get; set; }

        [Required(ErrorMessage = C.USER_PHONE_MANDATORY_STR)]
        [Phone(ErrorMessage = C.PLEASE_VALID_PHONE_STR)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = C.USER_ADDRESS_MANDATORY_STR)]
        [RegularExpression(C.ADDRESS_REGEX_STR, ErrorMessage = C.INVALID_ADDRESS_STR)]
        public string Address { get; set; }

        [Timestamp]
        public byte[] ?RowVersion { get; set; } // Removed the [Optional] attribute as it is invalid here.

        //public User(string username, string password, string role, string email, string phoneNumber, string address, byte[] rowVersion)
        //{
        //    this.Username = username;
        //    this.Password = password;
        //    this.Role = role;
        //    this.Email = email;
        //    this.PhoneNumber = phoneNumber;
        //    this.Address = address;
        //    this.RowVersion = rowVersion; // Initialize RowVersion with a default value.
        //}

        public User(string username, string password, string role, string email, string phoneNumber, string address)
        {
            this.Username = username;
            this.Password = password;
            this.Role = role;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
            this.RowVersion = new byte[8]; // Initialize RowVersion with a default value.
        }
    }
}
