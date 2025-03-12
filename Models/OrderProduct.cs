using Microsoft.AspNetCore.Identity;
using Punim_Diplome.Models;

namespace Punim_Diplome.Models
{
    public class OrderProduct
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }


        public DateTime OrderDate { get; set; } = DateTime.Now;


        public virtual IdentityUser User { get; set; }

        public Produkt Produkt { get; set; }

        public List<Produkt> Produkts { get; set; } = new List<Produkt>();



    }
}
