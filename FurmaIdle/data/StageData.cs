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
                Description = "",
                Lore = "",
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
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StartPartySize = 1,
                MaxPartySize = 5,
                StartContractLevel = 1,
                MaxContractLevel = 6,
                CoinId = "m01",
                ClickId = "i00",
                Modifiers = new List<ModifierModel>(),
                Expedition = new ExpeditionModel(),
            },
            #endregion

            #region Unlockable Stages (s01)
            ["s01"] = new StageModel
            {
                Id = "s01",
                Name = "Correntezas",
                Description = "",
                Lore = "",
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
                UnlockId = "ux020",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StartPartySize = 2,
                MaxPartySize = 3,
                StartContractLevel = 4,
                MaxContractLevel = 6,
                CoinId = "m02",
                ClickId = "i00",
                Modifiers = new List<ModifierModel>(),
                Expedition = new ExpeditionModel(),
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
                Lore = stage.Lore,
                Icon = stage.Icon,
                Images = new List<string>(stage.Images),
                UnlockId = stage.UnlockId,
                State = stage.State,
                Persistence = stage.Persistence,
                StartPartySize = stage.StartPartySize,
                MaxPartySize = stage.MaxPartySize,
                StartContractLevel = stage.StartContractLevel,
                MaxContractLevel = stage.MaxContractLevel,
                Expedition = stage.Expedition,
                CoinId = stage.CoinId,
                ClickId = stage.ClickId,
                Modifiers = stage.Modifiers,
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