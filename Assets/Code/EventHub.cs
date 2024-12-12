using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class EventHub : MonoBehaviour
{
    public static event Action OnPlayerLanding;
    public static event Action<int> OnScoreEarned;

    public static event Action OnGameOver;

    public static event Action OnPlatformCollision;

    public static event Action<string, bool> RunPlayerAnimation;
    public static event Action OnPlayerJump;

    public static void PlayerLand(int points)
    {
        OnPlayerLanding?.Invoke();
        OnScoreEarned?.Invoke(points);
    }

    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void PlatformCollision()
    {
        OnPlatformCollision?.Invoke();
    }

    public static void PlayerAnimation(string animationId, bool state)
    {
        RunPlayerAnimation?.Invoke(animationId, state);
    }

    public static void PlayerJump()
    {
        OnPlayerJump?.Invoke();
    }
}
