using DeltaDaily.Components.Data;
using static DeltaDaily.Components.Data.DreamsData;

namespace DeltaDaily.Components.Models;

public class DayModel
{
    public DateTime Data { get; set; } = DateTime.Now;

    // Bloco 1
    public TimeOnly? PrimeiroDespertar { get; set; }
    public TimeOnly? SairDaCama { get; set; }

    public bool TomouRemedio { get; set; }
    public string? DoseRemedio { get; set; }
    public TimeOnly? HorarioRemedio { get; set; }

    public TimeOnly? LogoutHora { get; set; }
    public TimeOnly? DeitarHora { get; set; }

    // Bloco 2
    public List<WorkModel> Dedicadas { get; set; } = new();
    public List<WorkModel> Extras { get; set; } = new();

    // Bloco 3
    private SleepData.SleepQuality? QualidadeSono { get; set; }
    private DreamsData.SonhoTipo? TipoSonhos { get; set; }
    private string? NotasSonhos { get; set; }

    private HumorData.Humor? HumorMedio { get; set; }
    private bool VariacaoHumor { get; set; }
    private string? VariacaoHumorNotas { get; set; }
}
