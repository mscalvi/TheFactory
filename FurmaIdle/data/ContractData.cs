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
            ["c11"] = new ContractModel
            {
                Id = "c11",
                Name = "Varrer o Chão",
                Description = "",
                Icon = "icons/contracts/c11.jpg",
                Image = "images/contracts/c11.jpg",
                Level = 1,
                UnlockId = "uc110",
                PricingId = PricingHelper.PricingId.ContractAdd1m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c12"] = new ContractModel
            {
                Id = "c12",
                Name = "Carregar o Barco",
                Description = "",
                Icon = "icons/contracts/c12.jpg",
                Image = "images/contracts/c12.jpg",
                Level = 1,
                UnlockId = "uc120",
                PricingId = PricingHelper.PricingId.ContractAdd1m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            #endregion

            #region Level 2 (Aprendiz)
            ["c21"] = new ContractModel
            {
                Id = "c21",
                Name = "Servir Bebidas",
                Description = "",
                Icon = "icons/contracts/c21.jpg",
                Image = "images/contracts/c21.jpg",
                Level = 2,
                UnlockId = "uc210",
                PricingId = PricingHelper.PricingId.ContractAdd2m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c22"] = new ContractModel
            {
                Id = "c22",
                Name = "Limpar Peixe",
                Description = "",
                Icon = "icons/contracts/c22.jpg",
                Image = "images/contracts/c22.jpg",
                Level = 2,
                UnlockId = "uc220",
                PricingId = PricingHelper.PricingId.ContractAdd2m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c23"] = new ContractModel
            {
                Id = "c23",
                Name = "Lavar Figurino",
                Description = "",
                Icon = "icons/contracts/c23.jpg",
                Image = "images/contracts/c23.jpg",
                Level = 2,
                UnlockId = "uc230",
                PricingId = PricingHelper.PricingId.ContractAdd2m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            #endregion

            #region Level 3 (Iniciante)
            ["c31"] = new ContractModel
            {
                Id = "c31",
                Name = "Ajudar na Cozinha",
                Description = "",
                Icon = "icons/contracts/c31.jpg",
                Image = "images/contracts/c31.jpg",
                Level = 3,
                UnlockId = "uc310",
                PricingId = PricingHelper.PricingId.ContractAdd3m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c32"] = new ContractModel
            {
                Id = "c32",
                Name = "Organizar Ferramentas",
                Description = "",
                Icon = "icons/contracts/c32.jpg",
                Image = "images/contracts/c32.jpg",
                Level = 3,
                UnlockId = "uc320",
                PricingId = PricingHelper.PricingId.ContractAdd3m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c33"] = new ContractModel
            {
                Id = "c33",
                Name = "Preparar Armas",
                Description = "",
                Icon = "icons/contracts/c33.jpg",
                Image = "images/contracts/c33.jpg",
                Level = 3,
                UnlockId = "uc330",
                PricingId = PricingHelper.PricingId.ContractAdd3m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c34"] = new ContractModel
            {
                Id = "c34",
                Name = "Ajudar no Ensaio",
                Description = "",
                Icon = "icons/contracts/c34.jpg",
                Image = "images/contracts/c34.jpg",
                Level = 3,
                UnlockId = "uc340",
                PricingId = PricingHelper.PricingId.ContractAdd3m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = "k01",
                KnowledgeFactor3 = null,
            },
            #endregion

            #region Level 4 (Profissional)
            ["c41"] = new ContractModel
            {
                Id = "c41",
                Name = "Cozinhar",
                Description = "",
                Icon = "icons/contracts/c41.jpg",
                Image = "images/contracts/c41.jpg",
                Level = 4,
                UnlockId = "uc410",
                PricingId = PricingHelper.PricingId.ContractAdd4m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k03",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c42"] = new ContractModel
            {
                Id = "c42",
                Name = "Entalhar",
                Description = "",
                Icon = "icons/contracts/c42.jpg",
                Image = "images/contracts/c42.jpg",
                Level = 4,
                UnlockId = "uc420",
                PricingId = PricingHelper.PricingId.ContractAdd4m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k01",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c43"] = new ContractModel
            {
                Id = "c43",
                Name = "Pescar",
                Description = "",
                Icon = "icons/contracts/c43.jpg",
                Image = "images/contracts/c43.jpg",
                Level = 4,
                UnlockId = "uc430",
                PricingId = PricingHelper.PricingId.ContractAdd4m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = "k03",
                KnowledgeFactor3 = null,
            },
            ["c44"] = new ContractModel
            {
                Id = "c44",
                Name = "Apresentar",
                Description = "",
                Icon = "icons/contracts/c44.jpg",
                Image = "images/contracts/c44.jpg",
                Level = 4,
                UnlockId = "uc440",
                PricingId = PricingHelper.PricingId.ContractAdd4m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = "k01",
            },
            ["c45"] = new ContractModel
            {
                Id = "c45",
                Name = "Caçar",
                Description = "",
                Icon = "icons/contracts/c45.jpg",
                Image = "images/contracts/c45.jpg",
                Level = 4,
                UnlockId = "uc450",
                PricingId = PricingHelper.PricingId.ContractAdd4m01,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = "k05",
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
                Icon = contract.Icon,
                Image = contract.Image,
                Level = contract.Level,
                GainFactor = 1,
                PriceFactor = 1,
                TimeFactor = 1,
                UnlockId = contract.UnlockId,
                Persistence = contract.Persistence,
                State = contract.State,
                KnowledgeFactor1 = contract.KnowledgeFactor1,
                KnowledgeFactor2 = contract.KnowledgeFactor2,
                KnowledgeFactor3 = contract.KnowledgeFactor3,
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