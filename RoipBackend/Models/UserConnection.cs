using Microsoft.EntityFrameworkCore;
using System;

namespace RoipBackend.Models
{
    public class UserConnection
    {
        public char ConnectionId { get; set; } // מזהה החיבור של SignalR

        public int UserId { get; set; } // .מזהה המשתמש

        public DateTime ConnectedAt { get; set; } // תאריך ושעת החיבור.

        public DateTime? DisconnectedAt { get; set; } // תאריך ושעת הניתוק (יכול להיות NULL).
    }
}
