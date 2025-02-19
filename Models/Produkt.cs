using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Punim_Diplome.Models
{
    public class Produkt
        
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Brand { get; set; }

        public string? Description { get; set; }

        [Range(1, 128, ErrorMessage = "RAM must be between 1 and 128 GB.")]
        public int Ram { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

       public DateOnly Date { get; set; }

        public string? Storage { get; set; }

        [Required(ErrorMessage = "Storage type is required.")]
        public string Storagetype { get; set; } = string.Empty;

        [Range(10, 50, ErrorMessage = "Screen size must be between 10 and 50 inches.")]
        public int ScreenInch { get; set; }

        public string? Procesor { get; set; }

        public string? Category { get; set; }

        public string? ImageFileName { get; set; }

        public ICollection<Koment> Koments { get; set; } = new List<Koment>();












    }


}
