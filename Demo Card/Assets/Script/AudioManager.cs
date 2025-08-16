using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource1, audioSource2;
    [SerializeField] public AudioClip flipSound;
    [SerializeField] public AudioClip matchSound;
    [SerializeField] public AudioClip mismatchSound;
    [SerializeField] public AudioClip gameOverSound;

    public void PlaySound(AudioClip clip)
    {
        if (audioSource1 != null && clip != null)
        {
            audioSource1.PlayOneShot(clip);
        }
    }

    public void BGM()
    {
        if (audioSource1 != null)
        {
            audioSource2.loop = false;
            audioSource2.Pause();
        }
    }

    public void Restart()
    {
        if (audioSource2 != null)
        {
            audioSource2.loop = true;
            audioSource2.Play();
        }
    }
}
