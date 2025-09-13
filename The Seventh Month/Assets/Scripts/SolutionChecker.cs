using System.Collections.Generic;
using UnityEngine;

public class SolutionChecker : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CustomerManager customerManager;

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
            if (!playerItems.Contains(required))
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
            //  Delegate to CustomerManager so it decides whether to show poster
            if (customerManager != null)
            {
                customerManager.RegisterFailure(
                    customerManager.GetActiveCustomerData(), // <- New helper to get current CustomerData
                    currentCase.failurePoster
                );
            }
        }

        customerManager.CustomerDone(customerManager.bufferTime);
    }
}

