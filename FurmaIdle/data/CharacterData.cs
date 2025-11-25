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
            #region s01
            ["p001"] = new CharacterModel
            {
                Id = "p001",
                Name = "Ferri Karu",
                Description = "Taberneiro",
                Lore = "",
                Icon = "icons/characters/p001.svg",
                Image = "images/characters/p001.svg",
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k02",
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c011", "c021", "c031", "c041" },
                TraitId = "o01",
                SpecialtyId = "e01",
                Modifiers = new List<ModifierModel>(),
            },
            ["p102"] = new CharacterModel
            {
                Id = "p102",
                Name = "Maik Monhang",
                Description = "Artesão",
                Lore = "",
                Icon = "icons/characters/p102.svg",
                Image = "images/characters/p102.svg",
                UnlockId = "up102",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor1 = "k03",
                ContractsIds = new() { "c011", "c021", "c032", "c042" },
                TraitId = "o02",
                SpecialtyId = "e02",
                Modifiers = new List<ModifierModel>(),
            },
            ["p103"] = new CharacterModel
            {
                Id = "p103",
                Name = "Claimi Eky",
                Description = "Pescador",
                Lore = "",
                Icon = "icons/characters/p103.svg",
                Image = "images/characters/p103.svg",
                UnlockId = "up103",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.InBase,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k03",
                KnowledgeFactor1 = "k02",
                ContractsIds = new() { "c012", "c022", "c031", "c043" },
                TraitId = "o03",
                SpecialtyId = "e03",
                Modifiers = new List<ModifierModel>(),
            },
            ["p104"] = new CharacterModel
            {
                Id = "p104",
                Name = "Alan Nhengar",
                Description = "Bardo",
                Lore = "",
                Icon = "icons/characters/p104.svg",
                Image = "images/characters/p104.svg",
                UnlockId = "up104",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor1 = null,
                ContractsIds = new() { "c011", "c023", "c034", "c044" },
                TraitId = "o04",
                SpecialtyId = "e04",
                Modifiers = new List<ModifierModel>(),
            },
            ["p111"] = new CharacterModel
            {
                Id = "p111",
                Name = "Jaime Boor",
                Description = "Explorador",
                Lore = "",
                Icon = "icons/characters/p111.svg",
                Image = "images/characters/p111.svg",
                UnlockId = "up111",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k02",
                KnowledgeFactor1 = "k04",
                ContractsIds = new() { "c012", "c022", "c032", "c045" },
                TraitId = "o05",
                SpecialtyId = "e05",
                Modifiers = new List<ModifierModel>(),
            },
            ["p121"] = new CharacterModel
            {
                Id = "p121",
                Name = "Yg Iepora",
                Description = "Caçador",
                Lore = "",
                Icon = "icons/characters/p121.svg",
                Image = "images/characters/p121.svg",
                UnlockId = "up121",
                State = UnlockHelper.State.Blocked,
                CharState = UnlockHelper.CharState.Blocked,
                InStageId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                ContractCap = 1,
                KnowledgeFactor2 = "k03",
                KnowledgeFactor1 = "k05",
                ContractsIds = new() { "c011", "c022", "c033", "c045" },
                TraitId = "o06",
                SpecialtyId = "e06",
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
                    Modifiers = chara.Modifiers,
                    UseState = chara.UseState,
                };
            }
            return dict;
        }
    }
}
