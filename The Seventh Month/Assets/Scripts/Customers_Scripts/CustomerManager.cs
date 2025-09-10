using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class CustomerCasePair
{
    public CustomerData customer;
    public CustomerCase customerCase;

    public CustomerCasePair(CustomerData c, CustomerCase cc)
    {
        customer = c;
        customerCase = cc;
    }

}

public class CustomerManager : MonoBehaviour
{
    public CustomerData[] customers;
    public Transform spawnPoint;
    public PhotoPanelManager photoPanelManager;


    public ClockManager clockManager; // Assign in inspector

    private GameObject activeCustomer;
    private CustomerCase activeCase;

    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();


    private int customersServed = 0; // track number of customers
    public int maxCustomers = 5;     // stop after 5

    public float bufferTime = 2f; // default = 2 seconds


    void Start()
    {
        // Fill availablePairs with all possible customer-case combinations
        foreach (var customer in customers)
        {
            foreach (var c in customer.possibleCases)
            {
                availablePairs.Add(new CustomerCasePair(customer, c));
            }

            // 👇 Reset clock to 1 PM at game start
            if (clockManager != null)
            {
                clockManager.ResetClock();
            }
        }
        // Start the cycle automatically
        SpawnRandomCustomer();
    }

    public void SpawnRandomCustomer()
    {
        if (customersServed >= maxCustomers)
        {
            Debug.Log("All customers have been served!");
            return;
        }

        if (availablePairs.Count == 0)
        {
            Debug.Log("All customer-case pairs have been spawned!");
            return;
        }

        // Pick a random pair
        int index = Random.Range(0, availablePairs.Count);
        CustomerCasePair pair = availablePairs[index];
        // Remove the pair so it won't spawn again
        availablePairs.RemoveAt(index);

        activeCase = pair.customerCase;

        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = Instantiate(pair.customer.customerPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"Customer: {activeCustomer.name}, Case: {activeCase.caseName}");


        Sprite[] randomPhotos = GetRandomPhotos(activeCase.evidencePhotos, 3);

        if (photoPanelManager != null)
        {
            photoPanelManager.ShowEvidencePhotos(randomPhotos);
        }
    }


    public void CustomerDone(float bufferTime)
    {
        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = null;
        customersServed++;

        // Advance clock by 1 hour
        if (clockManager != null)
        {
            clockManager.AdvanceHour();
        }


        if (customersServed < maxCustomers)
        {
            StartCoroutine(SpawnNextCustomerAfterDelay(bufferTime));
        }
        else
        {
            Debug.Log("All customers served for today.");
        }
    }

    private IEnumerator SpawnNextCustomerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnRandomCustomer();
    }


    // -----------------------------
    // Public getter for the current active case
    public CustomerCase GetActiveCase()
    {
        return activeCase;
    }

    private Sprite[] GetRandomPhotos(Sprite[] pool, int count)
    {
        List<Sprite> shuffled = new List<Sprite>(pool);

        // Fisher-Yates shuffle
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            Sprite temp = shuffled[i];
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        // Take first N
        int finalCount = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, finalCount).ToArray();
    }
}



