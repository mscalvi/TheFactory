using FurmaIdle.Models; // Assumindo que TechModel está aqui
using FurmaIdle.Helpers; // Assumindo que UnlockHelper está aqui
using System.Collections.Generic;
using System.Linq;

namespace FurmaIdle.Data
{
    public class TechData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, TechModel> All = new()
        {
            #region T01x - Guildas (k01)
            ["t011"] = new TechModel
            {
                Id = "t011",
                Name = "Fundação de Guildas",
                Icon = "icons/techs/t011.png", // CORRIGIDO
                Description = "", // Vazio na tabela
                UnlockId = "l00",
                Level = 1,
                PricingId = PricingHelper.PricingId.TechUnlock1k01,
                State = UnlockHelper.State.Avaliable,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            #endregion

            #region T02x - Litorais (k02)
            ["t021"] = new TechModel
            {
                Id = "t021",
                Name = "Vilas Litorâneas",
                Icon = "icons/techs/t021.png", // CORRIGIDO
                Description = "",
                UnlockId = "l00",
                Level = 1,
                PricingId = PricingHelper.PricingId.TechUnlock1k02,
                State = UnlockHelper.State.Avaliable,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            ["t022"] = new TechModel
            {
                Id = "t022",
                Name = "Litorais Rochosos",
                Icon = "icons/techs/t022.png", // CORRIGIDO
                Description = "",
                UnlockId = "l01",
                Level = 2,
                PricingId = PricingHelper.PricingId.TechUnlock1k02,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            #endregion

            #region T03x - Sobrevivência (k03)
            ["t031"] = new TechModel
            {
                Id = "t031",
                Name = "Vida em Muradas",
                Icon = "icons/techs/t031.png", // CORRIGIDO
                Description = "",
                UnlockId = "l00",
                Level = 1,
                PricingId = PricingHelper.PricingId.TechUnlock1k03,
                State = UnlockHelper.State.Avaliable,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            ["t032"] = new TechModel
            {
                Id = "t032",
                Name = "Sobrevivência na Selva",
                Icon = "icons/techs/t032.png", // CORRIGIDO
                Description = "",
                UnlockId = "l02",
                Level = 2,
                PricingId = PricingHelper.PricingId.TechUnlock1k03,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            #endregion

            #region T04x - Navegação (k04)
            ["t041"] = new TechModel
            {
                Id = "t041",
                Name = "Construção de Barcos",
                Icon = "icons/techs/t041.png", // CORRIGIDO
                Description = "",
                UnlockId = "l00",
                Level = 1,
                PricingId = PricingHelper.PricingId.TechUnlock1k04,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            #endregion

            #region T05x - Caça (k05)
            ["t051"] = new TechModel
            {
                Id = "t051",
                Name = "Presas",
                Icon = "icons/techs/t051.png", // CORRIGIDO
                Description = "",
                UnlockId = "l02",
                Level = 1,
                PricingId = PricingHelper.PricingId.TechUnlock1k05,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            ["t052"] = new TechModel
            {
                Id = "t052",
                Name = "Predadores",
                Icon = "icons/techs/t052.png", // CORRIGIDO
                Description = "",
                UnlockId = "l02",
                Level = 2,
                PricingId = PricingHelper.PricingId.TechUnlock1k05,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            },
            ["t053"] = new TechModel
            {
                Id = "t053",
                Name = "Criaturas Insanas",
                Icon = "icons/techs/t053.png", // CORRIGIDO
                Description = "",
                UnlockId = "l03",
                Level = 3,
                PricingId = PricingHelper.PricingId.TechUnlock1k05,
                State = UnlockHelper.State.Blocked,
                Persistence = UnlockHelper.Persistence.untilExpansion,
            }
            #endregion
        };

        // --- Métodos Reutilizáveis do Padrão ---

        public static TechModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var tech))
            {
                throw new KeyNotFoundException($"Tech with ID '{id}' not found.");
            }

            return new TechModel
            {
                Id = tech.Id,
                Name = tech.Name,
                Icon = tech.Icon,
                Description = tech.Description,
                UnlockId = tech.UnlockId,
                Level = tech.Level,
                PricingId = tech.PricingId,
                State = tech.State,
                Persistence = tech.Persistence,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, TechModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, TechModel>(All.Count);
            if (ShowOrder.Count == 0) PopulateOrder();
            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var tech)) continue;
                dict[id] = GetDef(id);
            }
            return dict;
        }
    }
}