using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private EnemyState state;
    private Transform trans;
    private NavMeshAgent nav;
    private Animator anim;

    private EnemyView view;

    public Transform PatrolPoint1;
    public Transform PatrolPoint2;
    private Transform Target;
    private Vector3 des;

    

    //======== state of bool =========
    private bool isMoving = false;
    private bool isDead = false;
    private bool onDamaged = false;
    private bool findEnemy = false;
    private bool EnemySearch = false;
    private bool OnCombat = false;
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
        des = Vector3.zero;
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

    private void EnemySight()
    {
        if (null != view.GetTarget())
        {
            Target = view.GetTarget();
            OnCombat = true;
            state = EnemyState.Alert;
            //Move();
        }
        else
        {
            Target = null;
            OnCombat = false;
        }
        // 작은 범위내에
        // 걸리면 state -> alert
    }


    private void UpdateAnim()
    {

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
            state = EnemyState.Attack;
            state = EnemyState.Chase;
            state = EnemyState.Search;
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
            nav.destination = Target.position;
              
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