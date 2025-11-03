using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class ModifierModel
    {
        public string ApplyerId { get; set; }                       //
        public EffectHelper.EffectType Type { get; set; }           // Tipo de Efeito do Modificador (Gain, Time, Cap, Cost)
        public UnlockHelper.Persistence Scope { get; set; }         // Tipo de Reset do Modificador (Perma, Expe, Expan)
        public EffectHelper.EffectOperation Operation { get; set; } // Tipo de Ação do Modificador (Add, Mult)
        public double Value { get; set; }                           // Valor do Modificador
        public EffectHelper.EffectSupertype Supertype { get; set; }
    }
}
