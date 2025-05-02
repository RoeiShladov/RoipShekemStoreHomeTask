using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class UserConnection
    {
        [Required(ErrorMessage = Consts.ID_REQUIRED_STR)]
        [Range(Consts.ID_MINIMUM_RANGE, Consts.ID_MAXIMUM_RANGE)]
        [Key]
        public string ConnectionId { get; set; } // מזהה החיבור של SignalR

        [Required(ErrorMessage = Consts.USER_EMAIL_MANDATORY_STR)]
        [EmailAddress(ErrorMessage = Consts.PLEASE_VALID_EMAIL_STR)]
        public string Email { get; set; } // .מזהה המשתמש

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ConnectedAt { get; set; } // תאריך ושעת החיבור.

        //public DateTime? DisconnectedAt { get; set; } // תאריך ושעת הניתוק (יכול להיות NULL).
    }
}
