using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionController : MonoBehaviour
{
    [Header("Instruction Panels (in order)")]
    public GameObject[] instructionPanels;

    [Header("Next Scene Name")]
    public string nextSceneName = "Loading";

    private int currentIndex = 0;
    private bool waitingForClick = true;  // prevents skipping panels

    void Start()
    {
        // Hide all panels at start
        foreach (var panel in instructionPanels)
            panel.SetActive(false);

        // Show first panel
        if (instructionPanels.Length > 0)
            instructionPanels[0].SetActive(true);
    }

    void Update()
    {
        // Wait for click and only advance one panel per click
        if (waitingForClick && Input.GetMouseButtonUp(0))
        {
            waitingForClick = false; // lock until next frame
            NextPanel();
        }
    }

    private void NextPanel()
    {
        currentIndex++;

        if (currentIndex < instructionPanels.Length)
        {
            // Hide all, show only current
            for (int i = 0; i < instructionPanels.Length; i++)
                instructionPanels[i].SetActive(i == currentIndex);

            waitingForClick = true; // ready for next click
        }
        else
        {
            // All panels done, load next scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
