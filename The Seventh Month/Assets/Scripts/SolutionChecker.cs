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
                customerManager.RegisterFailure(customerManager.GetActiveCustomerData());
            }
        }

        customerManager.CustomerDone(customerManager.bufferTime);
    }
}
