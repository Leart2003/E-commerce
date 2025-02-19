using DocumentFormat.OpenXml.Office2021.PowerPoint.Comment;
using Punim_Diplome.Models;

namespace Punim_Diplome.Data.Services
{
    public interface IKomentService
    {


        Task Add(Koment koment);

        Task  GetCommentById(int id);

    }
}
