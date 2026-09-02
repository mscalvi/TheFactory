using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float HoldTime = 0.5f;

    public UnityEvent OnClick;
    public UnityEvent OnHold;

    private Coroutine HoldCoroutine;
    private bool Holding;
    private bool HasHeld;

    public void OnPointerDown(PointerEventData eventData)
    {
        Holding = true;
        HasHeld = false;

        HoldCoroutine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Holding = false;

        if (HoldCoroutine != null)
        {
            StopCoroutine(HoldCoroutine);
            HoldCoroutine = null;
        }

        if (!HasHeld)
        {
            OnClick?.Invoke();
        }
    }

    private IEnumerator HoldRoutine()
    {
        yield return new WaitForSeconds(HoldTime);

        if (Holding)
        {
            HasHeld = true;
            OnHold?.Invoke();
        }
    }
}