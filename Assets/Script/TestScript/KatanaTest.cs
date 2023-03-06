using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class KatanaTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Weapon"))
        {
            Debug.Log("Damaged!");
            Destroy(gameObject); 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Weapon"))
        {
            Debug.Log("Damaged!");
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageCoroutine()
    {
        float elapsed = 0f;
        float waitTime = 1f;

        while (elapsed < waitTime)
        {
            if (Time.timeScale > 0f)
            {
                elapsed += Time.deltaTime;
            }
            else
            {
                elapsed += Time.unscaledDeltaTime;
            }

            yield return null;
        }
    }
}
