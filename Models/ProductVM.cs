namespace Punim_Diplome.Models
{
    public class ProductVM
    {

        public IEnumerable<Produkt> Products { get; set; } = new List<Produkt>();

        public string?  SearchString { get; set; }

        public string? SelectedBrand { get; set; }

        public string? SelectedCategory { get; set; }



        public IEnumerable<string>? AvailableBrands { get; set; }

        public IEnumerable<string>? AvailableCategories { get; set; }



    }
}
