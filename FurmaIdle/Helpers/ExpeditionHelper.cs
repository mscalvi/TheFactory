using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Helpers
{
    public class ExpeditionHelper
    {
        public static List<CharacterModel> GetActiveCharacters(ExpeditionModel Expedition)
        {
            List<CharacterModel> characterModels = new List<CharacterModel> ();


            foreach(string Character in Expedition.ContractsId)
            {
                CharacterModel inParty = LocateHelper.LocateCharacter(Character);
                characterModels.Add(inParty);
            }

            return characterModels;
        }
    }
}
