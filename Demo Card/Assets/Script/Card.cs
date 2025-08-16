using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    // A reference to the SpriteRenderer or Image component of the card.
    [SerializeField] private Image cardFrontImage;
    [SerializeField] private Image cardBackImage;

    // The unique ID for this card's face.
    public int cardID;

    // Is the card currently face up?
    public bool isFaceUp = false;

    // A reference to the GameManager.
    private GameManager gameManager;
    private AudioManager audioMan;

    private Coroutine flipCoroutine;

    // Public method to set the card's visual identity.
    public void SetCard(int id, Sprite frontSprite, GameManager manager, AudioManager audioManager)
    {
        cardID = id;
        cardFrontImage.sprite = frontSprite;
        cardFrontImage.gameObject.SetActive(false); // Start with the card face down.
        gameManager = manager;
        audioMan = audioManager;
    }

    // This method is called when the user clicks the card.
    public void OnPointerClick(PointerEventData eventData)
    {
        // Don't do anything if the card is already face up or the game state doesn't allow flipping.
        if (isFaceUp || !gameManager.canFlip)
        {
            return;
        }

        // Inform the GameManager that this card was clicked.
        gameManager.CardClicked(this);
    }

    // Public method to flip the card.
    public void Flip()
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
        }
        flipCoroutine = StartCoroutine(FlipAnimation());
    }

    // Coroutine for a smooth flip animation.
    private IEnumerator FlipAnimation()
    {
        isFaceUp = !isFaceUp;
        float duration = 0.25f; // Animation duration in seconds.
        float elapsed = 0f;

        // Loop to rotate the card from 0 to 90 degrees.
        while (elapsed < duration / 2)
        {
            float angle = Mathf.Lerp(0f, 90f, (elapsed / (duration / 2)));
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // At 90 degrees, switch the sprite.
        transform.localRotation = Quaternion.Euler(0, 90, 0);
        cardBackImage.gameObject.SetActive(!isFaceUp);
        cardFrontImage.gameObject.SetActive(isFaceUp);

        elapsed = 0f;
        // Loop to rotate the card from 90 to 180 degrees.
        while (elapsed < duration / 2)
        {
            float angle = Mathf.Lerp(90f, 180f, (elapsed / (duration / 2)));
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0); // Reset the rotation after the flip.

        flipCoroutine = null;
    }
}