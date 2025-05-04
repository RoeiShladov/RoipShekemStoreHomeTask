using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoipBackend.Models
{
    public class Logger
    {
        [Key]
        [Required]
        [Range(C.ID_MINIMUM_RANGE, C.ID_MAXIMUM_RANGE)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Unique identifier for the log entry.

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } // Time of the log entry.

        [Required]
        [StringLength(C.LOG_LEVEL_MAXIMUM_LENGTH)]
        public string LogLevel { get; set; } // Level of the log (e.g., Info, Warning, Error).

        [Required]
        [StringLength(C.LOG_MESSAGE_MAXIMUM_LENGTH)]
        public string FriendlyDescribtion { get; set; } // Log message.        

        [StringLength(C.LOG_EXCEPTION_MAXIMUM_LENGTH)]
        public string Exception { get; set; } // Exception details if applicable.

        [Range(C.ID_MINIMUM_RANGE, C.ID_MAXIMUM_RANGE)]
        public string UserId { get; set; } // Optional: User associated with the log entry.

        //[StringLength(C.LOG_ADDITIONAL_DATA_MAXIMUM_LENGTH)]
        //public string AdditionalData { get; set; } // Optional: Any extra data.
    }
}
