using FurmaIdle.Models;
using FurmaIdle.Helpers;

namespace FurmaIdle.Data
{
    public class SpecialtyData
    {
        public static int SchemaVersion => 1;

        public static readonly List<string> ShowOrder = new();

        internal static readonly Dictionary<string, SpecialtyModel> All = new()
        {
            #region Specialties
            ["e01"] = new SpecialtyModel
            {
                Id = "e01",
                Name = "Gorjetas",
                Description = "Produz instantaneamente a média de Moedas por segundo dos Contratos da Expedição",
                Lore = "",
                Image = "images/specialties/e01.svg",
                Icon = "icons/specialties/e01.svg",
                TargetId = "aContracts",
                EffectValue = 20.0,
                EffectType = EffectHelper.EffectType.BurstCoinGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Duration = 0.2,
                Cost = 10,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e02"] = new SpecialtyModel
            {
                Id = "e02",
                Name = "Cobrar um Extra",
                Description = "Aumenta o ganho dos Contratos da Expedição",
                Lore = "",
                Image = "images/specialties/e02.svg",
                Icon = "icons/specialties/e02.svg",
                TargetId = "aContracts",
                EffectValue = 2.0,
                EffectType = EffectHelper.EffectType.ContractGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Duration = 20,
                Cost = 30,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e03"] = new SpecialtyModel
            {
                Id = "e03",
                Name = "Força de Pescador",
                Description = "Aumenta o ganho por Click",
                Lore = "",
                Image = "images/specialties/e03.svg",
                Icon = "icons/specialties/e03.svg",
                TargetId = "i01",
                EffectValue = 3.0,
                EffectType = EffectHelper.EffectType.ClickGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Duration = 10,
                Cost = 35,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e04"] = new SpecialtyModel
            {
                Id = "e04",
                Name = "Empolgar",
                Description = "Diminui o custo das Especialidades da Expedição",
                Lore = "",
                Image = "images/specialties/e04.svg",
                Icon = "icons/specialties/e04.svg",
                TargetId = "aSpecialties",
                EffectValue = 0.3,
                EffectType = EffectHelper.EffectType.SpecialtyCost,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Cost,
                Duration = 10,
                Cost = 50,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e05"] = new SpecialtyModel
            {
                Id = "e05",
                Name = "Produção Eficiente",
                Description = "Aumenta a geração de Recursos de toda a Guilda",
                Lore = "",
                Image = "images/specialties/e05.svg",
                Icon = "icons/specialties/e05.svg",
                TargetId = "aResources",
                EffectValue = 1.2,
                EffectType = EffectHelper.EffectType.ResourceGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Gain,
                Duration = 60,
                Cost = 15,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e06"] = new SpecialtyModel
            {
                Id = "e06",
                Name = "Caça Rápida",
                Description = "Diminui o Tempo de Caçar",
                Lore = "",
                Image = "images/specialties/e06.svg",
                Icon = "icons/specialties/e06.svg",
                TargetId = "c045",
                EffectValue = 0.05,
                EffectType = EffectHelper.EffectType.ContractTime,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                EffectSupertype = EffectHelper.EffectSupertype.Time,
                Duration = 5,
                Cost = 80,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            }
            #endregion
        };

        public static SpecialtyModel GetDef(string id)
        {
            if (!All.TryGetValue(id, out var specialty))
            {
                throw new KeyNotFoundException($"Specialty with ID '{id}' not found.");
            }

            return new SpecialtyModel
            {
                Id = specialty.Id,
                Name = specialty.Name,
                Description = specialty.Description,
                Lore = specialty.Lore,
                Image = specialty.Image,
                Icon = specialty.Icon,
                TargetId = specialty.TargetId,
                EffectValue = specialty.EffectValue,
                EffectType = specialty.EffectType,
                EffectOp = specialty.EffectOp,
                EffectSupertype = specialty.EffectSupertype,
                Duration = specialty.Duration,
                Cost = specialty.Cost,
                PricingId = specialty.PricingId,
                Modifiers = specialty.Modifiers,
                Persistence = UnlockHelper.Persistence.untilTimer,
                UseState = specialty.UseState,
            };
        }

        public static void PopulateOrder()
        {
            ShowOrder.Clear();
            IEnumerable<string> keys = All?.Keys.AsEnumerable() ?? Enumerable.Empty<string>();
            ShowOrder.AddRange(keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        public static Dictionary<string, SpecialtyModel> CreateInitialStates()
        {
            var dict = new Dictionary<string, SpecialtyModel>(All.Count);

            if (ShowOrder.Count == 0) PopulateOrder();

            foreach (var id in ShowOrder)
            {
                if (!All.TryGetValue(id, out var specialty)) continue;

                dict[id] = new SpecialtyModel
                {
                    Id = specialty.Id,
                    Name = specialty.Name,
                    Description = specialty.Description,
                    Image = specialty.Image,
                    TargetId = specialty.TargetId,
                    EffectValue = specialty.EffectValue,
                    EffectType = specialty.EffectType,
                    EffectOp = specialty.EffectOp,
                    EffectSupertype = specialty.EffectSupertype,
                    Duration = specialty.Duration,
                    Cost = specialty.Cost,
                    PricingId = specialty.PricingId,
                    Modifiers = specialty.Modifiers,
                    Persistence = UnlockHelper.Persistence.untilTimer,
                    UseState = specialty.UseState,
                };
            }
            return dict;
        }
    }
}