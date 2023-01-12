using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KatanaCollider : MonoBehaviour
{
    [SerializeField]
    private Collider col;

    [SerializeField]
    private UnityEvent regain;

    private void Start()
    {
        col= GetComponent<Collider>();
    }

    public void OnAttack()
    {
        Debug.Log("ColliderOn");
        col.enabled = true;
    }

    public void AttackEnd()
    {
        Debug.Log("ColliderOff");
        col.enabled = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Equals("Enemy"))
        {
            Debug.Log("Katana On Hit Enemy!");
            regain?.Invoke();
        }
    }

}
