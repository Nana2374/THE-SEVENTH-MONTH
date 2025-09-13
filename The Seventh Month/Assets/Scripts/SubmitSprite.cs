using UnityEngine;

public class SubmitSprite : MonoBehaviour
{
    public SolutionChecker solutionChecker; // assign in inspector
    public CameraMovement cameraMovement;   // assign in inspector

    void OnMouseDown()
    {
        if (solutionChecker != null)
        {
            solutionChecker.SubmitSolution();
            Debug.Log("Submit sprite clicked!");
        }

        if (cameraMovement != null)
        {
            cameraMovement.MoveUp(); // pan camera back to desk
        }
    }
}
