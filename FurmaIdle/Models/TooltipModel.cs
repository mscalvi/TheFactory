namespace FurmaIdle.Models
{
    public sealed record TooltipModel(
        string Id,
        string Name,
        string Conhecimentos,       // k11, k10 (ou nomes futuramente)
        string ContratosDisponiveis,// c10, c20…
        string Traco,               // trait id/nomes
        string Especialidade        // e01…
    );
}