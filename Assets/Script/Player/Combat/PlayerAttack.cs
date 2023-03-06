using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    private Dictionary<string, AttackTime> MeleeAttack;
    private Dictionary<string, AttackTime> dashAttack;
    public int AttackCount = 0;
    private bool Combo = false;
    private bool CallNextAttack = false;
    private string prevkey;
    private string key = "melee";
    public UnityEvent AttackStart;
    public UnityEvent AttackEnd;
    private PlayerController playercontroller;
    [SerializeField] private float attackTimer = 0;
    private bool isRight = true;
    [SerializeField] private bool dashAttackOrder = false;

    private void Awake()
    {
        playercontroller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        SetAttack();
        SetDashAttack();
    }

    public void AttackOrder()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            key = "melee";
            AttackCount = 1;
            key += AttackCount.ToString();
            playercontroller.anim.SetBool(key, true);
        }
    }
    public void Attack()
    {
        if (AttackCount == 0)
            return;


        if (CallNextAttack)
        {
            key = "melee";
            Vector3 moveVec = playercontroller.InputMove();
            if (moveVec.sqrMagnitude != 0)
                transform.forward = Vector3.Lerp(transform.forward, moveVec, 0.8f);

            AttackCount = AttackCount >= 3 ? 1 : AttackCount += 1;
            key += AttackCount.ToString();
            attackTimer = 0;
            playercontroller.anim.SetBool(key, true);
            playercontroller.anim.SetBool(prevkey, false);
            Combo = false;
            CallNextAttack = false;
        }


        if (AttackCount > 0)
        {
            attackTimer += Time.deltaTime;
            AttackTime attack;
            MeleeAttack.TryGetValue(key, out attack);
            prevkey = key;

            if (attackTimer >= attack.startattack &&
               attackTimer < attack.endanim)
            {
                CheckCombo();
            }

            if (attackTimer >= attack.startattack &&
                attackTimer < attack.endattack)
            {
                AttackStart?.Invoke();

            }
            else if (attackTimer >= attack.endattack &&
                    attackTimer <= attack.endanim)
            {
                AttackEnd?.Invoke();
                if (Combo)
                {
                    CallNextAttack = true;
                }
            }
            else if (attackTimer > attack.startattack)
            {
                playercontroller.anim.SetBool(key, false);
                attackTimer = 0;
                AttackCount = 0;
            }
        }
    }

    private void CheckCombo()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Combo = true;
        }
    }

    private void SetAttack()
    {
        MeleeAttack = new Dictionary<string, AttackTime>();
        string name;
        name = "melee1";
        MeleeAttack.Add(name, new AttackTime(name, 0.2f, 0.7f, 1.15f));
        name = "melee2";
        MeleeAttack.Add(name, new AttackTime(name, 0.2f, 0.7f, 1.05f));
        name = "melee3";
        MeleeAttack.Add(name, new AttackTime(name, 0.02f, 1.01f, 1.3f));
    }

    private void SetDashAttack()
    {
        dashAttack = new Dictionary<string, AttackTime>();
        string name;
        name = "DashAttack1";
        dashAttack.Add(name, new AttackTime(name, 0.05f, 0.3f, 0.4f));
        name = "DashAttack2";
        dashAttack.Add(name, new AttackTime(name, 0.05f, 0.3f, 0.4f));

    }

    public void DashAttackOrder()
    {
        if (Input.GetButtonDown("Fire1") && !dashAttackOrder)
        {
            dashAttackOrder = true;
        }
    }

    public void DashAttackEnd()
    {
        if (!dashAttackOrder)
            return;

        dashAttackOrder = false;
        playercontroller.anim.SetBool("SlashAgain", false);
        playercontroller.anim.SetBool("DashAttack", false);
        if (attackTimer != 0)
            attackTimer = 0;
    }

    private void ContinueDashAttack()
    {
        if(Input.GetButton("Fire1") && !Combo)
        {
            Combo = true;
        }
    }

    public void DashAttack()
    {
        if (!dashAttackOrder)
            return;

        attackTimer += Time.deltaTime;
        playercontroller.anim.SetBool("DashAttack",true);
        AttackTime dashattack;
        string key = "DashAttack";
        int keynumber = isRight ? 1 : 2;
        key += keynumber.ToString();
        dashAttack.TryGetValue(key,out dashattack);

        if (dashattack.startattack <= attackTimer
            && dashattack.endanim > attackTimer)
        {
            ContinueDashAttack();
        }

        if(dashattack.startattack <= attackTimer &&
            dashattack.endattack > attackTimer)
        {
            AttackStart?.Invoke();
        }
        else if(Combo)
        {
            attackTimer = 0;
            playercontroller.anim.SetBool("SlashAgain",!isRight);
            isRight = !isRight;
            Combo = false;
        }
        else if(dashattack.endattack <= attackTimer &&
            dashattack.endanim > attackTimer)
        {
            AttackEnd?.Invoke();
        }
        else if(dashattack.endanim <= attackTimer)
        {
            attackTimer = 0;
            playercontroller.anim.SetBool("DashAttack", false);
            dashAttackOrder = false;
        }
    }

}
