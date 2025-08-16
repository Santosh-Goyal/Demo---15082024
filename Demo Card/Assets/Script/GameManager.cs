using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] private Vector2 cardSize = new Vector2(100, 150);
    [SerializeField] private float cardSpacing = 100f;

    [Space(20)]
    [Header("Dynamic Variable")]
    private int pairsCount;
    private int columns;
    private int rows;

    [Space(20)]
    [Header("Card Reference")]
    [SerializeField] private GameObject cardPrefab; // The prefab of the card.
    [SerializeField] private Transform cardContainer; // The container for the cards in the scene.

    [Space(20)]
    [Header("Card Faces Pool")]
    [SerializeField] private Sprite[] allCardFaces; // Array of sprites for the card faces, assign these in the Inspector.
    [SerializeField] private int maxPairs = 16; // The max number of pairs for a game.

    [Space(20)]
    [Header("UI and Sound")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioManager audioManager;

    [Space(20)]
    [Header("Game State Variables")]
    private List<Card> flippedCards = new List<Card>(); // Stores the currently flipped cards for comparison.
    private List<Card> allCards = new List<Card>(); // This will hold all instantiated cards.
    private int score = 0;
    private int combo = 0;
    public bool canFlip = false; // Tracks if a card comparison is in progress.
    private int matchedPairs = 0;

    private void Start()
    {
        LoadGame();
        CalculateLayout();
        GenerateCards();
        StartCoroutine(ShowAndHideCards());
    }

    // Dynamically calculates the number of pairs and grid layout.
    private void CalculateLayout()
    {
        // Choose a random number of pairs between 2 and the maxPairs.
        // Make sure it's an even number of cards.
        int totalCards;
        do
        {
            pairsCount = Random.Range(2, maxPairs + 1);
            totalCards = pairsCount * 2;
        } while (totalCards % 2 != 0 || totalCards > allCardFaces.Length * 2);

        // Find a balanced grid layout (rows x columns) for the total number of cards.
        columns = 1;
        rows = 1;

        for (int i = 1; i <= Mathf.Sqrt(totalCards); i++)
        {
            if (totalCards % i == 0)
            {
                rows = i;
                columns = totalCards / i;
            }
        }

        // Ensure columns are greater than or equal to rows for a horizontal layout.
        if (columns < rows)
        {
            int temp = columns;
            columns = rows;
            rows = temp;
        }

        Debug.Log("Generated a new game with " + pairsCount + " pairs. Grid is " + columns + "x" + rows);
    }

    private void GenerateCards()
    {
        // Select a random set of card faces from the pool.
        List<Sprite> selectedFaces = new List<Sprite>();
        List<int> faceIndices = new List<int>();

        for (int i = 0; i < allCardFaces.Length; i++)
        {
            faceIndices.Add(i);
        }

        Shuffle(faceIndices);

        for (int i = 0; i < pairsCount; i++)
        {
            selectedFaces.Add(allCardFaces[faceIndices[i]]);
        }

        // Create a list of all card IDs based on the selected faces.
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < pairsCount; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // Shuffle the list of IDs.
        Shuffle(cardIDs);

        // Calculate the size of the entire grid
        float gridWidth = columns * cardSize.x + (columns - 1) * cardSpacing;
        float gridHeight = rows * cardSize.y + (rows - 1) * cardSpacing;

        // Calculate the top-left corner offset to center the grid
        Vector2 startPos = new Vector2(-gridWidth / 2 + cardSize.x / 2, gridHeight / 2 - cardSize.y / 2);

        // Instantiate and set up the cards.
        for (int i = 0; i < cardIDs.Count; i++)
        {
            GameObject newCardObject = Instantiate(cardPrefab, cardContainer);
            Card newCard = newCardObject.GetComponent<Card>();
            newCard.SetCard(cardIDs[i], selectedFaces[cardIDs[i]], this, audioManager);
            allCards.Add(newCard);

            // Calculate the card's position in the grid
            int row = i / columns;
            int col = i % columns;

            float posX = startPos.x + col * (cardSize.x + cardSpacing);
            float posY = startPos.y - row * (cardSize.y + cardSpacing);

            newCardObject.transform.localPosition = new Vector3(posX, posY, 0);
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

    private IEnumerator ShowAndHideCards()
    {
        // Set all cards face up at the beginning.
        foreach (Card card in allCards)
        {
            card.Flip();
        }

        // Wait for a few seconds so the player can see them.
        yield return new WaitForSeconds(3.0f);

        // Flip all cards back down to start the game.
        foreach (Card card in allCards)
        {
            card.Flip();
        }

        // Now, allow the player to interact with the cards.
        canFlip = true;
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