using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITickable
{
    void OnTick(float dt);
}

public class TickService : MonoBehaviour
{
    [SerializeField] float tickInterval = 0.2f;

    float timer = 0f;

    bool isPaused = false;

    readonly List<ITickable> tickables = new();
    
    public void Initialize()
    {
        Debug.Log("TickService On");
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

        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            float dt = timer;
            timer = 0f;

            foreach (var t in tickables)
            {
                t.OnTick(dt);
            }
        }
    }

    // Pause System
    public void Pause()
    {
        isPaused = true;
        Debug.Log("TickService Paused");
    }

    public void Resume()
    {
        Debug.Log("TickService Retomado");
        isPaused = false;
    }
}