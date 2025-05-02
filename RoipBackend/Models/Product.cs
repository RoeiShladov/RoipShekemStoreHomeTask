using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoipBackend.Models
{
    public class Product
    {
        [Required]
        [Range(Consts.ID_MINIMUM_RANGE, Consts.ID_MAXIMUM_RANGE)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment
        public required int Id { get; set; }

        [Required(ErrorMessage = Consts.USER_NAME_MANDATORY_STR)]
        [StringLength(Consts.MAXIMUM_PRODUCT_NAME_LENGTH, MinimumLength = Consts.MINIMUM_USER_NAME_LENGTH)]
        public required string ProductName { get; set; }

        [StringLength(Consts.DESCRIBTION_MAXIMUM_LENGTH, ErrorMessage = Consts.PRODUCT_DESCRIBTION_STR)]
        public string Description { get; set; }

        [Required]
        [Range(Consts.PRODUCT_MIN_VALUE, double.MaxValue, ErrorMessage = Consts.PRICE_ABOVE_ZERO_STR)]
        public decimal Price { get; set; }
      
        [Required]
        [Range(Consts.QUANTITY_MINIMUM_VALUE, int.MaxValue, ErrorMessage = Consts.QUANTITY_NOT_NEGATIVE_STR)]
        public int Quantity { get; set; }

        [Url(ErrorMessage = Consts.IMAGE_URL_VALIDATION_STR)]
        public string ImageUrl { get; set; }
       
        public Product(string productName, string description, int price, int quantity, string imageUrl)
        {
            this.ProductName = productName;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.ImageUrl = imageUrl;
        }
    }
}
