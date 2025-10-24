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
            ["es01"] = new SpecialtyModel
            {
                Id = "es01",
                Name = "Coleta Acelerada",
                Description = "Produz instantaneamente a média de Moedas por segundo da Expedição",
                Image = "images/specialties/es01.jpg",
                TargetId = "aCoins",
                EffectValue = 20.0,
                EffectType = EffectHelper.EffectType.BurstCoinGain,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                Duration = 0,
                Cost = 10,
                PricingId = "r01"
            },
            ["es02"] = new SpecialtyModel
            {
                Id = "es02",
                Name = "Produção Eficiente",
                Description = "Aumenta a geração de Recursos da Expedição",
                Image = "images/specialties/es02.jpg",
                TargetId = "aResources",
                EffectValue = 1.2,
                EffectType = EffectHelper.EffectType.ResourceGain,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                Duration = 20,
                Cost = 15,
                PricingId = "r01"
            },
            ["es03"] = new SpecialtyModel
            {
                Id = "es03",
                Name = "Gorjetas",
                Description = "Aumenta o ganho dos Contratos da Expedição",
                Image = "images/specialties/es03.jpg",
                TargetId = "aContracts",
                EffectValue = 2.0,
                EffectType = EffectHelper.EffectType.CoinGain,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                Duration = 30,
                Cost = 30,
                PricingId = "r01"
            },
            ["es04"] = new SpecialtyModel
            {
                Id = "es04",
                Name = "Uso Consciente",
                Description = "Diminui o custo das Especialidades da Expedição",
                Image = "images/specialties/es04.jpg",
                TargetId = "aSpecialties",
                EffectValue = 0.8,
                EffectType = EffectHelper.EffectType.SpecialtyCost,
                EffectOperation = EffectHelper.EffectOperation.Multiplicative,
                Duration = 10,
                Cost = 10,
                PricingId = "r01"
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
                EffectOperation = specialty.EffectOperation,
                Duration = specialty.Duration,
                Cost = specialty.Cost,
                PricingId = specialty.PricingId,
                GainFactor = 1,
                PriceFactor = 1,
                TimeFactor = 1,
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
                    EffectOperation = specialty.EffectOperation,
                    Duration = specialty.Duration,
                    Cost = specialty.Cost,
                    PricingId = specialty.PricingId,
                    GainFactor = 1,
                    PriceFactor = 1,
                    TimeFactor = 1,
                };
            }
            return dict;
        }
    }
}