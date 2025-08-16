using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    private void Start()
    {
        bool hasSavedGame = PlayerPrefs.GetInt("HasSavedGame", 0) == 1;
        continueButton.interactable = hasSavedGame;
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("LoadSavedGame", 0);
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("LoadSavedGame", 1);
        SceneManager.LoadScene("GameScene");
    }
}