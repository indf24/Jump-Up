using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private GameObject ball;

    private void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(Restart()));
        menuButton.onClick.AddListener(() => SceneControl.LoadScene("MainMenu"));
    }

    private IEnumerator GameOverScreen()
    {
        TurnPlayerObjectIntoUI();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MovePlayerObject(new(0f, 15f)));
        yield return StartCoroutine(ScalePlayerObject(new(15f, 15f, 15f)));
        gameOverButtons.SetActive(true);
    }

    private IEnumerator Restart()
    {
        gameOverButtons.SetActive(false);
        yield return StartCoroutine(ScalePlayerObject(new(1f, 1f, 1f)));
        EventHub.Restart();
        yield return StartCoroutine(MovePlayerObject(new(0f, 6.5f)));
        SceneControl.ReloadScene();
    }

    private void TurnPlayerObjectIntoUI()
    {
        ball.GetComponent<Rigidbody2D>().simulated = false;
        ball.GetComponent<CircleCollider2D>().enabled = false;
    }

    private IEnumerator MovePlayerObject(Vector2 targetPos)
    {
        Vector2 startPos = ball.transform.position;
        float easeTime = 0.7f;

        float elapsedTime = 0;
        while (elapsedTime < easeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / easeTime; // Normalized progress (0 to 1)
            float easeT = 1 - Mathf.Pow(1 - t, 3); // Quadratic ease-out
            ball.transform.position = Vector2.Lerp(startPos, targetPos, easeT);
            yield return null;
        }

        // Ensure exact target position at the end
        ball.transform.position = targetPos;
    }

    private IEnumerator ScalePlayerObject(Vector3 targetScale)
    {
        Vector3 startScale = ball.transform.localScale;
        float easeTime = 0.7f;

        float elapsedTime = 0;
        while (elapsedTime < easeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / easeTime; // Normalized progress (0 to 1)
            float easeT = 1 - Mathf.Pow(1 - t, 3); // Quadratic ease-out
            ball.transform.localScale = Vector3.Lerp(startScale, targetScale, easeT);
            yield return null;
        }

        // Ensure exact target position at the end
        ball.transform.localScale = targetScale;
    }

    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventHub.PlayerAnimation("Flying", false);
        EventHub.GameOver();
        StartCoroutine(GameOverScreen());    
    }

}
