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

    [SerializeField]
    private ParticleSystem particle;
    private void Start()
    {
        col= GetComponent<Collider>();
    }

    public void OnAttack()
    {
        col.enabled = true;
    }

    public void AttackEnd()
    {
        col.enabled = false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Equals("Enemy"))
        {
            Debug.Log("Katana On Hit Enemy!");
            regain?.Invoke();
        }

        if(collision.tag.Equals("Bullet"))
        {
            Debug.Log("Particle Play");
            Transform trans = collision.transform;
            particle.transform.position = trans.position;
            particle.Play();
        }
    }

}
