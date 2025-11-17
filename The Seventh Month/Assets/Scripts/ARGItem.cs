using UnityEngine;

public class ARGItem : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        if (animator != null)
            animator.SetTrigger("Play");

        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}