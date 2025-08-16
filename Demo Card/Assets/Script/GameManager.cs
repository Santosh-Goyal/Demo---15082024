using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;
    private int combo = 0;

    // The prefab of the card.
    [SerializeField] private GameObject cardPrefab;

    // The container for the cards in the scene.
    [SerializeField] private Transform cardContainer;

    // Array of sprites for the card faces, assign these in the Inspector.
    [SerializeField] private Sprite[] cardFaces;

    // Stores the currently flipped cards for comparison.
    private List<Card> flippedCards = new List<Card>();

    [SerializeField] private AudioManager audioManager;
    
    // Tracks if a card comparison is in progress.
    public bool canFlip = true;

    // The number of pairs to create.
    [SerializeField] private int pairsCount = 6;

    private int matchedPairs = 0;

    private void Start()
    {
        LoadGame();
        GenerateCards();
    }

    private void GenerateCards()
    {
        // Check if we have enough card faces to create the pairs.
        if (cardFaces.Length < pairsCount)
        {
            Debug.LogError("Not enough card faces for the number of pairs!");
            return;
        }

        // Create a list of all card IDs. Each ID appears twice.
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < pairsCount; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // Shuffle the list of IDs.
        Shuffle(cardIDs);

        // Instantiate and set up the cards.
        for (int i = 0; i < cardIDs.Count; i++)
        {
            GameObject newCardObject = Instantiate(cardPrefab, cardContainer);
            Card newCard = newCardObject.GetComponent<Card>();
            newCard.SetCard(cardIDs[i], cardFaces[cardIDs[i]], this, audioManager);
        }
    }

    // Fisher-Yates shuffle algorithm.
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Called by the Card script when a card is clicked.
    public void CardClicked(Card card)
    {
        // Flip the card and add it to our list.
        card.Flip();
        flippedCards.Add(card);
        audioManager.PlaySound(audioManager.flipSound);

        // If two cards have been flipped, start the comparison process.
        if (flippedCards.Count == 2)
        {
            canFlip = false;
            StartCoroutine(CheckForMatch());
        }
    }

    // The coroutine to check if the two flipped cards match.
    private IEnumerator CheckForMatch()
    {
        // Wait for a short time to allow the user to see the cards.
        canFlip = false;
        yield return new WaitForSeconds(1.0f);

        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];

        if (card1.cardID == card2.cardID)
        {
            // Cards match.
            Debug.Log("Match!");
            audioManager.PlaySound(audioManager.matchSound);
            combo++;
            score += 100 * combo; // Score with a combo multiplier.
            UpdateScoreDisplay();
            matchedPairs++;

            // Disable matched cards.
            card1.gameObject.SetActive(false);
            card2.gameObject.SetActive(false);

            if (matchedPairs == pairsCount)
            {
                Debug.Log("Game Over!");
                audioManager.BGM();
                audioManager.PlaySound(audioManager.gameOverSound);
                // You can add logic to show a game over screen here.
                SaveGame(); // Save the final score.
            }

        }
        else
        {
            // Cards do not match.
            Debug.Log("Mismatch!");
            combo = 0; // Reset combo on mismatch.
            audioManager.PlaySound(audioManager.mismatchSound);
            card1.Flip(); // Flip them back down.
            card2.Flip();
        }

        // Clear the list for the next pair of cards.
        flippedCards.Clear();
        canFlip = true;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save(); // Save the data to disk.
        Debug.Log("Game Saved!");
    }

    private void LoadGame()
    {
        if (PlayerPrefs.HasKey("PlayerScore"))
        {
            score = PlayerPrefs.GetInt("PlayerScore");
            UpdateScoreDisplay();
            Debug.Log("Game Loaded!");
        }
    }
}