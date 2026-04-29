using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryEntryDefinition : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text TotalKills;
    public TMP_Text LastKills;
    public Image Icon;

    public void Setup(EnemyInstance enemy, BestiaryEntry entry)
    {
        if (enemy == null || entry == null)
            return;

        Name.text = enemy.Name;

        TotalKills.text = $"Total: {entry.KilledTotal:N0}";
        LastKills.text = $"Run: {entry.KilledLastExpedition:N0}";

        // Se você tiver sprite:
        //if (enemy.Icon != null)
        //    Icon.sprite = enemy.Icon;
    }

}
