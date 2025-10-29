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
                Name = "Coleta Acelerada",
                Description = "Produz instantaneamente a média de Moedas por segundo dos Contratos da Expedição",
                Image = "images/specialties/e01.jpg",
                TargetId = "zContracts",
                EffectValue = 20.0,
                EffectType = EffectHelper.EffectType.BurstCoinGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                Duration = 0,
                Cost = 10,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e02"] = new SpecialtyModel
            {
                Id = "e02",
                Name = "Produção Eficiente",
                Description = "Aumenta a geração de Recursos de toda a Guilda",
                Image = "images/specialties/e02.jpg",
                TargetId = "aResources",
                EffectValue = 1.2,
                EffectType = EffectHelper.EffectType.ResourceGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                Duration = 20,
                Cost = 15,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e03"] = new SpecialtyModel
            {
                Id = "e03",
                Name = "Gorjetas",
                Description = "Aumenta o ganho dos Contratos da Expedição",
                Image = "images/specialties/e03.jpg",
                TargetId = "zContracts",
                EffectValue = 2.0,
                EffectType = EffectHelper.EffectType.ContractGain,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                Duration = 30,
                Cost = 30,
                PricingId = "r01",
                Modifiers = new List<ModifierModel>(),
            },
            ["e04"] = new SpecialtyModel
            {
                Id = "e04",
                Name = "Uso Consciente",
                Description = "Diminui o custo das Especialidades da Expedição",
                Image = "images/specialties/e04.jpg",
                TargetId = "zCharacters",
                EffectValue = 0.8,
                EffectType = EffectHelper.EffectType.SpecialtyCost,
                EffectOp = EffectHelper.EffectOperation.Multiplicative,
                Duration = 10,
                Cost = 10,
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
                Image = specialty.Image,
                TargetId = specialty.TargetId,
                EffectValue = specialty.EffectValue,
                EffectType = specialty.EffectType,
                EffectOp = specialty.EffectOp,
                Duration = specialty.Duration,
                Cost = specialty.Cost,
                PricingId = specialty.PricingId,
                Modifiers = specialty.Modifiers,
                Persistence = UnlockHelper.Persistence.untilTimer,
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
                    Duration = specialty.Duration,
                    Cost = specialty.Cost,
                    PricingId = specialty.PricingId,
                    Modifiers = specialty.Modifiers,
                    Persistence = UnlockHelper.Persistence.untilTimer,
                };
            }
            return dict;
        }
    }
}