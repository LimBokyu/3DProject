using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossState { idle, attack, dodge, changePhase, dead}
public class BossController : MonoBehaviour
{
    // ======== Component =========
    [SerializeField] private BossAttack attack;
    [SerializeField] private BossEvent events;
    // ============================

    BossState state;

    private Animator anim;
    private Rigidbody rigid;

    private int hp;

    // ====== state of bool =======
    private bool isAttacking;
    private bool isDead;
    private bool isChasing;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        SetDefault();
    }

    private void Update()
    {
        switch (state)
        {
            case BossState.idle:
                break;
            case BossState.attack:
                attack.BossAttackBehaviour();
                break;
            case BossState.dodge:
                break;
            case BossState.changePhase:
                break;
            case BossState.dead:
                Dead();
                break;
        }

        CheckBossLife();
    }

    private void LateUpdate()
    {
        StateUpdate();
    }

    private void StateUpdate()
    {
        if (isDead)
            state = BossState.dead;
        else if(isAttacking)
            state = BossState.attack;
        else if(isChasing)
            state = BossState.changePhase;
        else
            state = BossState.idle;
    }

    private void CheckBossLife()
    {
        if (hp <= 0)
            isDead = true;
    }

    private void Dead()
    {

    }

    public void SetDefault()
    {
        hp = 2000;
    }
}
