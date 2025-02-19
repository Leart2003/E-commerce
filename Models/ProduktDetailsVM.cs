using System.ComponentModel.DataAnnotations;

namespace Punim_Diplome.Models
{
    public class ProduktDetailsVM
    {
        public Produkt? Produkt { get; set; }
        public List<Koment>? Koments { get; set; } = new List<Koment>();

        [Required(ErrorMessage = "Comment content is required.")]
        public string NewKoment { get; set; }= string.Empty;
    }
}
