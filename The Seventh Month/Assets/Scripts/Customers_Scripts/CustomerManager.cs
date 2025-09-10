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

    private GameObject activeCustomer;
    private CustomerCase activeCase;

    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();

    void Start()
    {
        // Fill availablePairs with all possible customer-case combinations
        foreach (var customer in customers)
        {
            foreach (var c in customer.possibleCases)
            {
                availablePairs.Add(new CustomerCasePair(customer, c));
            }
        }
    }

    public void SpawnRandomCustomer()
    {
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



