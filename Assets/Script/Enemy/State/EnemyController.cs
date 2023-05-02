using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState { Idle, Attack, Alert, Patrol, MoveBack, Dead, Chase }
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyState state;

    // ========= Component ==========
    private Executions executions;
    private EnemyView view;
    private Rigidbody rigid;
    private Collider col;
    private EnemyAttack enemyAttack;
    private EnemyPatrol enemyPatrol;
    private EnemyMove enemyMove;
    public Animator anim;
    // ==============================

    // ======== Enemy Alert ==========
    private Vector3 firstlookat;
    // ===============================

    // ======== MoveBack Logic =======
    [SerializeField]
    private Vector3 firstPosition;
    private Coroutine moveBackCoroutine;
    // ===============================

    [SerializeField]
    private Transform target;

    [SerializeField]
    private LayerMask alliesMask;
    //==============

    // ======= Enemy Rotation ========
    private Vector3 dir;
    private float rotTimer = 0f;
    private float rotationpercent = 1.5f;
    // ===============================
       
    // ======== Enemy Status =========
    [SerializeField]
    private int m_HP;
    // ===============================

    // ======== state of bool ========
    private bool isMoving = false;
    private bool isDead = false;
    private bool onDamaged = false;
    private bool enemySearch = false;
    private bool onCombat = false;
    private bool inShootingRange = false;
    private bool onHit = false;
    private bool onMoveBack = false;
    private bool onAlert = false;
    private bool onChase = false;
    public  bool isPatrol = false;
    // ===============================

    private void Awake()
    {
        executions = GetComponent<Executions>();
        anim = GetComponent<Animator>();
        view = GetComponent<EnemyView>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    private void Start()
    {
        m_HP = 100;
        state = EnemyState.Idle;
        target = null;
        moveBackCoroutine = null;
        firstPosition = transform.position;
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Dead:
                break;

            case EnemyState.Idle:
                break;

            case EnemyState.Alert:
                Alert();
                break;

            case EnemyState.Attack:
                enemyAttack.AttackBehaviour();
                break;

            case EnemyState.Patrol:
                enemyPatrol.PatrolBehaviour();
                break;

            case EnemyState.Chase:
                Move();
                break;

            case EnemyState.MoveBack:
                MoveBack();
                break;
        }

        EnemySight();
        CheckHP();
        StateUpdate();
        UpdateAnim();
    }

    private void StateUpdate()
    {
        if (isDead)
            state = EnemyState.Dead;
        else if (!onCombat)
        {
            if (onDamaged || onAlert)
                state = EnemyState.Alert;
            else if (onMoveBack)
                state = EnemyState.MoveBack;
            else
            {
                if (isPatrol)
                    state = EnemyState.Patrol;
                else
                    state = EnemyState.Idle;
            }
        }
        else if (onCombat)
        {
            if (target != null)
                state = EnemyState.Attack;
            else if (onChase)
                state = EnemyState.Chase;
        }
    }

    public bool GetCombatState()
    {
        return onCombat;
    }

    public void SetIsMoving(bool value)
    {
        isMoving = value;
    }

    private void EnemySight()
    {
        if (null != view.GetTarget())
        {
            target = view.GetTarget();
            onAlert = true;
        }
        else
        {
            target = null;
            onCombat = false;
        }
        // Player�� ���� ���� ���� Target���� �����Ǿ��� ��� state -> alert
    }

    private void MoveBack()
    {
        onCombat = false;

        if (moveBackCoroutine == null)
        {
            //Debug.Log("Call MoveBack Coroutine");
            moveBackCoroutine = StartCoroutine(BackToFirstPosition());
        }

        isMoving = true;

        if((firstPosition - transform.position).sqrMagnitude <= 0.5f)
        {
            state = EnemyState.Idle;
            isMoving = false;
            enemyMove.SetNavMeshMoving(true);
            StopCoroutine(moveBackCoroutine);
            moveBackCoroutine = null;
        }
    }

    private void UpdateAnim()
    {
        anim.SetBool("isMoving",isMoving);
    }

    public bool GetSearch()
    {
        return enemySearch;
    }

    public void Alert()
    {
        onCombat = true;
        firstlookat = transform.forward;
        anim.SetBool("Alert",true);
        if (onHit)
            CheckNearAllies();
        enemyAttack.AttackBehaviour();
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
        if (target != null)
        {
            if (view.isInRange())
            {
                rotTimer += Time.deltaTime;
                float per = rotTimer / rotationpercent;
                dir = target.position - transform.position;
                dir.y = 0f;
                Quaternion rot = Quaternion.LookRotation(dir.normalized);

                transform.forward = Vector3.Lerp(firstlookat, dir, per);
                if (enemySearch)
                    enemySearch = !enemySearch;
                StopMoving();
            }
            else
            {
                if (anim.GetBool("OnAttack"))
                    anim.SetBool("OnAttack", false);

                isMoving = true;
                // Nav <- Stop;
                enemyMove.SetNavAgentDestination(target.position);
            }
        }
        else
        {
            if (view.GetTarget() == null && (enemyMove.GetNavAgentDestination() - transform.position).sqrMagnitude < 0.5f)
            {
                isMoving = false;
                state = EnemyState.MoveBack;
            }
        }
    }

    private IEnumerator BackToFirstPosition()
    {
        yield return new WaitForSeconds(2f);
        //NavControl(false);
        //nav.destination = firstPosition;
    }

    private void StopMoving()
    {
        isMoving = false;
        
    }

    private void CheckHP()
    {
        if(m_HP <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        isDead = true;
        rigid.useGravity = false;
        DisableEnemyComponent();
        Debug.Log("EnemyDie");
        anim.SetBool("Executed", true);
        Destroy(gameObject, 10f);
    }

    private void DisableEnemyComponent()
    {
        Destroy(view);
        Destroy(enemyPatrol);
        Destroy(enemyAttack);
        Destroy(GetComponent<Collider>());
    }
    
    public void TakeDamage(int damage)
    {
        onDamaged = true;
        onHit = true;
        m_HP -= damage;
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