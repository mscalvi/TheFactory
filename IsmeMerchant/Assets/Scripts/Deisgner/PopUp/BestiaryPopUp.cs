using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryPopUp : MonoBehaviour
{
    private GameState GameState;

    [SerializeField] GameObject BestiaryPanel;

    [SerializeField] Button CloseBtn;

    [SerializeField] TextMeshProUGUI Title;

    public TMP_Text TypeName;
    [SerializeField] Transform TypePanel;
    [SerializeField] EnemyTypeDefinition EnemyTypeDefinition;

    [SerializeField] EnemyBestiaryDefinition EnemyBestiaryDefinition;
    [SerializeField] Transform SpeciesPanel;

    // Informações
    [SerializeField] GameObject InfoPanel;

    [SerializeField] Slider KillCounterSlider;
    [SerializeField] GameObject KillSlider;

    public TMP_Text KillCounterText;
    public TMP_Text EnemyName;
    public TMP_Text EnemyLifeTitle;
    public TMP_Text EnemyLife;
    public TMP_Text EnemyLifeGain;
    public TMP_Text EnemySpeedTitle;
    public TMP_Text EnemySpeed;
    public TMP_Text EnemySpeedGain;
    public TMP_Text EnemyDamageTitle;
    public TMP_Text EnemyDamage;
    public TMP_Text EnemyDamageGain;
    public TMP_Text EnemyAtkSpeedTitle;
    public TMP_Text EnemyAtkSpeed;
    public TMP_Text EnemyAtkSpeedGain;

    EnemyHelper.EnemyType EnemyType;

    public void Show(GameState gameState)
    {
        ClearMainContainer();

        GameState = gameState;

        Hide();

        BestiaryPanel.SetActive(true);

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Title.text = "Bestiário";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Title.text = "Bestiary";
        }

        foreach (EnemyHelper.EnemyType type in Enum.GetValues(typeof(EnemyHelper.EnemyType)))
        {
            var obj = Instantiate(EnemyTypeDefinition, TypePanel);

            var ui = obj.GetComponent<EnemyTypeDefinition>();

            ui.Setup(type, this, GameState);
        }

        InfoPanel.SetActive(false);
        KillSlider.SetActive(false);
        ShowSpecies(EnemyType);
    }

    public void ShowSpecies(EnemyHelper.EnemyType EnemyType)
    {
        ClearContainer();

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            TypeName.text = EnemyType.ToString();
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            TypeName.text = EnemyType.ToString();
        }

        foreach (var enemy in GameState.DataState.enemies.Values)
        {
            if (enemy.EnemyType != EnemyType) continue;

            if (!enemy.Known) continue;

            if (enemy.UnlockStatus == UnlockHelper.UnlockStatus.Available)
            {
                var obj = Instantiate(EnemyBestiaryDefinition, SpeciesPanel);

                var ui = obj.GetComponent<EnemyBestiaryDefinition>();

                ui.Setup(enemy, this, GameState);
            }
        }
    }

    public void ShowInfo(EnemyInstance Enemy)
    {
        EnemyName.text = "???";

        EnemyLifeTitle.text = "";
        EnemyLife.text = "";
        EnemyLifeGain.text = "";

        EnemySpeedTitle.text = "";
        EnemySpeed.text = "";
        EnemySpeedGain.text = "";

        EnemyDamageTitle.text = "";
        EnemyDamage.text = "";
        EnemyDamageGain.text = "";

        EnemyAtkSpeedTitle.text = "";
        EnemyAtkSpeed.text = "";
        EnemyAtkSpeedGain.text = "";

        InfoPanel.SetActive(true);
        KillSlider.SetActive(true);

        float killProgress = (float)(GameState.ExpeditionState.Ship.CurrentLife / GameState.ExpeditionState.Ship.ActualLife);

        BestiaryEntry entry = GameState.BestiaryState.Bestiary[Enemy.Id];
        int kills = entry.KilledTotal;

        int nextGoal = EnemyHelper.KillThresholds[^1];
        int level = 0;

        foreach (int goal in EnemyHelper.KillThresholds)
        {
            if (kills < goal)
            {
                nextGoal = goal;
                break;
            }

            level++;
        }

        KillCounterSlider.minValue = 0;
        KillCounterSlider.maxValue = nextGoal;
        KillCounterSlider.value = Mathf.Min(kills, nextGoal);

        KillCounterText.text = $"{kills}/{nextGoal}";

        if (kills >= EnemyHelper.KillThresholds[^1])
        {
            KillCounterSlider.maxValue = 1;
            KillCounterSlider.value = 1;
            KillCounterText.text = $"{kills}/{EnemyHelper.KillThresholds[^1]}";
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            if (level > 0) EnemyName.text = Enemy.NamePT;

            if (level > 1) EnemyLifeTitle.text = "Vida: ";
            if (level > 1) EnemyLife.text = Enemy.StartLife.ToString();
            if (level > 1) EnemyLifeGain.text = "+ " + Enemy.LifeGrowth.ToString();

            if (level > 2) EnemySpeedTitle.text = "Velocidade: ";
            if (level > 2) EnemySpeed.text = Enemy.Speed.ToString();
            if (level > 2) EnemySpeedGain.text = "+ " + Enemy.SpeedGrowth.ToString();

            if (level > 3) EnemyDamageTitle.text = "Dano: ";
            if (level > 3) EnemyDamage.text = Enemy.Damage.ToString();
            if (level > 3) EnemyDamageGain.text = "+ " + Enemy.DamageGrowth.ToString();

            if (level > 4) EnemyAtkSpeedTitle.text = "Vel. Ataque: ";
            if (level > 4) EnemyAtkSpeed.text = Enemy.AttackSpeed.ToString();
            if (level > 4) EnemyAtkSpeedGain.text = "+ " + Enemy.AttackSpeedGrowth.ToString();
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            if (level > 0) EnemyName.text = Enemy.NameEN;

            if (level > 1) EnemyLifeTitle.text = "Life: ";
            if (level > 1) EnemyLife.text = Enemy.StartLife.ToString();
            if (level > 1) EnemyLifeGain.text = "+ " + Enemy.LifeGrowth.ToString();

            if (level > 2) EnemySpeedTitle.text = "Speed: ";
            if (level > 2) EnemySpeed.text = Enemy.Speed.ToString();
            if (level > 2) EnemySpeedGain.text = "+ " + Enemy.SpeedGrowth.ToString();

            if (level > 3) EnemyDamageTitle.text = "Damage: ";
            if (level > 3) EnemyDamage.text = Enemy.Damage.ToString();
            if (level > 3) EnemyDamageGain.text = "+ " + Enemy.DamageGrowth.ToString();

            if (level > 4) EnemyAtkSpeedTitle.text = "Atk. Speed: ";
            if (level > 4) EnemyAtkSpeed.text = Enemy.AttackSpeed.ToString();
            if (level > 4) EnemyAtkSpeedGain.text = "+ " + Enemy.AttackSpeedGrowth.ToString();
        }
    }

    private void ClearContainer()
    {
        foreach (Transform child in SpeciesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearMainContainer()
    {
        foreach (Transform child in TypePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        BestiaryPanel.SetActive(false);
    }
}
