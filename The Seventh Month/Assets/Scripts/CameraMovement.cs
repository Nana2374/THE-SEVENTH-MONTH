using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;   // how fast to move (smooth)

    public Button buttonUp;
    public Button buttonDown;

    // Other UI elements to hide/show when moving camera
    public GameObject[] uiElementsToToggle;

    private Vector3 deskPosition;   // fixed starting pos
    private Vector3 drawerPosition; // fixed lower pos
    private Vector3 targetPosition;

    public float moveAmount = 10f; // how far down to move from desk
    public float uiDelay = 2f; // delay in seconds before showing UI

    void Start()
    {
        deskPosition = transform.position;
        drawerPosition = deskPosition + new Vector3(0, -moveAmount, 0);

        targetPosition = deskPosition;

        if (buttonUp != null) buttonUp.onClick.AddListener(MoveUp);
        if (buttonDown != null) buttonDown.onClick.AddListener(MoveDown);

        if (buttonUp != null) buttonUp.gameObject.SetActive(false);
        if (buttonDown != null) buttonDown.gameObject.SetActive(true);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void MoveDown()
    {
        targetPosition = drawerPosition;

        ToggleUI(false); // hide immediately

        if (buttonUp != null) buttonUp.gameObject.SetActive(true);
        if (buttonDown != null) buttonDown.gameObject.SetActive(false);
    }

    public void MoveUp()
    {
        targetPosition = deskPosition;

        // Show UI with delay
        StartCoroutine(ToggleUIWithDelay(true, uiDelay));

        if (buttonUp != null) buttonUp.gameObject.SetActive(false);
        if (buttonDown != null) buttonDown.gameObject.SetActive(true);
    }

    private void ToggleUI(bool state)
    {
        foreach (GameObject uiElement in uiElementsToToggle)
        {
            if (uiElement != null)
                uiElement.SetActive(state);
        }
    }

    private IEnumerator ToggleUIWithDelay(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject uiElement in uiElementsToToggle)
        {
            if (uiElement != null)
                uiElement.SetActive(state);
        }
    }
}
