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
            #region A Casa de Ferri (si)
            ["s00"] = new StageModel
            {
                Id = "s00",
                Name = "A Casa de Ferri",
                Description = "",
                Lore = "",
                Icon = "icons/stages/s00.svg",
                Images = new List<string> {
                    "images/stages/s00_0000.svg",
                    "images/stages/s00_1000.svg",
                },
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StartPartySize = 1,
                MaxPartySize = 1,
                StartContractLevel = 1,
                MaxContractLevel = 2,
                CoinId = "m01",
                ClickId = "i0i",
                Modifiers = new List<ModifierModel>(),
                Expedition = new ExpeditionModel(),
            },
            #endregion

            #region Ilha de Vera (s01)
            ["s01"] = new StageModel
            {
                Id = "s01",
                Name = "Ilha de Vera",
                Description = "",
                Lore = "",
                Icon = "icons/stages/s01.svg",
                Images = new List<string> {
                    "images/stages/s01_0000.svg",
                    "images/stages/s01_1000.svg",
                    "images/stages/s01_1100.svg",
                    "images/stages/s01_1010.svg",
                    "images/stages/s01_1001.svg",
                    "images/stages/s01_1110.svg",
                    "images/stages/s01_1101.svg",
                    "images/stages/s01_1011.svg",
                    "images/stages/s01_1111.svg",
                },
                UnlockId = null,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StartPartySize = 1,
                MaxPartySize = 4,
                StartContractLevel = 1,
                MaxContractLevel = 6,
                CoinId = "m01",
                ClickId = "i01",
                Modifiers = new List<ModifierModel>(),
                Expedition = new ExpeditionModel(),
            },
            #endregion

            #region Unlockable Stages (s2)
            ["s02"] = new StageModel
            {
                Id = "s02",
                Name = "Correntezas",
                Description = "",
                Lore = "",
                Icon = "icons/stages/s02.svg",
                Images = new List<string> {
                    "images/stages/s02_0000.svg",
                    "images/stages/s02_1000.svg",
                    "images/stages/s02_1100.svg",
                    "images/stages/s02_1010.svg",
                    "images/stages/s02_1001.svg",
                    "images/stages/s02_1110.svg",
                    "images/stages/s02_1101.svg",
                    "images/stages/s02_1011.svg",
                    "images/stages/s02_1111.svg",
                },
                UnlockId = "us02",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                StartPartySize = 2,
                MaxPartySize = 3,
                StartContractLevel = 4,
                MaxContractLevel = 6,
                CoinId = "m02",
                ClickId = "i02",
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
                UseState = stage.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();

            // Ordena usando StringComparer.Ordinal (s01, s01, etc.)
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