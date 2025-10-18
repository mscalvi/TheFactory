using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FurmaIdle.Data
{
    public class StageData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, StageModel> All = new()
        {
            #region Initial Stage (s00)
            ["s00"] = new StageModel
            {
                Id = "s00",
                Name = "Ilha de Vera",
                Description = "", // Vazio na tabela
                Icon = "icons/stages/s00.jpg",
                Images = new List<string> {
                    "images/stages/s00_0000.jpg",
                    "images/stages/s00_1000.jpg",
                    "images/stages/s00_1100.jpg",
                    "images/stages/s00_1010.jpg",
                    "images/stages/s00_1001.jpg",
                    "images/stages/s00_1110.jpg",
                    "images/stages/s00_1101.jpg",
                    "images/stages/s00_1011.jpg",
                    "images/stages/s00_1111.jpg",
                },
                Level = null,
                PricingId = null,
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                PartySizeStart = 3,
                PartySizeMax = 5,
                StartContractLevel = 1,
                MaxContractLevel = 6,
                CoinId = "m01",
                ClickId = "cl00",
            },
            #endregion

            #region Unlockable Stages (s01)
            ["s01"] = new StageModel
            {
                Id = "s01",
                Name = "Correntezas",
                Description = "", // Vazio na tabela
                Icon = "icons/stages/s01.jpg",
                Images = new List<string> {
                    "images/stages/s01_0000.jpg",
                    "images/stages/s01_1000.jpg",
                    "images/stages/s01_1100.jpg",
                    "images/stages/s01_1010.jpg",
                    "images/stages/s01_1001.jpg",
                    "images/stages/s01_1110.jpg",
                    "images/stages/s01_1101.jpg",
                    "images/stages/s01_1011.jpg",
                    "images/stages/s01_1111.jpg",
                },
                Level = 1,
                PricingId = PricingHelper.PricingId.StageUnlock1m01,
                UnlockId = "ux020",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                PartySizeStart = 2,
                PartySizeMax = 2,
                StartContractLevel = 4,
                MaxContractLevel = 6,
                CoinId = "m02",
                ClickId = "cl00",
            },
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static StageModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var stage))
            {
                throw new KeyNotFoundException($"Stage with ID '{id}' not found.");
            }

            // Retorna uma nova instância (cópia) para não modificar a definição estática
            return new StageModel
            {
                Id = stage.Id,
                Name = stage.Name,
                Description = stage.Description,
                Icon = stage.Icon,
                Images = new List<string>(stage.Images),
                Level = stage.Level,
                PricingId = stage.PricingId,
                UnlockId = stage.UnlockId,
                State = stage.State,
                Persistence = stage.Persistence,
                PartySizeStart = stage.PartySizeStart,
                PartySizeMax = stage.PartySizeMax,
                StartContractLevel = stage.StartContractLevel,
                MaxContractLevel = stage.MaxContractLevel,
                ActiveExpedition = new ExpeditionModel(),
                CoinId = stage.CoinId,
                ClickId = stage.ClickId,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (s00, s01, etc.)
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, StageModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, StageModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var stage)) continue;

                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}