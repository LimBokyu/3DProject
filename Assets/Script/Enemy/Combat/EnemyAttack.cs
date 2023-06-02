using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WeaponType { pistol, rifle }
public class EnemyAttack : MonoBehaviour
{
    private int ammo;
    private Coroutine reload = null;
    private Vector3 dir;
    private WeaponType type;

    public Transform muzzle;

    public GameObject bullet;
    public ParticleSystem gunFlash;

    [SerializeField]
    private float reloadtimer = 0;

    public AudioSource gunShot;

    public void AttackBehaviour()
    {
        Attack();
    }

    private void Attack()
    {
        if (ammo == 0)
        {
            Reload();
        }
    }

    private Quaternion ApplyRecoil()
    {
        dir = new Vector3(transform.rotation.x + Random.Range(0f, 3f),
                          transform.rotation.y + Random.Range(0f, 3f),
                          transform.rotation.z);
        return Quaternion.Euler(dir);
    }

    public void Shoot()
    {
        gunFlash.Play();
        gunShot.Play();
        Instantiate(bullet, muzzle.position, ApplyRecoil());
        ammo -= 1;
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
