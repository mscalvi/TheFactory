using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class CurrencyView : MonoBehaviour
{
    public CurrencyInstance Currency;
    public Transform IncomePoint;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private UnityEngine.UI.Image currencyIcon;
    [SerializeField] private float riseDistance = 0.5f;
    [SerializeField] private float duration = 0.6f;

    private SpriteRenderer spriteRenderer;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    [SerializeField] private Vector3 incomePosition = new Vector3(0f, 1.0425f, 0f);

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(CurrencyInstance currency, double amount, Transform incomePoint)
    {
        Currency = currency;
        IncomePoint = incomePoint;

        ApplySprite();
        ApplyValue(amount);

        startPosition = IncomePoint.position;
        targetPosition = startPosition + Vector3.up * riseDistance;

        transform.position = startPosition;

        StartCoroutine(AnimateIncome());
    }

    private void ApplyValue(double amount)
    {
        int fixedAmount = (int)Math.Floor(amount);

        valueText.text = $"+{fixedAmount}";
    }

    private IEnumerator AnimateIncome()
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

    private void ApplySprite()
    {
        Sprite sprite = Resources.Load<Sprite>(
            $"Sprites/Currencies/{Currency.Id}"
        );

        if (sprite != null)
            currencyIcon.sprite = sprite;
    }
}