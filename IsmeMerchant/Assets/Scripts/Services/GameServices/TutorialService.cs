using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public (string,string) SetText(GameHelper.Tutorial Type)
    {
        string Title = "";
        string Info = "";

        switch (Type)
        {
            // Inicio do Jogo, na Landing
            case GameHelper.Tutorial.StartTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.StartTut = true;
                return (Title, Info);

            // Tomar Dano, na Expedition
            case GameHelper.Tutorial.ShipTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.ShipTut = true;
                return (Title, Info);

            // Inicio da Expedition, na Expedition
            case GameHelper.Tutorial.ExpeditionTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.ExpeditionTut = true;
                return (Title, Info);

            // Comprar Upgrade, na Expedition
            case GameHelper.Tutorial.UpgradesTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.UpgradesTut = true;
                return (Title, Info);

            // Inimigo Morto, na Expedition
            case GameHelper.Tutorial.ClickTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.ClickTut = true;
                return (Title, Info);

            // Clicar em Buildings, na Landing
            case GameHelper.Tutorial.BuildingsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.BuildingsTut = true;
                return (Title, Info);
            
            // Clicar em Alchemy, na Landing
            case GameHelper.Tutorial.AlchemyTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.AlchemyTut = true;
                return (Title, Info);

            // Clicar em Bestiary, na Landing
            case GameHelper.Tutorial.BestiaryTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.BestiaryTut = true;
                return (Title, Info);

            // Final do Dia 1, na Expedition
            case GameHelper.Tutorial.MarcosTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.MarcosTut = true;
                return (Title, Info);
            
            // Final da Noite 1, na Expedition
            case GameHelper.Tutorial.ExperienceTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.ExperienceTut = true;
                return (Title, Info);

            // Chegar na Destination 1, na Expedition
            case GameHelper.Tutorial.DestinationsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.DestinationsTut = true;
                return (Title, Info);

            // Primeiro Reload, na Expedition
            case GameHelper.Tutorial.WeaponsTut:
                if (GameState.ActualLanguage == GameState.Language.English)
                {
                    Title = "";
                    Info = "";
                }
                if (GameState.ActualLanguage == GameState.Language.Portugues)
                {
                    Title = "";
                    Info = "";
                }
                GameState.ProgressState.WeaponsTut = true;
                return (Title, Info);
        }

        return (Title, Info);
    }


    private void UpgradesTut(UpgradeInstance upgrade)
    {
        SetText(GameHelper.Tutorial.UpgradesTut);
    }
    private void ExpeditionTut()
    {
        SetText(GameHelper.Tutorial.ExpeditionTut);
    }
    private void DayTut()
    {
        SetText(GameHelper.Tutorial.MarcosTut);
    }
    private void ExperienceTut()
    {
        SetText(GameHelper.Tutorial.ExperienceTut);
    }
    private void DestinationTut()
    {
        SetText(GameHelper.Tutorial.DestinationsTut);
    }
    private void WeaponsTut(WeaponInstance weapon)
    {
        SetText(GameHelper.Tutorial.WeaponsTut);
    }
    private void ClickTut(EnemyRuntime enemy)
    {
        SetText(GameHelper.Tutorial.ClickTut);
    }
    private void ShipTut()
    {
        if (GameState.ExpeditionState.Ship.CurrentLife < GameState.ExpeditionState.Ship.ActualLife)
        {
            SetText(GameHelper.Tutorial.ShipTut);
        }
    }

    // Eventos
    void OnEnable()
    {
        GameEvents.OnUpgradeBought += UpgradesTut;
        ExpeditionEvents.OnExpeditionStart += ExpeditionTut;
        ExpeditionEvents.OnDayFinish += DayTut;
        ExpeditionEvents.OnNightFinish += ExperienceTut;
        ExpeditionEvents.OnDestinationArrival += DestinationTut;
        ExpeditionEvents.OnRechargeStart += WeaponsTut;
        ExpeditionEvents.OnEnemyDeath += ClickTut;
        ExpeditionEvents.OnShipAtributeChange += ShipTut;
    }

    void OnDisable()
    {
        GameEvents.OnUpgradeBought -= UpgradesTut;
        ExpeditionEvents.OnExpeditionStart -= ExpeditionTut;
        ExpeditionEvents.OnDayFinish -= DayTut;
        ExpeditionEvents.OnNightFinish -= ExperienceTut;
        ExpeditionEvents.OnDestinationArrival -= DestinationTut;
        ExpeditionEvents.OnRechargeStart -= WeaponsTut;
        ExpeditionEvents.OnEnemyDeath -= ClickTut;
        ExpeditionEvents.OnShipAtributeChange -= ShipTut;
    }
}
