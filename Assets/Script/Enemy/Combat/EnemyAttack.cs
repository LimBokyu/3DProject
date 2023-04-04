using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private int ammo;
    private Coroutine shotBullet;
    private Coroutine reload = null;

    public Transform muzzle;

    public GameObject bullet;
    public ParticleSystem gunFlash;

    [SerializeField]
    private float reloadtimer = 0;

    public AudioSource gunShot;


    public void AttackBehaviour()
    {

    }

    private void Attack()
    {
        if (ammo == 0)
        {
            Reload();
        }
        else
        {

            if (shotBullet == null)
                shotBullet = StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.98f);
        gunFlash.Play();
        gunShot.Play();
        Instantiate(bullet, muzzle.position, transform.rotation);
        ammo -= 1;
        shotBullet = null;
    }

    private void Reload()
    {
        if (reload == null)
            reload = StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadtimer);
        ammo = 7;
        reload = null;
    }
}
