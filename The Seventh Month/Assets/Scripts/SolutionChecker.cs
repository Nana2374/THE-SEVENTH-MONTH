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
        List<ItemData> requiredItems = new List<ItemData>(currentCase.requiredItems);


        bool solved = true;

        // --- STEP 1: Extra items check ---
        // If the player picked more items than required → FAIL immediately
        if (playerItems.Count != requiredItems.Count)
        {
            solved = false;
        }
        else
        {
            // --- STEP 2: Item-by-item match ---
            foreach (ItemData required in requiredItems)
            {
                bool foundMatch = false;

                foreach (ItemData playerItem in playerItems)
                {
                    // Exact match
                    if (playerItem == required)
                    {
                        foundMatch = true;
                        break;
                    }

                    // Flexible rule: accept ANY talisman
                    if (currentCase.acceptsAnyTalisman &&
                        playerItem.category == ItemData.ItemCategory.Talisman &&
                        required.category == ItemData.ItemCategory.Talisman)
                    {
                        Debug.Log($"[SolutionChecker] {playerItem.itemName} accepted as valid talisman substitute!");
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    solved = false;
                    break;
                }
            }
        }

        // ---------------------------------------
        // FINAL RESULT
        // ---------------------------------------
        if (solved)
        {
            Debug.Log($"[SolutionChecker] SUCCESS: {currentCase.successOutcome}");
            // TODO add your success handling
        }
        else
        {
            Debug.Log($"[SolutionChecker] FAILURE: {currentCase.failureOutcome}");

            // Play main fail sound
            audioSource?.Play();

            // Light flicker audio
            if (lightFlickerAudioSource != null && lightFlickerClip != null)
                lightFlickerAudioSource.PlayOneShot(lightFlickerClip);

            // Light flicker effect
            lightFlash?.TriggerFailFlicker();
            Debug.Log("Light flicker.");

            // ARG image flash only for stalker cases
            if (currentCase.caseType == CaseType.Stalker)
                imageFlicker?.TriggerFlicker();

            customerManager.RegisterFailure(customerManager.GetActiveCustomerData());
        }

        // Finish day
        customerManager.CustomerDone(customerManager.bufferTime);

        // Clear inventory
        inventoryManager.ClearInventory();
    }
}
