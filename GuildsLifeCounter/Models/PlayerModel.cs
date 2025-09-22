using System.ComponentModel.DataAnnotations;

namespace GuildsLifeCounter.Models;

public class PlayerModel
{
    [Required, StringLength(32)]
    public string Nome { get; set; } = string.Empty;

    public string? Lider { get; set; }
    public string? Membro1 { get; set; }
    public string? Membro2 { get; set; }
    public string? Membro3 { get; set; }
    public string? Membro4 { get; set; }

    [Range(0, int.MaxValue)]
    public int Fama { get; set; }

    [Range(0, int.MaxValue)]
    public int Experiencia { get; set; }

    [Range(0, int.MaxValue)]
    public int Dinheiro { get; set; }

    [Range(0, int.MaxValue)]
    public int Ingredientes { get; set; }
}
