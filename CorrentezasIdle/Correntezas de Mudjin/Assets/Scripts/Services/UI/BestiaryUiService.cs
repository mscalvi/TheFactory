using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryUiService : MonoBehaviour
{
    private GameState GameState;

    [SerializeField] Transform BestiaryPanel;
    [SerializeField] BestiaryEntryDefinition BestiaryPrefab;

    public void Initialize(GameState game)
    {
        GameState = game;

        PopulateBestiary();
    }

    private void PopulateBestiary()
    {
        ClearContainer();

        foreach (var entry in GameState.BestiaryState.Bestiary)
        {
            var enemy = GameState.DataState.enemies.GetValueOrDefault(entry.Key);

            if (enemy == null)
                continue;

            var go = Instantiate(BestiaryPrefab, BestiaryPanel);
            var ui = go.GetComponent<BestiaryEntryDefinition>();

            ui.Setup(enemy, entry.Value);
        }
    }

    private void ClearContainer()
    {
        foreach (Transform child in BestiaryPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
