using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Punim_Diplome.Models
{
    public class OrderProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        [ForeignKey("ProductId")]
        [ValidateNever]
        public virtual Produkt Produkt { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public virtual IdentityUser User { get; set; }

    }
}
