namespace FurmaIdle.Models
{
    public enum SpecialtyTarget { Coins, Resources }         // simples p/ agora
    public enum SpecialtyOp { Multiplicative }               // só mult por enquanto

    public sealed class SpecialtyModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public SpecialtyTarget Target { get; set; }
        public SpecialtyOp Op { get; set; } = SpecialtyOp.Multiplicative;
        public double Value { get; set; } = 1.0;             // x1.2, x2.0 etc.
        public double DurationSec { get; set; } = 20;        // 20s, 60s…
        public string CostResourceId { get; set; } = "r100";
        public double Cost { get; set; } = 10;
        public string? ResourceIdScope { get; set; }         // ex.: "r100" para e01/e03
    }

    // runtime: buff ativo
    public sealed class ActiveSpecialtyModel
    {
        public string SpecialtyId { get; set; } = "";
        public string CharId { get; set; } = "";
        public DateTimeOffset EndsAtUtc { get; set; }
    }
}
