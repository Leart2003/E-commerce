using DocumentFormat.OpenXml.InkML;
using Punim_Diplome.Models;

namespace Punim_Diplome.Data.Services
{
    public interface IProduktService
    {



        Task<Produkt> GetByID(int id);

    }
}
