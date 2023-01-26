using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed = 20;

    private Rigidbody rig;

    private Coroutine Timer;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        //rig.AddForce(transform.forward * MoveSpeed, ForceMode.Impulse);
        rig.velocity = transform.forward * MoveSpeed;
        Timer = null;
    }

    private void Update()
    {
        //transform.Translate(transform.forward * MoveSpeed * Time.deltaTime);
        if (Timer == null)
            Timer = StartCoroutine(DestroyTimer());
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals("Weapon"))
        {
            Cutoff();
        }
        else if(other.transform.gameObject.layer.Equals(0)
             || other.transform.gameObject.layer.Equals(9))
        {
            Destroy(gameObject);
        }
    }

    private void Cutoff()
    {
        Debug.Log("CutBullet");
        Destroy(gameObject);
    }
}
