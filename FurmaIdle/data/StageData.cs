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
                Icon = "icons/stages/s00.png",
                Images = new List<string> {
                    "images/stages/s00_0000.png",
                    "images/stages/s00_1000.png",
                    "images/stages/s00_1100.png",
                    "images/stages/s00_1010.png",
                    "images/stages/s00_1001.png",
                    "images/stages/s00_1110.png",
                    "images/stages/s00_1101.png",
                    "images/stages/s00_1011.png",
                    "images/stages/s00_1111.png",
                },
                Level = null,
                PricingId = null,
                UnlockId = null,
                State = UnlockHelper.State.Unlocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                PartySizeStart = 3,
                PartySizeMax = 5,
                ExpeditionId = 0
            },
            #endregion

            #region Unlockable Stages (s01)
            ["s01"] = new StageModel
            {
                Id = "s01",
                Name = "Correntezas",
                Description = "", // Vazio na tabela
                Icon = "icons/stages/s01.png",
                Images = new List<string> {
                    "images/stages/s01_0000.png",
                    "images/stages/s01_1000.png",
                    "images/stages/s01_1100.png",
                    "images/stages/s01_1010.png",
                    "images/stages/s01_1001.png",
                    "images/stages/s01_1110.png",
                    "images/stages/s01_1101.png",
                    "images/stages/s01_1011.png",
                    "images/stages/s01_1111.png",
                },
                Level = 1,
                PricingId = PricingHelper.PricingId.StageUnlock1m01,
                UnlockId = "ux020",
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.Permanent,
                PartySizeStart = 2,
                PartySizeMax = 2,
                ExpeditionId = 1
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

                // Cria o estado inicial do modelo clonado
                dict[id] = GetDef(id);
            }
            return dict;
        }

        public static StageModel LocateStage(string StageId)
        {
            All.TryGetValue(StageId, out _);

            return GetDef(StageId);
        }

        public static ExpeditionModel LocateExpediction(string StageId)
        {
            StageModel Stage = LocateStage(StageId);

            return Stage.ExpeditionId; // stage model tem que ter actualexpedition model
        }
    }
}