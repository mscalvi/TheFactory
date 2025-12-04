using FurmaIdle.Models;
using FurmaIdle.Helpers;

namespace FurmaIdle.Data
{
    public class CharacterData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, CharacterModel> All = new()
        {
            #region s00
            ["p0001"] = new CharacterModel
            {
                Id = "p0001",
                Name = "Ferri Karu",
                Description = "",
                Class = "Taberneiro",
                Lore = "",
                Icon = "icons/characters/p0001.svg",
                NavIcon = "icons/nav/p0001.svg",
                Image = "images/characters/p0001.svg",
                BigImage = "images/big/p0001.svg",
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = null,
                KnowledgeFactor1 = "k02",
                ContractsIds = new() { "c001", "c101", "c201", "c301", "c401" },
                TraitId = "a0001",
                SpecialtyId = "e0001",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region s01
            ["p0101"] = new CharacterModel
            {
                Id = "p0101",
                Name = "Maik Monhang",
                Description = "",
                Class = "Artesão",
                Lore = "",
                Icon = "icons/characters/p0101.svg",
                NavIcon = "icons/nav/p0101.svg",
                Image = "images/characters/p0101.svg",
                BigImage = "images/big/p0101.svg",
                UnlockId = "up0101",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = null,
                KnowledgeFactor1 = "k01",
                ContractsIds = new() { "c101", "c201", "c302", "c402" },
                TraitId = "a0101",
                SpecialtyId = "e0101",
                Modifiers = new List<ModifierModel>(),
            },
            ["p0102"] = new CharacterModel
            {
                Id = "p0102",
                Name = "Claimi Eky",
                Description = "",
                Class = "Pescador",
                Lore = "",
                Icon = "icons/characters/p0102.svg",
                NavIcon = "icons/nav/p0102.svg",
                Image = "images/characters/p0102.svg",
                BigImage = "images/big/p0102.svg",
                UnlockId = "up0102",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = null,
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c102", "c202", "c301", "c403" },
                TraitId = "a0102",
                SpecialtyId = "e0102",
                Modifiers = new List<ModifierModel>(),
            },
            ["p0103"] = new CharacterModel
            {
                Id = "p0103",
                Name = "Alan Nhengar",
                Description = "",
                Class = "Bardo",
                Lore = "",
                Icon = "icons/characters/p0103.svg",
                NavIcon = "icons/nav/p0103.svg",
                Image = "images/characters/p0103.svg",
                BigImage = "images/big/p0103.svg",
                UnlockId = "up0103",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c101", "c203", "c304", "c404" },
                TraitId = "a0103",
                SpecialtyId = "e0103",
                Modifiers = new List<ModifierModel>(),
            },
            ["p0111"] = new CharacterModel
            {
                Id = "p0111",
                Name = "Jaime Boor",
                Description = "",
                Class = "Explorador",
                Lore = "",
                Icon = "icons/characters/p0111.svg",
                NavIcon = "icons/nav/p0111.svg",
                Image = "images/characters/p0111.svg",
                BigImage = "images/big/p0111.svg",
                UnlockId = "up0111",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k04",
                KnowledgeFactor1 = null,
                ContractsIds = new() { "c102", "c202", "c302", "c405" },
                TraitId = "a0111",
                SpecialtyId = "e0111",
                Modifiers = new List<ModifierModel>(),
            },
            ["p0121"] = new CharacterModel
            {
                Id = "p0121",
                Name = "Yg Iepora",
                Description = "",
                Class = "Caçador",
                Lore = "",
                Icon = "icons/characters/p0121.svg",
                NavIcon = "icons/nav/p0121.svg",
                Image = "images/characters/p0121.svg",
                BigImage = "images/big/p0121.svg",
                UnlockId = "up0121",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k05",
                KnowledgeFactor1 = null,
                ContractsIds = new() { "c101", "c202", "c303", "c405" },
                TraitId = "a0121",
                SpecialtyId = "e0121",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        public static CharacterModel GetDef(string id)
        {
            var chara = All[id];
            return new CharacterModel
            {
                Id = chara.Id,
                Name = chara.Name,
                Description = chara.Description,
                Class = chara.Class,
                Lore = chara.Lore,
                Icon = chara.Icon,
                NavIcon = chara.NavIcon,
                Image = chara.Image,
                BigImage = chara.BigImage,
                UnlockId = chara.UnlockId,
                State = chara.State,
                CharState = chara.CharState,
                InStageId = chara.InStageId,
                Persistence= chara.Persistence,
                ContractCap = chara.ContractCap,
                KnowledgeFactor2 = chara.KnowledgeFactor2,
                KnowledgeFactor1 = chara.KnowledgeFactor1,
                ContractsIds = chara.ContractsIds,
                TraitId = chara.TraitId,
                SpecialtyId = chara.SpecialtyId,
                Modifiers = chara.Modifiers,
                UseState = chara.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = (All == null)
                ? Enumerable.Empty<string>()
                : All.Keys.AsEnumerable();

            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, CharacterModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, CharacterModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var chara)) continue;

                dict[id] = new CharacterModel
                {
                    Id = chara.Id,
                    Name = chara.Name,
                    Description = chara.Description,
                    Class = chara.Class,
                    Lore = chara.Lore,
                    Icon = chara.Icon,
                    NavIcon = chara.NavIcon,
                    Image = chara.Image,
                    BigImage = chara.BigImage,
                    UnlockId = chara.UnlockId,
                    State = chara.State,
                    InStageId = chara.InStageId,
                    Persistence = chara.Persistence,
                    ContractCap = chara.ContractCap,
                    KnowledgeFactor2 = chara.KnowledgeFactor2,
                    KnowledgeFactor1 = chara.KnowledgeFactor1,
                    ContractsIds = chara.ContractsIds,
                    TraitId = chara.TraitId,
                    SpecialtyId = chara.SpecialtyId,
                    Modifiers = chara.Modifiers,
                    UseState = chara.UseState,
                };
            }
            return dict;
        }
    }
}
