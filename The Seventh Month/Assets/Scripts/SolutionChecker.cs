using System.Collections.Generic;
using UnityEngine;
using static CustomerCase;

public class SolutionChecker : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CustomerManager customerManager;

    public AudioSource audioSource;
    public AudioSource lightFlickerAudioSource;
    public AudioClip lightFlickerClip;

    public Light2DFlicker lightFlash;
    public ImageFlicker imageFlicker;


    public void SubmitSolution()
    {
        if (inventoryManager == null || customerManager == null)
        {
            Debug.LogWarning("[SolutionChecker] Missing references!");
            return;
        }

        CustomerCase currentCase = customerManager.GetActiveCase();
        if (currentCase == null)
        {
            Debug.LogWarning("[SolutionChecker] No active customer case.");
            return;
        }

        Debug.Log($"[SolutionChecker] Checking solution for: {currentCase.caseName}");

        List<ItemData> playerItems = inventoryManager.GetCurrentItemsData();
        bool solved = true;

        foreach (ItemData required in currentCase.requiredItems)
        {
            bool itemFound = false;

            foreach (ItemData playerItem in playerItems)
            {
                //  Exact match
                if (playerItem == required)
                {
                    itemFound = true;
                    break;
                }

                //  Flexible rule: if case allows any talisman
                if (currentCase.acceptsAnyTalisman &&
                    playerItem.category == ItemData.ItemCategory.Talisman &&
                    required.category == ItemData.ItemCategory.Talisman)
                {
                    Debug.Log($"[SolutionChecker] {playerItem.itemName} accepted as valid talisman substitute!");
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                solved = false;
                break;
            }
        }

        if (solved)
        {
            Debug.Log($"[SolutionChecker] SUCCESS: {currentCase.successOutcome}");
            // TODO: Trigger success UI/animation here
        }
        else
        {
            Debug.Log($"[SolutionChecker] FAILURE: {currentCase.failureOutcome}");
            if (customerManager != null)
            {
                // Play main fail sound
                if (audioSource != null)
                    audioSource.Play();

                // Play light flicker sound
                if (lightFlickerAudioSource != null && lightFlickerClip != null)
                    lightFlickerAudioSource.PlayOneShot(lightFlickerClip);

                // Trigger light flicker (always)
                if (lightFlash != null)
                    lightFlash.TriggerFailFlicker();
                Debug.Log("Light flicker.");

                // Trigger ARG image flash ONLY for stalker cases
                if (currentCase.caseType == CaseType.Stalker && imageFlicker != null)
                    imageFlicker.TriggerFlicker();

                customerManager.RegisterFailure(customerManager.GetActiveCustomerData());
            }
        }

        customerManager.CustomerDone(customerManager.bufferTime);
        //clear inv
        inventoryManager.ClearInventory();
    }
}
