using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed = 300;

    private Rigidbody rig;
    public ParticleSystem crash;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        rig.AddForce(transform.forward * MoveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals("Weapon"))
        {
            Cutoff();
        }
    }

    private void Cutoff()
    {
        Debug.Log("CutBullet");
        Destroy(gameObject);
    }
}
