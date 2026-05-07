using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AcquisitonsService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    void Update()
    {
        long now = GetNow();

        var toFinish = new List<AcquisitionInstance>();

        foreach (var acq in GameState.CompanyState.ActiveAcquisitons)
        {
            if (!acq.IsRunning) continue;

            if (now >= acq.FinishTimestamp)
            {
                toFinish.Add(acq);
            }
            else
            {
                UpdateProgress(acq);
            }
        }

        foreach (var acq in toFinish)
        {
            Finish(acq);
        }

        FillSlotsFromQueue(now);
    }

    void OnAcquisitionBuy(AcquisitionInstance acq)
    {
        if (acq.IsRunning) return;

        long now = GetNow();

        if (GameState.CompanyState.ActiveAcquisitons.Count < GameState.CompanyState.MaxAcquisitionsSlots)
        {
            StartRunning(acq, now);
            GameState.CompanyState.ActiveAcquisitons.Add(acq);
        }
        else
        {
            if (GameState.CompanyState.AcquisitionsQueue.Count < GameState.CompanyState.MaxAcquisitonsQueue)
            {
                acq.IsRunning = false;
                GameState.CompanyState.AcquisitionsQueue.Enqueue(acq);
            }
            else
            {
                Debug.LogError("AcquisitionsService - Sem slots e sem fila!");
            }
        }
    }

    void UpdateProgress(AcquisitionInstance acq)
    {
        if (!acq.IsRunning) return;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        double total = acq.TotalTime;
        double remaining = acq.FinishTimestamp - now;
        double elapsed = total - remaining;

        float progress = (float)(elapsed / total);

        GameEvents.OnAcquisitionProgress?.Invoke(acq, progress, remaining);
    }

    void Finish(AcquisitionInstance acq)
    {
        acq.IsRunning = false;
        acq.ElapsedTime = acq.TotalTime;

        acq.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;

        GameEvents.OnAcquisitionFinished?.Invoke(acq);

        GameState.CompanyState.ActiveAcquisitons.Remove(acq);
    }

    public void ResolveOfflineProgress()
    {
        long now = GetNow();

        var toFinish = new List<AcquisitionInstance>();

        foreach (var acq in GameState.CompanyState.ActiveAcquisitons)
        {
            if (now >= acq.FinishTimestamp)
            {
                toFinish.Add(acq);
            }
        }

        foreach (var acq in toFinish)
        {
            Finish(acq);
        }

        FillSlotsFromQueue(now);
    }

    void FillSlotsFromQueue(long now)
    {
        while (
            GameState.CompanyState.ActiveAcquisitons.Count < GameState.CompanyState.MaxAcquisitionsSlots &&
            GameState.CompanyState.AcquisitionsQueue.Count > 0
        )
        {
            var next = GameState.CompanyState.AcquisitionsQueue.Dequeue();

            StartRunning(next, now);

            GameState.CompanyState.ActiveAcquisitons.Add(next);
        }
    }

    void StartRunning(AcquisitionInstance acq, long now)
    {
        acq.IsRunning = true;

        acq.StartTimestamp = now;
        acq.FinishTimestamp = now + (long)Math.Ceiling(acq.TotalTime);

        GameEvents.OnAcquisitionStarted?.Invoke(acq);
    }

    long GetNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    void OnEnable()
    {
        GameEvents.OnAcquisitionBuy += OnAcquisitionBuy;
        GameEvents.OnGameLoad += ResolveOfflineProgress;
    }

    void OnDisable()
    {
        GameEvents.OnAcquisitionBuy -= OnAcquisitionBuy;
        GameEvents.OnGameLoad -= ResolveOfflineProgress;
    }
}
