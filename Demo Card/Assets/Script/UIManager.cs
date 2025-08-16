using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameManager gameManager;
    private Animator animator;

    private void Start()
    {
        animator = gameOverPanel.GetComponent<Animator>();
    }

    public void ShowGameOverUI(int finalScore)
    {
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Final Score: " + finalScore.ToString();
        animator.Play("GameOver_Emerge");
    }

    public void HideGameOverUI()
    {
        animator.Play("GameOver_FadeOut");
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
}