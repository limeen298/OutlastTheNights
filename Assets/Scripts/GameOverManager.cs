
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public PlayerStats playerStats;

    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver && playerStats.health <= 0f)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        // Liberer la souris pendant le Game Over
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}


