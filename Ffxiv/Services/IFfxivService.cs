using System.Threading.Tasks;

namespace Ffxiv.Services
{
    public interface IFfxivService
    {
        Task<dynamic> GetItem(string id);

        Task<dynamic> GetRecipe(string id);
    }
}