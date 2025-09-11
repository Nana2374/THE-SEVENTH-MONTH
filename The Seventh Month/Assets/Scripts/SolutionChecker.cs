using System.Collections.Generic;
using UnityEngine;

public class SolutionChecker : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CustomerManager customerManager;
    public FailurePosterManager failurePosterManager; // must be linked in Inspector


    public void SubmitSolution()
    {
        // --- Safety checks ---
        if (inventoryManager == null || customerManager == null)
        {
            Debug.LogWarning("Missing references!");
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

            // Tell CustomerManager about the failure
            if (customerManager != null && customerManager.GetActiveCase() != null)
            {
                CustomerData failedCustomer = customerManager.GetActiveCustomerData();
                customerManager.RegisterFailure(failedCustomer);
            }
        }
        customerManager.CustomerDone(customerManager.bufferTime);
    }
}

