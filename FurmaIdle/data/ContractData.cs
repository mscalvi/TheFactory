using FurmaIdle.Models;
using FurmaIdle.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class ContractData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, ContractModel> All = new()
        {
            #region Level 1 (Trivial)
            ["c011"] = new ContractModel
            {
                Id = "c011",
                Name = "Varrer o Chão",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c011.svg",
                Image = "images/contracts/c011.svg",
                Level = 1,
                UnlockId = null,
                PricingId = PricingHelper.PricingId.ContractPurchase11,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c012"] = new ContractModel
            {
                Id = "c012",
                Name = "Carregar o Barco",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c012.svg",
                Image = "images/contracts/c012.svg",
                Level = 1,
                UnlockId = "uu120",
                PricingId = PricingHelper.PricingId.ContractPurchase11,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Level 2 (Aprendiz)
            ["c021"] = new ContractModel
            {
                Id = "c021",
                Name = "Servir Bebidas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c021.svg",
                Image = "images/contracts/c021.svg",
                Level = 2,
                UnlockId = "uu210",
                PricingId = PricingHelper.PricingId.ContractPurchase12,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c022"] = new ContractModel
            {
                Id = "c022",
                Name = "Limpar Peixe",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c022.svg",
                Image = "images/contracts/c022.svg",
                Level = 2,
                UnlockId = "uu220",
                PricingId = PricingHelper.PricingId.ContractPurchase12,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c023"] = new ContractModel
            {
                Id = "c023",
                Name = "Lavar Figurino",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c023.svg",
                Image = "images/contracts/c023.svg",
                Level = 2,
                UnlockId = "uu230",
                PricingId = PricingHelper.PricingId.ContractPurchase12,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Level 3 (Iniciante)
            ["c031"] = new ContractModel
            {
                Id = "c031",
                Name = "Ajudar na Cozinha",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c031.svg",
                Image = "images/contracts/c031.svg",
                Level = 3,
                UnlockId = "uu310",
                PricingId = PricingHelper.PricingId.ContractPurchase13,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c032"] = new ContractModel
            {
                Id = "c032",
                Name = "Organizar Ferramentas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c032.svg",
                Image = "images/contracts/c032.svg",
                Level = 3,
                UnlockId = "uu320",
                PricingId = PricingHelper.PricingId.ContractPurchase13,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c033"] = new ContractModel
            {
                Id = "c033",
                Name = "Preparar Armas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c033.svg",
                Image = "images/contracts/c033.svg",
                Level = 3,
                UnlockId = "uu330",
                PricingId = PricingHelper.PricingId.ContractPurchase13,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c034"] = new ContractModel
            {
                Id = "c034",
                Name = "Ajudar no Ensaio",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c034.svg",
                Image = "images/contracts/c034.svg",
                Level = 3,
                UnlockId = "uu340",
                PricingId = PricingHelper.PricingId.ContractPurchase13,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k01",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion

            #region Level 4 (Profissional)
            ["c041"] = new ContractModel
            {
                Id = "c041",
                Name = "Cozinhar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c041.svg",
                Image = "images/contracts/c041.svg",
                Level = 4,
                UnlockId = "uu410",
                PricingId = PricingHelper.PricingId.ContractPurchase14,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c042"] = new ContractModel
            {
                Id = "c042",
                Name = "Entalhar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c042.svg",
                Image = "images/contracts/c042.svg",
                Level = 4,
                UnlockId = "uu420",
                PricingId = PricingHelper.PricingId.ContractPurchase14,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k01",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c043"] = new ContractModel
            {
                Id = "c043",
                Name = "Pescar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c043.svg",
                Image = "images/contracts/c043.svg",
                Level = 4,
                UnlockId = "uu430",
                PricingId = PricingHelper.PricingId.ContractPurchase14,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = "k03",
                Modifiers = new List<ModifierModel>(),
            },
            ["c044"] = new ContractModel
            {
                Id = "c044",
                Name = "Apresentar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c044.svg",
                Image = "images/contracts/c044.svg",
                Level = 4,
                UnlockId = "uu440",
                PricingId = PricingHelper.PricingId.ContractPurchase14,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c045"] = new ContractModel
            {
                Id = "c045",
                Name = "Caçar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c045.svg",
                Image = "images/contracts/c045.svg",
                Level = 4,
                UnlockId = "uu450",
                PricingId = PricingHelper.PricingId.ContractPurchase14,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = "k05",
                Modifiers = new List<ModifierModel>(),
            },
            #endregion
        };

        // --- Criação ---

        public static ContractModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var contract))
            {
                throw new KeyNotFoundException($"Contract with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new ContractModel
            {
                Id = contract.Id,
                Name = contract.Name,
                Description = contract.Description,
                Lore = contract.Lore,
                Icon = contract.Icon,
                Image = contract.Image,
                Level = contract.Level,
                PricingId = contract.PricingId,
                UnlockId = contract.UnlockId,
                Persistence = contract.Persistence,
                State = contract.State,
                GameUseState = UnlockHelper.ContractState.Avaliable,
                CoinId = contract.CoinId,
                KnowledgeFactor1 = contract.KnowledgeFactor1,
                KnowledgeFactor2 = contract.KnowledgeFactor2,
                Modifiers = contract.Modifiers,
                UseState = contract.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, ContractModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, ContractModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var contract)) continue;

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}