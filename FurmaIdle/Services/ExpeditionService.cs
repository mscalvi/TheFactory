using FurmaIdle.Models;
using System.Xml.Linq;

namespace FurmaIdle.Services
{
    public interface IExpeditionService 
    {
        public List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        public ExpeditionService(ILocateService locate, ICurrentGameService game)
        {
            _locate = locate;
            _game = game;
        }

        public List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition)
        {
            var result = new List<CharacterModel>();
            if (expedition?.PartyIds == null) return result;

            foreach (var id in expedition.PartyIds)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                var c = _locate.LocateCharacter(id);
                if (c != null) result.Add(c);
            }
            return result;
        }

    }
}
