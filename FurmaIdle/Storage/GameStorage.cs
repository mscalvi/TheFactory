using System.Threading.Tasks;
using FurmaIdle.Models;

namespace FurmaIdle.Storage
{
    public class GameStorage
    {
        public interface IGameStore
        {
            Task<GameModel?> LoadAsync(string key = "main");
            Task SaveAsync(GameModel model, string key = "main");
        }
    }
}
