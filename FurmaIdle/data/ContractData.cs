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
            #region Level 0 (Pessoal)
            ["c001"] = new ContractModel
            {
                Id = "c001",
                Name = "Estudar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c001.svg",
                Image = "images/contracts/c001.svg",
                Level = 0,
                UnlockId = null,
                PricingId = PricingHelper.PricingId.ContractPurchase0,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            #endregion

            #region Level 1 (Trivial)
            ["c101"] = new ContractModel
            {
                Id = "c101",
                Name = "Varrer o Chão",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c101.svg",
                Image = "images/contracts/c101.svg",
                Level = 1,
                UnlockId = "uc101",
                PricingId = PricingHelper.PricingId.ContractPurchase1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            ["c102"] = new ContractModel
            {
                Id = "c102",
                Name = "Carregar o Barco",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c102.svg",
                Image = "images/contracts/c102.svg",
                Level = 1,
                UnlockId = "uu102",
                PricingId = PricingHelper.PricingId.ContractPurchase1,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k04",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.Port | ContractHelper.Context.River,
            },
            #endregion

            #region Level 2 (Aprendiz)
            ["c201"] = new ContractModel
            {
                Id = "c201",
                Name = "Servir Bebidas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c201.svg",
                Image = "images/contracts/c201.svg",
                Level = 2,
                UnlockId = "uu201",
                PricingId = PricingHelper.PricingId.ContractPurchase2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            ["c202"] = new ContractModel
            {
                Id = "c202",
                Name = "Limpar Peixe",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c202.svg",
                Image = "images/contracts/c202.svg",
                Level = 2,
                UnlockId = "uu202",
                PricingId = PricingHelper.PricingId.ContractPurchase2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge | ContractHelper.Context.River | ContractHelper.Context.Port,
            },
            ["c203"] = new ContractModel
            {
                Id = "c203",
                Name = "Lavar Figurino",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c203.svg",
                Image = "images/contracts/c203.svg",
                Level = 2,
                UnlockId = "uu203",
                PricingId = PricingHelper.PricingId.ContractPurchase2,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k01",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            #endregion

            #region Level 3 (Iniciante)
            ["c301"] = new ContractModel
            {
                Id = "c301",
                Name = "Ajudar na Cozinha",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c301.svg",
                Image = "images/contracts/c301.svg",
                Level = 3,
                UnlockId = "uu301",
                PricingId = PricingHelper.PricingId.ContractPurchase3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge | ContractHelper.Context.WarZone,
            },
            ["c302"] = new ContractModel
            {
                Id = "c302",
                Name = "Organizar Ferramentas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c302.svg",
                Image = "images/contracts/c302.svg",
                Level = 3,
                UnlockId = "uu302",
                PricingId = PricingHelper.PricingId.ContractPurchase3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            ["c303"] = new ContractModel
            {
                Id = "c303",
                Name = "Preparar Armas",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c303.svg",
                Image = "images/contracts/c303.svg",
                Level = 3,
                UnlockId = "uu303",
                PricingId = PricingHelper.PricingId.ContractPurchase3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanLarge | ContractHelper.Context.WarZone | ContractHelper.Context.Wild,
            },
            ["c304"] = new ContractModel
            {
                Id = "c304",
                Name = "Ajudar no Ensaio",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c304.svg",
                Image = "images/contracts/c304.svg",
                Level = 3,
                UnlockId = "uu304",
                PricingId = PricingHelper.PricingId.ContractPurchase3,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = "k01",
                KnowledgeFactor2 = null,
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            #endregion

            #region Level 4 (Profissional)
            ["c401"] = new ContractModel
            {
                Id = "c401",
                Name = "Cozinhar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c401.svg",
                Image = "images/contracts/c401.svg",
                Level = 4,
                UnlockId = "uu401",
                PricingId = PricingHelper.PricingId.ContractPurchase4,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k03",
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge | ContractHelper.Context.WarZone | ContractHelper.Context.Wild,
            },
            ["c402"] = new ContractModel
            {
                Id = "c402",
                Name = "Entalhar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c402.svg",
                Image = "images/contracts/c402.svg",
                Level = 4,
                UnlockId = "uu402",
                PricingId = PricingHelper.PricingId.ContractPurchase4,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k01",
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge | ContractHelper.Context.Wild,
            },
            ["c403"] = new ContractModel
            {
                Id = "c403",
                Name = "Pescar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c403.svg",
                Image = "images/contracts/c403.svg",
                Level = 4,
                UnlockId = "uu403",
                PricingId = PricingHelper.PricingId.ContractPurchase4,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k03",
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.Port | ContractHelper.Context.River,
            },
            ["c404"] = new ContractModel
            {
                Id = "c404",
                Name = "Apresentar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c404.svg",
                Image = "images/contracts/c404.svg",
                Level = 4,
                UnlockId = "uu404",
                PricingId = PricingHelper.PricingId.ContractPurchase4,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k01",
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.UrbanSmall | ContractHelper.Context.UrbanLarge,
            },
            ["c405"] = new ContractModel
            {
                Id = "c405",
                Name = "Caçar",
                Description = "",
                Lore = "",
                Icon = "icons/contracts/c405.svg",
                Image = "images/contracts/c405.svg",
                Level = 4,
                UnlockId = "uu405",
                PricingId = PricingHelper.PricingId.ContractPurchase4,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                CoinId = "m01",
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k05",
                Modifiers = new List<ModifierModel>(),
                Context = ContractHelper.Context.Wild | ContractHelper.Context.Infected | ContractHelper.Context.Cavernous,
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
                ItemType = contract.ItemType,
                Description = contract.Description,
                Lore = contract.Lore,
                Icon = contract.Icon,
                Image = contract.Image,
                Level = contract.Level,
                PricingId = contract.PricingId,
                UnlockId = contract.UnlockId,
                Persistence = contract.Persistence,
                State = contract.State,
                GameUseState = UnlockHelper.ContractState.Available,
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