using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState { Idle, Move, Attack, Alert, Patrol, MoveBack, Dead}
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int m_HP;

    private EnemyState state;
    private Transform transform;
    private NavMeshAgent nav;

    private void Start()
    {
        m_HP = 100;
        state = EnemyState.Idle;
        transform = GetComponent<Transform>();  
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Move();
        Alive();
    }

    private void Move()
    {
        
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
