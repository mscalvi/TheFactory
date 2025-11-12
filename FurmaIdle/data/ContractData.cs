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
                Icon = "icons/contracts/c011.jpg",
                Image = "images/contracts/c011.jpg",
                Level = 1,
                UnlockId = null,
                PricingId = PricingHelper.PricingId.ContractPurchase01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c012"] = new ContractModel
            {
                Id = "c012",
                Name = "Carregar o Barco",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c012.jpg",
                Image = "images/contracts/c012.jpg",
                Level = 1,
                UnlockId = "uu120",
                PricingId = PricingHelper.PricingId.ContractPurchase01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
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
                Icon = "icons/contracts/c021.jpg",
                Image = "images/contracts/c021.jpg",
                Level = 2,
                UnlockId = "uu210",
                PricingId = PricingHelper.PricingId.ContractPurchase02,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c022"] = new ContractModel
            {
                Id = "c022",
                Name = "Limpar Peixe",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c022.jpg",
                Image = "images/contracts/c022.jpg",
                Level = 2,
                UnlockId = "uu220",
                PricingId = PricingHelper.PricingId.ContractPurchase02,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c023"] = new ContractModel
            {
                Id = "c023",
                Name = "Lavar Figurino",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c023.jpg",
                Image = "images/contracts/c023.jpg",
                Level = 2,
                UnlockId = "uu230",
                PricingId = PricingHelper.PricingId.ContractPurchase02,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
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
                Icon = "icons/contracts/c031.jpg",
                Image = "images/contracts/c031.jpg",
                Level = 3,
                UnlockId = "uu310",
                PricingId = PricingHelper.PricingId.ContractPurchase03,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c032"] = new ContractModel
            {
                Id = "c032",
                Name = "Organizar Ferramentas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c032.jpg",
                Image = "images/contracts/c032.jpg",
                Level = 3,
                UnlockId = "uu320",
                PricingId = PricingHelper.PricingId.ContractPurchase03,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c033"] = new ContractModel
            {
                Id = "c033",
                Name = "Preparar Armas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c033.jpg",
                Image = "images/contracts/c033.jpg",
                Level = 3,
                UnlockId = "uu330",
                PricingId = PricingHelper.PricingId.ContractPurchase03,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c034"] = new ContractModel
            {
                Id = "c034",
                Name = "Ajudar no Ensaio",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c034.jpg",
                Image = "images/contracts/c034.jpg",
                Level = 3,
                UnlockId = "uu340",
                PricingId = PricingHelper.PricingId.ContractPurchase03,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor3 = null,
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
                Icon = "icons/contracts/c041.jpg",
                Image = "images/contracts/c041.jpg",
                Level = 4,
                UnlockId = "uu410",
                PricingId = PricingHelper.PricingId.ContractPurchase04,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c042"] = new ContractModel
            {
                Id = "c042",
                Name = "Entalhar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c042.jpg",
                Image = "images/contracts/c042.jpg",
                Level = 4,
                UnlockId = "uu420",
                PricingId = PricingHelper.PricingId.ContractPurchase04,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k01",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c043"] = new ContractModel
            {
                Id = "c043",
                Name = "Pescar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c043.jpg",
                Image = "images/contracts/c043.jpg",
                Level = 4,
                UnlockId = "uu430",
                PricingId = PricingHelper.PricingId.ContractPurchase04,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = "k03",
                KnowledgeFactor3 = null,
                Modifiers = new List<ModifierModel>(),
            },
            ["c044"] = new ContractModel
            {
                Id = "c044",
                Name = "Apresentar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c044.jpg",
                Image = "images/contracts/c044.jpg",
                Level = 4,
                UnlockId = "uu440",
                PricingId = PricingHelper.PricingId.ContractPurchase04,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = "k01",
                Modifiers = new List<ModifierModel>(),
            },
            ["c045"] = new ContractModel
            {
                Id = "c045",
                Name = "Caçar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c045.jpg",
                Image = "images/contracts/c045.jpg",
                Level = 4,
                UnlockId = "uu450",
                PricingId = PricingHelper.PricingId.ContractPurchase04,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = "k05",
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
                UseState = UnlockHelper.ContractState.Avaliable,
                CoinId = contract.CoinId,
                KnowledgeFactor1 = contract.KnowledgeFactor1,
                KnowledgeFactor2 = contract.KnowledgeFactor2,
                KnowledgeFactor3 = contract.KnowledgeFactor3,
                Modifiers = contract.Modifiers,
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