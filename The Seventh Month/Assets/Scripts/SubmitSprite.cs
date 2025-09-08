using UnityEngine;

public class SubmitSprite : MonoBehaviour
{
    public SolutionChecker solutionChecker; // assign in inspector

    void OnMouseDown()
    {
        if (solutionChecker != null)
        {
            solutionChecker.SubmitSolution();
            Debug.Log("Submit sprite clicked!");
        }
    }
}
