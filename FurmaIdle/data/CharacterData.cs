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
            ["p001"] = new CharacterModel
            {
                Id = "p001",
                Name = "Ferri Karu",
                Description = "Taberneiro",
                Lore = "",
                Icon = "icons/characters/p001.jpg",
                Image = "images/characters/p001.jpg",
                UnlockId = "up001",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k02",
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c11", "c21", "c31", "c41" },
                TraitId = "tr05",
                SpecialtyId = "es03"
            },
            ["p002"] = new CharacterModel
            {
                Id = "p002",
                Name = "Maik Monhang",
                Description = "Artesão",
                Lore = "",
                Icon = "icons/characters/p002.jpg",
                Image = "images/characters/p002.jpg",
                UnlockId = "up002",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c11", "c21", "c32", "c42" },
                TraitId = "tr04",
                SpecialtyId = "es01"
            },
            ["p003"] = new CharacterModel
            {
                Id = "p003",
                Name = "Claimi Eky",
                Description = "Pescador",
                Lore = "",
                Icon = "icons/characters/p003.jpg",
                Image = "images/characters/p003.jpg",
                UnlockId = "up003",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k03",
                KnowledgeFactor1 = "k02",
                ContractsIds = new() { "c12", "c22", "c31", "c43" },
                TraitId = "tr03",
                SpecialtyId = "es02"
            },
            ["p004"] = new CharacterModel
            {
                Id = "p004",
                Name = "Alan Nhengar",
                Description = "Bardo",
                Lore = "",
                Icon = "icons/characters/p004.jpg",
                Image = "images/characters/p004.jpg",
                UnlockId = "up004",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor1 = null,
                ContractsIds = new() { "c11", "c23", "c34", "c44" },
                TraitId = "tr01",
                SpecialtyId = "es03"
            },
            ["p011"] = new CharacterModel
            {
                Id = "p011",
                Name = "Jaime Boor",
                Description = "Explorador",
                Lore = "",
                Icon = "icons/characters/p011.jpg",
                Image = "images/characters/p011.jpg",
                UnlockId = "up011",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 5,
                KnowledgeFactor2 = "k02",
                KnowledgeFactor1 = "k04",
                ContractsIds = new() { "c12", "c22", "c32", "c45" },
                TraitId = "tr02",
                SpecialtyId = "es04"
            },
            ["p021"] = new CharacterModel
            {
                Id = "p021",
                Name = "Yg Iepora",
                Description = "Caçador",
                Lore = "",
                Icon = "icons/characters/p021.jpg",
                Image = "images/characters/p021.jpg",
                UnlockId = "up021",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 5,
                KnowledgeFactor2 = "k03",
                KnowledgeFactor1 = null,
                ContractsIds = new() { "c11", "c22", "c33", "c45" },
                TraitId = "tr04",
                SpecialtyId = "es02"
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
                Lore = chara.Lore,
                Icon = chara.Icon,
                Image = chara.Image,
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
                GainFactor = 1,
                PriceFactor = 1,
                TimeFactor = 1,
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
                    Lore = chara.Lore,
                    Icon = chara.Icon,
                    Image = chara.Image,
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
                };
            }
            return dict;
        }
    }
}
