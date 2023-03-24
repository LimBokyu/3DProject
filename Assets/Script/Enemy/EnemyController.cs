using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState { Idle, Attack, Alert, Patrol, MoveBack, Dead, Search, Move }

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int m_HP;

    [SerializeField] private EnemyState state;

    private Rigidbody rigid;
    private Collider collider;

    [SerializeField]
    private Vector3 FirstPosition;
    private Vector3 firstlookat;
    private NavMeshAgent nav;
    public Animator anim;

    private Coroutine shotBullet;
    private Coroutine MoveBackCoroutine;
    private Coroutine reload = null;

    private EnemyView view;

    public Transform muzzle;

    public GameObject Bullet;
    public ParticleSystem gunFlash;
    public AudioSource gunShot;

    private Executions executions;

    [SerializeField]
    private Transform Target;

    [SerializeField]
    private LayerMask alliesMask;

    [SerializeField]
    private int Ammo;

    private Vector3 dir;
    private float rotTimer = 0;
    private float rotationpercent = 1.5f;

    [SerializeField]
    private float reloadtimer = 0;

    //======== state of bool =========
    private bool isMoving = false;
    private bool isDead = false;
    private bool onDamaged = false;
    private bool EnemySearch = false;
    private bool OnCombat = false;
    private bool InShootingRange = false;
    private bool onHit = false;
    //================================

    private void Awake()
    {
        executions = GetComponent<Executions>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        view = GetComponent<EnemyView>();
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        m_HP = 100;
        state = EnemyState.Idle;
        Target = null;
        shotBullet = null;
        MoveBackCoroutine = null;
        Ammo = 7;
        FirstPosition = transform.position;

        
        foreach(AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if(clip.name == "Reloading")
            {
                reloadtimer = clip.length;
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Dead:
                Dead();
                break;

            case EnemyState.Idle:
                executions.GetAssassinationRange();
                break;

            case EnemyState.Alert:
                Alert();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Patrol:
                executions.GetAssassinationRange();
                break;

            case EnemyState.Search:
                SearchEnemy();
                break;

            case EnemyState.MoveBack:
                executions.GetAssassinationRange();
                MoveBack();
                break;
        }

        //SetTarget();
        Move();
        EnemySight();
        CheckHP();
        StateUpdate();
        UpdateAnim();
    }

    public bool GetCombat()
    {
        return OnCombat;
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.98f);
        anim.SetBool("OnAttack", true);
        gunFlash.Play();
        gunShot.Play();
        Instantiate(Bullet, muzzle.position, transform.rotation);
        Ammo -= 1;
        shotBullet = null;
    }

    private void EnemySight()
    {
        if (null != view.GetTarget())
        {
            Target = view.GetTarget();
            state = EnemyState.Alert;
        }
        else
        {
            Target = null;
            OnCombat = false;
        }
        // 작은 범위내에
        // 걸리면 state -> alert
    }

    private void MoveBack()
    {
        OnCombat = false;

        if (MoveBackCoroutine == null)
        {
            //Debug.Log("Call MoveBack Coroutine");
            MoveBackCoroutine = StartCoroutine(BackToFirstPosition());
        }

        isMoving = true;

        if((FirstPosition - transform.position).sqrMagnitude < 0.5f)
        {
            state = EnemyState.Idle;
            isMoving = false;
            nav.Stop();
            StopCoroutine(MoveBackCoroutine);
            MoveBackCoroutine = null;
        }
    }


    private void Reload()
    {
        if(reload == null)
            reload = StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        anim.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadtimer);
        anim.SetBool("Reload", false);
        Ammo = 7;
        reload = null;
    }
    private void UpdateAnim()
    {
        anim.SetBool("isMoving",isMoving);
    }

    private void SearchEnemy()
    {
        
    }

    public bool GetSearch()
    {
        return EnemySearch;
    }

    private void StateUpdate()
    {
        if (isDead)
            state = EnemyState.Dead;
        else if (!OnCombat)
        {
            if (onDamaged)
                state = EnemyState.Alert;
            else if (isMoving)
                state = EnemyState.Move;
        }
        else if (OnCombat)
        {
            if (Target != null)
                state = EnemyState.Attack;
            else if (EnemySearch)
                state = EnemyState.Search;
        }
        else
            state = EnemyState.Idle;
    }

    public void Alert()
    {
        OnCombat = true;
        firstlookat = transform.forward;
        anim.SetBool("Alert",true);
        if (onHit)
            CheckNearAllies();
        Attack();
    }

    private void CheckNearAllies()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, 4.5f, alliesMask);
        EnemyController ec = new EnemyController();
        for (int index = 0; index < col.Length; index++)
        {
            if (col[index].transform == this.transform)
                continue;

            ec = col[index].GetComponent<EnemyController>();
            if(ec != null)
                ec.Alert();
        }
        onHit = false;
        ec = null;
    }

    private void Move()
    {
        if (Target != null)
        {
            if (view.isInRange())
            {
                rotTimer += Time.deltaTime;
                float per = rotTimer / rotationpercent;
                dir = Target.position - transform.position;
                dir.y = 0f;
                Quaternion rot = Quaternion.LookRotation(dir.normalized);

                transform.forward = Vector3.Lerp(firstlookat, dir, per);
                if (EnemySearch)
                    EnemySearch = !EnemySearch;
                StopMoving();
            }
            else
            {
                if (anim.GetBool("OnAttack"))
                    anim.SetBool("OnAttack", false);

                StopCoroutine(Shoot());
                isMoving = true;
                nav.Resume();
                nav.destination = Target.position;
            }
        }
        else
        {
            if (view.GetTarget() == null && (nav.destination - transform.position).sqrMagnitude < 0.5f)
            {
                isMoving = false;
                state = EnemyState.MoveBack;
            }
        }
    }

    private IEnumerator BackToFirstPosition()
    {
        yield return new WaitForSeconds(2f);
        nav.Resume();
        nav.destination = FirstPosition;
    }

    private void StopMoving()
    {
        isMoving = false;
        nav.Stop();
    }

    private void CheckHP()
    {
        if(m_HP <= 0)
        {
            isDead = true;
        }
    }

    public void Dead()
    {
        isDead = true;
        rigid.useGravity = false;
        Destroy(view);
        Destroy(collider);
        Debug.Log("EnemyDie");
        anim.SetBool("Executed", true);
        Destroy(gameObject, 10f);
    }
    
    public void TakeDamage(int damage)
    {
        onDamaged = true;
        onHit = true;
        m_HP -= damage;
        //string TestText = damage.ToString() + " 데미지를 입어 체력이 " + m_HP.ToString() + " 남았습니다";
        //Debug.Log(TestText);
    }

    private void Attack()
    {
        if(Ammo == 0)
        {
            Reload();
        }
        else
        {
            onDamaged = false;
            if (OnCombat && view.GetTarget() == null)
            {
                EnemySearch = true;
                return;
            }


            if (shotBullet == null)
                shotBullet = StartCoroutine(Shoot());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals("Weapon"))
        {
            if (other.transform.gameObject.layer.Equals(9))
            {
                //Debug.Log("Damaged by Katana");
                TakeDamage(50);
            }
            else if (other.transform.gameObject.layer.Equals(10))
            {
                //Debug.Log("Damaged by CutBlade");
                TakeDamage(10);
            }
        }
    }
}