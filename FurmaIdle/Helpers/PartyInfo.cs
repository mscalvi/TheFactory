namespace FurmaIdle.Services
{
    public readonly record struct PartyInfo(
        int UsedRoster,   // selecionados no roster (pré-start)
        int CapRoster,    // cap da guilda
        int UsedStage,    // membros na expedição ativa deste stage
        int CapStage,     // cap efetivo do stage (min(stage,guild))
        bool ExpeditionActive
    );
}