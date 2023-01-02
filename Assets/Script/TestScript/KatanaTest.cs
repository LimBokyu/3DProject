using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class KatanaTest : MonoBehaviour
{
    //Collider col;

    private void Start()
    {
        //col =GetComponentInChildren<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Weapon"))
        {
            Debug.Log("Damaged!");
            Destroy(gameObject); 
        }
    }
}
