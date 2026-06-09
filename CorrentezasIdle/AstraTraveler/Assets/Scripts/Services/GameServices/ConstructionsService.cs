using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionsService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    void Update()
    {
        long now = GetNow();

        var toFinish = new List<ConstructionInstance>();

        foreach (var acq in GameState.CompanyState.ActiveConstructions)
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

    void OnConstructionBuy(ConstructionInstance acq)
    {
        if (acq.IsRunning) return;

        long now = GetNow();

        if (GameState.CompanyState.ActiveConstructions.Count < GameState.CompanyState.MaxConstructionsSlots)
        {
            StartRunning(acq, now);
            GameState.CompanyState.ActiveConstructions.Add(acq);
        }
        else
        {
            if (GameState.CompanyState.ConstructionsQueue.Count < GameState.CompanyState.MaxConstructionsQueue)
            {
                acq.IsRunning = false;
                GameState.CompanyState.ConstructionsQueue.Enqueue(acq);
            }
            else
            {
                Debug.LogError("ConstructionsService - Sem slots e sem fila!");
            }
        }
    }

    void UpdateProgress(ConstructionInstance acq)
    {
        if (!acq.IsRunning) return;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        double total = acq.ActualTime;
        double remaining = acq.FinishTimestamp - now;
        double elapsed = total - remaining;

        float progress = (float)(elapsed / total);

        GameEvents.OnConstructionProgress?.Invoke(acq, progress, remaining);
    }

    void Finish(ConstructionInstance acq)
    {
        acq.IsRunning = false;
        acq.ElapsedTime = acq.ActualTime;

        GameState.CompanyState.ActiveConstructions.Remove(acq);

        acq.Level++;
        acq.StartCost *= 1.65;
        acq.StartTime *= 1.5f;

        GameEvents.OnConstructionFinished?.Invoke(acq);
    }

    public void ResolveOfflineProgress()
    {
        long now = GetNow();

        var toFinish = new List<ConstructionInstance>();

        foreach (var acq in GameState.CompanyState.ActiveConstructions)
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
            GameState.CompanyState.ActiveConstructions.Count < GameState.CompanyState.MaxConstructionsSlots &&
            GameState.CompanyState.ConstructionsQueue.Count > 0
        )
        {
            var next = GameState.CompanyState.ConstructionsQueue.Dequeue();

            StartRunning(next, now);

            GameState.CompanyState.ActiveConstructions.Add(next);
        }
    }

    void StartRunning(ConstructionInstance acq, long now)
    {
        acq.IsRunning = true;

        acq.StartTimestamp = now;
        acq.FinishTimestamp = now + (long)Math.Ceiling(acq.ActualTime);

        GameEvents.OnConstructionStarted?.Invoke(acq);
    }

    long GetNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    void OnEnable()
    {
        GameEvents.OnConstructionBuy += OnConstructionBuy;
        GameEvents.OnGameLoad += ResolveOfflineProgress;
    }

    void OnDisable()
    {
        GameEvents.OnConstructionBuy -= OnConstructionBuy;
        GameEvents.OnGameLoad -= ResolveOfflineProgress;
    }
}
