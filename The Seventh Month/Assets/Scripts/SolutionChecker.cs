using System.Collections.Generic;
using UnityEngine;

public class SolutionChecker : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CustomerManager customerManager;

    public void SubmitSolution()
    {
        // --- Safety checks ---
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager reference is missing in SolutionChecker!");
            return;
        }

        if (customerManager == null)
        {
            Debug.LogWarning("CustomerManager reference is missing in SolutionChecker!");
            return;
        }

        // --- Get the current active customer case ---
        CustomerCase currentCase = customerManager.GetActiveCase();
        if (currentCase == null)
        {
            Debug.LogWarning("No active customer case. Make sure a customer has been spawned!");
            return;
        }

        Debug.Log("Submitting solution for customer case: " + currentCase.caseName);

        // --- Get items in inventory ---
        List<ItemData> playerItems = inventoryManager.GetCurrentItemsData();

        // --- Check if all required items are present ---
        bool solved = true;
        foreach (ItemData required in currentCase.requiredItems)
        {
            if (!playerItems.Contains(required))
            {
                solved = false;
                break;
            }
        }

        // --- Show outcome ---
        if (solved)
        {
            Debug.Log("Success! " + currentCase.successOutcome);
            // TODO: Trigger success UI here
        }
        else
        {
            Debug.Log("Failure... " + currentCase.failureOutcome);
            // TODO: Trigger failure UI here
        }
    }
}

