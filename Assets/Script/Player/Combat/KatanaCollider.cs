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

    public bool blademode;
    private float damage { get; set; }

    [SerializeField]
    private Transform effect;
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
            effect.position = collision.transform.position;
            particle.Play();
        }
    }

}
