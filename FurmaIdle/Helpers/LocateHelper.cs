using FurmaIdle.Models;

namespace FurmaIdle.Helpers
{
    public class LocateHelper
    {
        public static CharacterModel LocateCharacter(string CharId)
        {
            CharacterModel Character = null;

            return Character;
        }

        public static StageModel LocateStage(string StageId)
        {
            StageModel Stage = null;

            return Stage;
        }

        public static ExpeditionModel LocateExpedition(string StageId)
        {
            StageModel Stage = LocateStage(StageId);

            return Stage.ActiveExpedition;
        }

        public static ContractModel LocateContract(string ContractId)
        {
            ContractModel Contract = null;

            return Contract;
        }
    }
}
