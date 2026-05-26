using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Estado atual do jogo, no geral. Salvo e atualizado sempre, nunca destruído. Funciona como backup para o PlayerPrefs, enquanto o jogo não for fechado

    public DataState DataState;
    public UnlockState UnlockState;
    public ProgressState ProgressState;
    public CompanyState CompanyState;
    public ExpeditionState ExpeditionState;
    public MissionsState MissionsState;
    public BestiaryState BestiaryState;
    public UpgradesState UpgradesState;

    // Informações Necessárias
    public float WorldScale = 0.2f;
    public float ActualGameSpeed = 1;
    public float MaxGameSpeed = 1;

    // Linguagem
    public Language ActualLanguage = Language.Portugues;

    public enum Language
    {
        English,
        Portugues,
    }
}
