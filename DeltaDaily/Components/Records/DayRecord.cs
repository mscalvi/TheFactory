using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Records;

public sealed class DayRecord
{
    [Key] public string Date { get; set; } = "";   // chave primária: "yyyy-MM-dd"
    public bool Closed { get; set; }               // espelha DayModel.DiaFechado
    public DateTime UpdatedAtUtc { get; set; }     // para futuro sync
    public string Payload { get; set; } = "";      // JSON do DayModel
}
