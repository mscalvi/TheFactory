using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Estado atual do jogo, no geral. Salvo e atualizado sempre, nunca destruído. Funciona como backup para o PlayerPrefs, enquanto o jogo não for fechado

    public DataState DataState;
    public ShipState ShipState;
    public UnlockState UnlockState;
    public ProgressState ProgressState;
    public CompanyState CompanyState;
    public RecordsState RecordsState;
    public ExpeditionState ExpeditionState;
    public MissionsState MissionsState;
    public TripulationState TripulationState;
    public BestiaryState BestiaryState;

    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Informações Necessárias
    public float WorldScale = 0.2f;
    public MissionInstance MainMission;
}
