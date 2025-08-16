using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel, pausePanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameManager gameManager;
    private Animator animator1, animator2;

    private void Start()
    {
        animator1 = gameOverPanel.GetComponent<Animator>();
        animator2 = pausePanel.GetComponent<Animator>();
    }

    public void ShowGameOverUI(int finalScore)
    {
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Final Score: " + finalScore.ToString();
        animator1.Play("GameOver_Emerge");
    }

    public void HideGameOverUI()
    {
        animator1.Play("GameOver_FadeOut");
        gameOverPanel.SetActive(false);
    }

    // Public methods for button events.
    public void OnQuitButtonClicked()
    {
        gameManager.QuitGame();
        Debug.Log("Quit button clicked!");
    }

    public void OnNextButtonClicked()
    {
        Debug.Log("Next button clicked!");
        HideGameOverUI();
        gameManager.RestartGame();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        animator2.Play("GameOver_FadeOut");
        pausePanel.SetActive(false);
        Debug.Log("Game resumed!");
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        animator2.Play("GameOver_Emerge");
        Debug.Log("Game paused!");
    }
}