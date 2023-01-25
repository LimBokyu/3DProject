using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState { Idle, Attack, Alert, Patrol, MoveBack, Dead, Search, Chase, Move }
enum IdleState { None, Idle, Patrol, MoveBack }
enum CombatState { None, Attack, Alert, Search, Chase }

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int m_HP;

    private float moveSpeed = 500f;

    [SerializeField] private EnemyState state;
    private Transform trans;
    private NavMeshAgent nav;
    private Animator anim;

    private Coroutine shotBullet;

    private EnemyView view;

    public Transform muzzle;
    public Transform PatrolPoint1;
    public Transform PatrolPoint2;

    public GameObject Bullet;
    public ParticleSystem gunFlash;

    [SerializeField]
    private Transform Target;

    private Vector3 dir;

    //======== state of bool =========
    private bool isMoving = false;
    private bool isDead = false;
    private bool onDamaged = false;
    private bool findEnemy = false;
    private bool EnemySearch = false;
    private bool OnCombat = false;
    private bool InShootingRange = false;
    //================================



    private void Start()
    {
        m_HP = 100;
        state = EnemyState.Idle;
        trans = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        view = GetComponent<EnemyView>();
        Target = null;
        shotBullet = null;
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Dead:
                Dead();
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Idle:
                break;

            case EnemyState.Alert:
                Alert();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Patrol:
                break;

            case EnemyState.Search:
                SearchEnemy();
                break;

            case EnemyState.MoveBack:
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
        yield return new WaitForSeconds(1.2f);
        float y = muzzle.transform.eulerAngles.y;
        float z = muzzle.transform.eulerAngles.z;
        Instantiate(Bullet, muzzle.position, transform.rotation); //Quaternion.Euler(0f, y, z));
        shotBullet = null;
    }

    private void EnemySight()
    {
        if (null != view.GetTarget())
        {
            Target = view.GetTarget();
            OnCombat = true;
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

    
    private void SetTarget()
    {
        if (null != view.GetTarget())
        {
            Target = view.GetTarget();
            Target.position = view.GetTarget().position;
        }
        else
        { Target = null; }
    }


    private void UpdateAnim()
    {
        anim.SetBool("isMoving",isMoving);
    }

    private void SearchEnemy()
    {
        
    }

    private void StateUpdate()
    {
        if (isDead)
            state = EnemyState.Dead;
        else if (onDamaged)
            state = EnemyState.Alert;
        else if (isMoving)
            state = EnemyState.Move;
        else if(OnCombat)
        {
          // state = EnemyState.Attack;
          // state = EnemyState.Chase;
          // state = EnemyState.Search;
        }
    }

    private void Alert()
    {
        OnCombat = true;
        Attack();
    }

    private void Move()
    {
        if (Target != null)
        {
            if (view.isInRange())
            {
                dir = Target.position - transform.position;
                dir.y = 0f;
                Quaternion rot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = rot;
                StopMoving();
            }
            else
            {
                StopCoroutine(Shoot());
                isMoving = true;
                nav.Resume();
                nav.destination = Target.position;
            }
        }
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
        Debug.Log("EnemyDie");
        Destroy(gameObject);
    }
    
    public void TakeDamage(int damage)
    {
        onDamaged = true;
        m_HP -= damage;
        string TestText = damage.ToString() + " 데미지를 입어 체력이 " + m_HP.ToString() + " 남았습니다";
        Debug.Log(TestText);
    }

    private void Attack()
    {
        if (shotBullet == null)
          shotBullet = StartCoroutine(Shoot());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Equals("Weapon"))
        {
            if (other.transform.gameObject.layer.Equals(9))
            {
                Debug.Log("Damaged by Katana");
                TakeDamage(50);
            }
            else if (other.transform.gameObject.layer.Equals(10))
            {
                Debug.Log("Damaged by CutBlade");
                TakeDamage(10);
            }
        }
    }
}