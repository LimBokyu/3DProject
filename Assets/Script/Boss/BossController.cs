using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossState { idle, attack, dodge, changePhase, dead, stun, encounter}
public class BossController : MonoBehaviour
{
    // ======== Component =========
    [SerializeField] private BossAttack attack;
    [SerializeField] private BossEvent events;
    [SerializeField] private BossAction action;
    // ============================    

    private Animator anim;
    private Rigidbody rigid;

    private int hp;

    private Coroutine hitRecovery = null;
    private YieldInstruction recoveryTime = new WaitForSeconds(3f);

    // ====== State and Bool =======
    BossState state;
    private bool isAttacking;
    private bool isDead;
    private bool isChasing;
    private bool isInvincible;
    private bool isStun;
    private bool isChangePhase;
    private bool isDodge;
    // =============================

    // ========= Property =========
    public bool Invincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }
    // ============================

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
                action.BossDodge();
                break;
            case BossState.changePhase:
                break;
            case BossState.dead:
                Dead();
                break;
            case BossState.stun:
                break;
            case BossState.encounter:
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
        else if (isAttacking)
            state = BossState.attack;
        else if (isChasing)
            state = BossState.changePhase;
        else if (isStun)
            state = BossState.stun;
        else if(isChangePhase)
            state = BossState.changePhase;
        else if(isDodge)
            state = BossState.dodge;
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
        anim.SetBool("isDead", true);
    }

    public void SetDefault()
    {
        hp = 2000;
    }

    private IEnumerator HitRecovery()
    {
        yield return recoveryTime;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
            return;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isInvincible)
            return;
    }
}
