using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class CriticalView : MonoBehaviour
{
    private GameState GameState;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float riseDistance = 0.3f;
    [SerializeField] private float duration = 0.3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    [SerializeField] private Vector3 incomePosition = new Vector3(0f, 1.0425f, 0f);

    public void Setup(Vector3 enemyPoint, GameState game)
    {
        GameState = game;
        startPosition = enemyPoint;

        targetPosition = startPosition + Vector3.up * riseDistance;

        transform.position = startPosition;

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            valueText.text = "Crítico!";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            valueText.text = "Critical!";
        }

        StartCoroutine(AnimateCritical());
    }

    private IEnumerator AnimateCritical()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}