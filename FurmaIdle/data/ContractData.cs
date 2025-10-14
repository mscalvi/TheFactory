using FurmaIdle.Models;
using FurmaIdle.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class ContractsData
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
                Icon = "icons/contracts/c11.png",
                Image = "image/contracts/c11.png",
                Level = 1,
                UnlockId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Unlocked,
                KnowledgeFactor1 = null,
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = null,
            },
            ["c12"] = new ContractModel
            {
                Id = "c12",
                Name = "Carregar o Barco",
                Description = "",
                Icon = "icons/contracts/c12.png",
                Image = "image/contracts/c12.png",
                Level = 1,
                UnlockId = null,
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Unlocked,
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
                Icon = "icons/contracts/c21.png",
                Image = "image/contracts/c21.png",
                Level = 2,
                UnlockId = "uc210",
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
                Icon = "icons/contracts/c22.png",
                Image = "image/contracts/c2.png",
                Level = 2,
                UnlockId = "uc220",
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
                Icon = "icons/contracts/c23.png",
                Image = "image/contracts/c23.png",
                Level = 2,
                UnlockId = "uc230",
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
                Icon = "icons/contracts/c31.png",
                Image = "image/contracts/c31.png",
                Level = 3,
                UnlockId = "uc310",
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
                Icon = "icons/contracts/c32.png",
                Image = "image/contracts/c32.png",
                Level = 3,
                UnlockId = "uc320",
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
                Icon = "icons/contracts/c33.png",
                Image = "image/contracts/c3.png",
                Level = 3,
                UnlockId = "uc330",
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
                Icon = "icons/contracts/c34.png",
                Image = "image/contracts/c34.png",
                Level = 3,
                UnlockId = "uc340",
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
                Icon = "icons/contracts/c41.png",
                Image = "image/contracts/c41.png",
                Level = 4,
                UnlockId = "uc410",
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
                Icon = "icons/contracts/c42.png",
                Image = "image/contracts/c42.png",
                Level = 4,
                UnlockId = "uc420",
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
                Icon = "icons/contracts/c43.png",
                Image = "image/contracts/c43.png",
                Level = 4,
                UnlockId = "uc430",
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
                Icon = "icons/contracts/c44.png",
                Image = "image/contracts/c44.png",
                Level = 4,
                UnlockId = "uc440",
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
                Icon = "icons/contracts/c45.png",
                Image = "image/contracts/c45.png",
                Level = 4,
                UnlockId = "uc450",
                Persistence = UnlockHelper.Persistence.Permanent,
                State = UnlockHelper.State.Blocked,
                KnowledgeFactor1 = "k02",
                KnowledgeFactor2 = null,
                KnowledgeFactor3 = "k05",
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

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