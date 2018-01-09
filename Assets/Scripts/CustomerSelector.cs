using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSelector : MonoBehaviour {
    public Customer[] customers;

    private int customerIndex = -1; // start at -1 because SelectNextCustomer

    public Customer SelectRandomCustomer()
    {
        int i = Random.Range(0, customers.Length);
        Debug.Log("Selected customer: " + customers[i].name);
        return customers[i];
    }

    public Customer SelectNextCustomer()
    {
        customerIndex++;
        if (customerIndex >= customers.Length)
            return null;
        return customers[customerIndex];
    }
}
