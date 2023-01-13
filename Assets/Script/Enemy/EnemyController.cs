using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState { Idle, Attack, Alert, Patrol, MoveBack, Dead, Search, Chase}
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int m_HP;

    private float moveSpeed = 500f;

    private EnemyState state;
    private Transform trans;
    private NavMeshAgent nav;

    private Animator anim;

    private bool isMoving = false;
    private void Start()
    {
        m_HP = 100;
        state = EnemyState.Idle;
        trans = GetComponent<Transform>();  
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        switch (state)
            
        {
            case EnemyState.Dead:
                Dead();
                break;

            case EnemyState.Chase:
                Move();
                break;

            case EnemyState.Idle:
                break;

            case EnemyState.Alert:
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Patrol:
                Move();
                break;

            case EnemyState.Search:
                Move();
                break;

            case EnemyState.MoveBack:
                Move();
                break;
        }

        Alive();
    }

    private void Move()
    {
        isMoving = true;
        anim.SetBool("isMoving", isMoving);
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed);
    }

    private void Alive()
    {
        if(m_HP <= 0)
        {
            state = EnemyState.Dead;
            Dead();
        }
    }

    public void Dead()
    {
        Debug.Log("EnemyDie");
        Destroy(gameObject);
    }
    
    public void TakeDamage(int damage)
    {
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
