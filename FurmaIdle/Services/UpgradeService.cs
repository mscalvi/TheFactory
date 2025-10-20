using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IUpgradeService
    {
        Task ApplyUpgrade(GameModel game, string upgradeId);
    }

    public sealed class UpgradeService : IUpgradeService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;

        public UpgradeService(ICurrentGameService game, ILocateService locate)
        {
            _game = game;
            _locate = locate;
        }

        public async Task ApplyUpgrade (GameModel game, string upgradeId)
        {
            UpgradeModel up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);

            switch (up.EffectType)
            {
                // Variable Change
                case EffectHelper.EffectType.ContractLevelUnlock:
                    ContractLevelUnlock(up.TargetId);
                    break;
                case EffectHelper.EffectType.ContractCapUnlock:
                    ContractCapUnlock(up.TargetId);
                    break;

                default:
                    break;
            }
        }

        private void ContractLevelUnlock (string stageId)
        {
            StageModel stage = _locate.LocateStage(_game.CurrentGame, stageId);

            stage.ActualContractLevel += 1;
        }

        private void ContractCapUnlock(string characterId)
        {
            if (characterId == "cAll")
            {
                foreach (var c in _game.CurrentGame.Characters)
                {
                    CharacterModel character = _locate.LocateCharacter(_game.CurrentGame, c.Key);
                    character.ContractCap += 1;
                }
            }
            else
            {
                CharacterModel character = _locate.LocateCharacter(_game.CurrentGame, characterId);

                character.ContractCap += 1;
            }
        }
    }
}
