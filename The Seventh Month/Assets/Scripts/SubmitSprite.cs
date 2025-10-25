using UnityEngine;

public class SubmitSprite : MonoBehaviour
{
    public SolutionChecker solutionChecker; // assign in inspector
    public CameraMovement cameraMovement;   // assign in inspector

    public AudioClip bellSound;
    public AudioSource audioSource;   // assign in Inspector

    void OnMouseDown()
    {
        if (solutionChecker != null)
        {
            solutionChecker.SubmitSolution();
            audioSource.PlayOneShot(bellSound);
            Debug.Log("Submit sprite clicked!");
        }

        if (cameraMovement != null)
        {
            cameraMovement.MoveUp(); // pan camera back to desk
        }
    }
}
