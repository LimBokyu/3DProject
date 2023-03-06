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
    private float attackTimer = 0;
    private bool isRight = true;

    private void Awake()
    {
        playercontroller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        SetAttack();
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

            if (attackTimer >= attack.GetStart() &&
               attackTimer < attack.GetEndAnim())
            {
                CheckCombo();
            }

            if (attackTimer >= attack.GetStart() &&
                attackTimer < attack.GetEnd())
            {
                AttackStart?.Invoke();

            }
            else if (attackTimer >= attack.GetEnd() &&
                    attackTimer <= attack.GetEndAnim())
            {
                AttackEnd?.Invoke();
                if (Combo)
                {
                    CallNextAttack = true;
                }
            }
            else if (attackTimer > attack.GetEndAnim())
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
        dashAttack.Add(name, new AttackTime(name, 0.2f, 0.7f, 1.15f));
        name = "DashAttack2";
        dashAttack.Add(name, new AttackTime(name, 0.2f, 0.7f, 1.15f));

    }

    public void DashAttack()
    {
        
    }

    private void SetSlashDirection()
    {
        if(isRight)
        {

        }
        else
        {

        }
    }
}
