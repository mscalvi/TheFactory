using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITickable
{
    void OnTick(float dt);
}

public class TickService : MonoBehaviour
{
    private GameState GameState;

    [SerializeField] float tickInterval = 1f / 30f;

    float timer = 0f;

    bool isPaused = false;

    readonly List<ITickable> tickables = new();
    
    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public void Subscribe(ITickable t)
    {
        if (!tickables.Contains(t))
            tickables.Add(t);
    }

    public void Unsubscribe(ITickable t)
    {
        tickables.Remove(t);
    }

    void Update()
    {
        if (isPaused)
            return;

        float scaledDelta = Time.deltaTime * GameState.ActualGameSpeed;

        timer += scaledDelta;

        while (timer >= tickInterval)
        {
            timer -= tickInterval;

            foreach (var t in tickables)
            {
                t.OnTick(tickInterval);
            }
        }
    }

    // Pause System
    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }
}