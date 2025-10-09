using FurmaIdle.Models;
using System.Linq;
using static FurmaIdle.Helpers.UpgradeCostEnum;
using static System.Net.Mime.MediaTypeNames;

namespace FurmaIdle.Data
{
    public class ContractData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> Order = new();

        internal static readonly Dictionary<string, ContractModel> All = new()
        {
            #region Nível 1
            ["c10"] = new ContractModel
            {
                Id = "c10",
                Name = "Varrer o Chão",
                Level = 1,
                Image = "images/icons/contracts/c10.png",
                FirstKnowId = "k10",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c11"] = new ContractModel
            {
                Id = "c11",
                Name = "Carregar o Barco",
                Level = 1,
                Image = "images/icons/contracts/c11.png",
                FirstKnowId = "k12",
                SecondKnowId = "k11",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            #endregion
            #region Nível 2
            ["c20"] = new ContractModel
            {
                Id = "c20",
                Name = "Servir Bebidas",
                Level = 2,
                Image = "images/icons/contracts/c20.png",
                FirstKnowId = "",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c21"] = new ContractModel
            {
                Id = "c21",
                Name = "Limpar Peixes",
                Level = 2,
                Image = "images/icons/contracts/c21.png",
                FirstKnowId = "",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c22"] = new ContractModel
            {
                Id = "c22",
                Name = "Lavar Figurinos",
                Level = 2,
                Image = "images/icons/contracts/c22.png",
                FirstKnowId = "",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            #endregion
            #region Nível 3
            ["c30"] = new ContractModel
            {
                Id = "c30",
                Name = "Ajudar na Cozinha",
                Level = 3,
                Image = "images/icons/contracts/c30.png",
                FirstKnowId = "",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c31"] = new ContractModel
            {
                Id = "c31",
                Name = "Organizar Ferramentas",
                Level = 3,
                Image = "images/icons/contracts/c31.png",
                FirstKnowId = "k12",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c32"] = new ContractModel
            {
                Id = "c32",
                Name = "Preparar Armas",
                Level = 3,
                Image = "images/icons/contracts/c32.png",
                FirstKnowId = "k12",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            ["c33"] = new ContractModel
            {
                Id = "c33",
                Name = "Ajudar no Ensaio",
                Level = 3,
                Image = "images/icons/contracts/c33.png",
                FirstKnowId = "k10",
                SecondKnowId = "",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = true,
                Avaliable = false,
                ConDestId = "d00"
            },
            #endregion
            #region Nível 4
            ["c40"] = new ContractModel
            {
                Id = "c40",
                Name = "Cozinhar",
                Level = 4,
                Image = "images/icons/contracts/c40.png",
                FirstKnowId = "",
                SecondKnowId = "k12",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = false,
                Avaliable = true,
                ConDestId = "d00"
            },
            ["c41"] = new ContractModel
            {
                Id = "c41",
                Name = "Entalhar",
                Level = 4,
                Image = "images/icons/contracts/c41.png",
                FirstKnowId = "",
                SecondKnowId = "k10",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = false,
                Avaliable = false,
                ConDestId = "d01"
            },
            ["c42"] = new ContractModel
            {
                Id = "c42",
                Name = "Pescar",
                Level = 4,
                Image = "images/icons/contracts/c42.png",
                FirstKnowId = "k12",
                SecondKnowId = "k11",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = false,
                Avaliable = false,
                ConDestId = "d01"
            },
            ["c43"] = new ContractModel
            {
                Id = "c43",
                Name = "Apresentar",
                Level = 4,
                Image = "images/icons/contracts/c43.png",
                FirstKnowId = "",
                SecondKnowId = "k10",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = false,
                Avaliable = true,
                ConDestId = "d00"
            },
            ["c44"] = new ContractModel
            {
                Id = "c44",
                Name = "Caçar",
                Level = 4,
                Image = "images/icons/contracts/c44.png",
                FirstKnowId = "k11",
                SecondKnowId = "k12",
                ThirdKnowId = "",
                FirstDiferential = "",
                SecondDiferential = "",
                Unlocked = false,
                Avaliable = false,
                ConDestId = "d02"
            }
            #endregion
        };

        public static void PopulateOrderFromAll()
        {
            Order.Clear();
            IEnumerable<string> keys = (All == null)
                ? Enumerable.Empty<string>()
                : All.Keys.AsEnumerable();

            Order.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static ContractModel GetDef(string id)
        {
            var cont = All[id];
            return new ContractModel
            {
                Id = cont.Id,
                Name = cont.Name,
                Level = cont.Level,
                Image = cont.Image,
                FirstKnowId = cont.FirstKnowId,
                SecondKnowId = cont.SecondKnowId,
                ThirdKnowId = cont.ThirdKnowId,
                FirstDiferential = cont.FirstDiferential,
                SecondDiferential = cont.SecondDiferential,
                Unlocked = cont.Unlocked,
                Avaliable = cont.Avaliable,
                ConDestId = cont.ConDestId,
                Quant = 0
            };
        }

        public static Dictionary<string, ContractModel> CreateInitialContracts()
        {
            var CoinsCollection = new Dictionary<string, ContractModel>(capacity: All.Count);

            if (Order.Count == 0) PopulateOrderFromAll();

            foreach (var id in Order)
            {
                if (!All.TryGetValue(id, out var cont)) continue;

                CoinsCollection[id] = new ContractModel
                {
                    Id = cont.Id,
                    Name = cont.Name,
                    Level = cont.Level,
                    Image = cont.Image,
                    FirstKnowId = cont.FirstKnowId,
                    SecondKnowId = cont.SecondKnowId,
                    ThirdKnowId = cont.ThirdKnowId,
                    FirstDiferential = cont.FirstDiferential,
                    SecondDiferential = cont.SecondDiferential,
                    Unlocked = cont.Unlocked,
                    Avaliable = cont.Avaliable,
                    ConDestId = cont.ConDestId,
                    Quant = 0
                };
            }

            return CoinsCollection;
        }
    }
}