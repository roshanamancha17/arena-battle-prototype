using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;

    public void ShowWin()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        if (losePanel != null)
            losePanel.SetActive(true);
    }

    public void NextLevel()
    {
        // Load next scene in build order
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(currentIndex + 1);
        else
            SceneManager.LoadScene(0); // restart if no next level
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
