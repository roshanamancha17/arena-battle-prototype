using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Base playerBase;
    public Base enemyBase;
    public GameObject winPanel;
    public GameObject losePanel;

    private bool gameEnded = false;

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
            if (winPanel != null) winPanel.SetActive(true);
            Debug.Log("You Win!");
        }
        else
        {
            if (losePanel != null) losePanel.SetActive(true);
            Debug.Log("You Lose!");
        }

        // Optional: freeze gameplay
        Time.timeScale = 0f;
    }
}
