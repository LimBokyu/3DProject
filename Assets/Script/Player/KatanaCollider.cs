using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaCollider : MonoBehaviour
{
    [SerializeField]
    private Collider col;

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

}
