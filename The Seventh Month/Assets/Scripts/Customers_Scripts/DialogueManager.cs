using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel; // assign DialoguePanel
    public TextMeshProUGUI dialogueText; // assign DialogueText
    public float typingSpeed = 0.05f; // speed of typing effect

    private Coroutine typingCoroutine;

    // Start dialogue with text
    public void ShowDialogue(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    // Hide dialogue
    public void HideDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
