using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    internal static GameCoordinator instance;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    internal void PrepareGameMode1() => PlatformManager.instance.ShowBottomPlatform();

    internal void PlayerCancelJump()
    {
        TrajectoryManager.instance.DespawnBars();
        PlayerManager.instance.PlayerAnimation("Holding", false);
    }

    internal void UpdateTrajectory(Rigidbody2D rigidbody, Vector2 jumpVector, int minJumpForce, int maxJumpForce) => TrajectoryManager.instance.MakeTrajectory(rigidbody, jumpVector, minJumpForce, maxJumpForce);

    internal void PlayerPrepareJump() => PlayerManager.instance.PlayerAnimation("Holding", true);

    internal void PlayerJump()
    {
        TrajectoryManager.instance.DespawnBars();
        PlatformManager.instance.DespawnPlatform();

        PlayerManager.instance.PlayerAnimation("Flying", true);
        PlayerManager.instance.PlayerAnimation("Holding", false);
    }

    internal void PlayerPlatformCollision()
    {
        PlayerManager.instance.PlayerAnimation("Holding", false);
        PlayerManager.instance.PlayerAnimation("Flying", false);
        PlayerManager.instance.StartPlatformCollision();
    }

    internal void PlayerLand()
    {
        PlatformManager.instance.StartNextPlatformSequence();
        ScoreManager.instance.AddPoints(1);
    }

    internal void GameOver()
    {
        PlayerManager.instance.PlayerAnimation("Flying", false);

        PlatformManager.instance.GameOver();
        ScoreManager.instance.UpdateHighscore();
        ScoreManager.instance.UpdateGameOverScores();
    }

    internal void SecondChance()
    {
        GameOverManager.instance.StartSecondChance();
        PlatformManager.instance.StartSecondChance();
    }

    internal void Retry()
    {
        PlatformManager.instance.ShowBottomPlatform();
        ScoreManager.instance.ResetCurrentScore();
    }
}