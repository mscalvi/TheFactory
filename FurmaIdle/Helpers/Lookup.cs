using FurmaIdle.Models;
using FurmaIdle.Services;

namespace FurmaIdle.Helpers
{
    public static class LookupData
    {
        public static StageModel? Stage(GameModel gm, IStageService stages, string id)
            => stages.Get(gm, id);

        public static CharacterModel? Character(GameModel gm, string id)
            => gm.Characters != null && gm.Characters.TryGetValue(id, out var c) ? c : null;

        public static TechModel? Tech(GameModel gm, string id)
            => gm.Technologies != null && gm.Technologies.TryGetValue(id, out var t) ? t : null;

        public static DestinationModel? Dest(GameModel gm, string id)
            => gm.Destinations != null && gm.Destinations.TryGetValue(id, out var d) ? d : null;

        public static UpgradeModel? Upgrade(GameModel gm, string id)
            => gm.Upgrades != null && gm.Upgrades.TryGetValue(id, out var u) ? u : null;

        public static ResourceModel? Resource(GameModel gm, string id)
            => gm.Resources != null && gm.Resources.TryGetValue(id, out var r) ? r : null;
    }
}
