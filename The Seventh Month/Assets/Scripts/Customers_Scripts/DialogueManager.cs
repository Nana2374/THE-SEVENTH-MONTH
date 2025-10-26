using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    [Header("Voice Settings")]
    public AudioClip[] MaleNoises;
    public AudioClip[] FemaleNoises;
    public AudioSource audioSource;

    [Header("Fade Settings")]
    public float fadeOutDuration = 0.3f;

    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;
    private bool isMale = true;

    //  New method: start dialogue using a CustomerCase
    public void ShowDialogue(CustomerCase customerCase, string dialogue)
    {
        isMale = customerCase.caseGender == CustomerCase.gender.Male; // assign to the field, not a local
        ShowDialogue(dialogue, isMale);
    }




    public void ShowDialogue(string text, bool maleVoice = true)
    {
        isMale = maleVoice;


        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StartFadeOut();
        dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        AudioClip[] activeClips = isMale ? MaleNoises : FemaleNoises;

        foreach (char c in text)
        {
            dialogueText.text += c;

            if (!char.IsWhiteSpace(c) && activeClips.Length > 0 && audioSource != null)
            {
                if (!audioSource.isPlaying) // only play if nothing is currently playing
                {
                    audioSource.clip = activeClips[Random.Range(0, activeClips.Length)];
                    audioSource.Play();
                }
            }


            yield return new WaitForSeconds(typingSpeed);
        }

        StartFadeOut();
    }

    private void StartFadeOut()
    {
        if (audioSource == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAudio());
    }

    private IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
