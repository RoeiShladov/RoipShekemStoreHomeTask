using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Runtime.InteropServices;

namespace RoipBackend.Models
{   
    [Microsoft.EntityFrameworkCore.Index(nameof(ProductName), IsUnique = true)]
    public class Product
    {
        [Key]
        [Range(C.ID_MINIMUM_RANGE, C.ID_MAXIMUM_RANGE)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment  
        public int Id { get; set; }

        [Required(ErrorMessage = C.USER_NAME_MANDATORY_STR)]
        [StringLength(C.MAXIMUM_PRODUCT_NAME_LENGTH, MinimumLength = C.MINIMUM_USER_NAME_LENGTH)]
        public string ProductName { get; set; }

        [StringLength(C.DESCRIBTION_MAXIMUM_LENGTH, ErrorMessage = C.PRODUCT_DESCRIBTION_STR)]
        public string Description { get; set; }

        [Required]
        [Range(C.PRODUCT_MIN_VALUE, double.MaxValue, ErrorMessage = C.PRICE_ABOVE_ZERO_STR)]
        public int Price { get; set; }

        [Required]
        [Range(C.QUANTITY_MINIMUM_VALUE, int.MaxValue, ErrorMessage = C.QUANTITY_NOT_NEGATIVE_STR)]
        public int Quantity { get; set; }

        [Url(ErrorMessage = C.IMAGE_URL_VALIDATION_STR)]
        public string ImageUrl { get; set; }

        [Timestamp]
        public byte[] ?RowVersion { get; set; } = Array.Empty<byte>(); // Initialize to avoid CS8618  

        //public Product(string productName, string description, int price, int quantity, string imageUrl, byte[] rowVersion)
        //{
        //    this.ProductName = productName;
        //    this.Description = description;
        //    this.Price = price;
        //    this.Quantity = quantity;
        //    this.ImageUrl = imageUrl;
        //    this.RowVersion = rowVersion; // Initialize RowVersion with a default value.
        //}

        public Product(string productName, string description, int price, int quantity, string imageUrl)
        {
            this.ProductName = productName;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.ImageUrl = imageUrl;
            this.RowVersion = new byte[8]; // Initialize RowVersion with a default value.
        }
    }
}
