using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public float moveAmount = 10f; // how much to move up/down
    public float moveSpeed = 5f;   // how fast to move (smooth)

    public Button buttonUp;
    public Button buttonDown;

    // Other UI elements to hide/show when moving camera
    public GameObject[] uiElementsToToggle;

    private Vector3 targetPosition;

    void Start()
    {
        // Start at current position
        targetPosition = transform.position;

        if (buttonUp != null) buttonUp.onClick.AddListener(MoveUp);
        if (buttonDown != null) buttonDown.onClick.AddListener(MoveDown);

        // Start: show Down button, hide Up
        if (buttonUp != null) buttonUp.gameObject.SetActive(false);
        if (buttonDown != null) buttonDown.gameObject.SetActive(true);
    }

    void Update()
    {
        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void MoveDown()
    {
        targetPosition = transform.position + new Vector3(0, -moveAmount, 0);

        ToggleUI(false);

        // Switch button visibility
        if (buttonUp != null) buttonUp.gameObject.SetActive(true);
        if (buttonDown != null) buttonDown.gameObject.SetActive(false);
    }

    public void MoveUp()
    {
        targetPosition = transform.position + new Vector3(0, moveAmount, 0);

        ToggleUI(true);

        // Switch button visibility
        if (buttonUp != null) buttonUp.gameObject.SetActive(false);
        if (buttonDown != null) buttonDown.gameObject.SetActive(true);
    }

    private void ToggleUI(bool state)
    {
        foreach (GameObject uiElement in uiElementsToToggle)
        {
            if (uiElement != null) uiElement.SetActive(state);
        }
    }
}