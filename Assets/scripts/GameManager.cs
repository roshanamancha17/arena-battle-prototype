using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Base playerBase;
    public Base enemyBase;
    public GameObject winPanel;
    public GameObject losePanel;

    private bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f; // ensure game runs normally when level starts
    }

    void Update()
    {
        if (gameEnded) return;

        if (playerBase == null)
        {
            GameOver(false);
        }
        else if (enemyBase == null)
        {
            GameOver(true);
        }
    }

    void GameOver(bool playerWon)
    {
        gameEnded = true;

        if (playerWon)
        {
            if (winPanel) winPanel.SetActive(true);
            Debug.Log("You Win!");
        }
        else
        {
            if (losePanel) losePanel.SetActive(true);
            Debug.Log("You Lose!");
        }

        Time.timeScale = 0f; // Pause game
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        // If no more scenes, restart from level 1 or show main menu later
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0; // restart game or change later
        }

        SceneManager.LoadScene(nextScene);
    }
}
