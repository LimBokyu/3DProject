using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
enum Playerstate { Idle, Move, Attack}
public class PlayerController : MonoBehaviour
{
    Camera cam;
    public HealthBar healthBar;
    public RegainBar RegainBar;

    public UnityEvent AttackStart;
    public UnityEvent AttackEnd;

    private Animator anim;

    private CharacterController controller;

    private Playerstate state;

    private float regainTimer = 0;
    private float attackTimer = 0;
    private bool OnDamaged = false;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField] 
    private float moveY;

    [SerializeField]
    private int maxHealth = 500;

    [SerializeField]
    private int prevHealth;

    [SerializeField]
    private int currentHealth;

    [SerializeField]
    private float concentrate;

    private void Awake()
    {
        controller= GetComponent<CharacterController>();   
        anim = GetComponentInChildren<Animator>();
    }

    public void Start()
    {
        cam = Camera.main;
        state = Playerstate.Idle;
        Cursor.lockState = CursorLockMode.Locked;

        currentHealth = maxHealth;
        prevHealth = maxHealth;
        SetHealth();
    }

    public void Update()
    {
        Move();
        Jump();
        Attack();
        DamageTest();
        SetHealth();
        Regain();
    }

    public void SetHealth()
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        RegainBar.SetMaxRegainHealth(maxHealth);
        RegainBar.SetRegainHealth(prevHealth);
    }

    public void DamageTest()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(150);
        }
    }

    public void Move()
    {
        if(state != Playerstate.Attack)
        {
            Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x,0f,Camera.main.transform.forward.z).normalized;
            Vector3 rightVec = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

            Vector3 moveInput = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

            Vector3 moveVec = forwardVec* moveInput.z + rightVec* moveInput.x;

            controller.Move(moveVec * moveSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        prevHealth = currentHealth;
        currentHealth -= damage;
        OnDamaged = true;
        regainTimer = 0;
        if (currentHealth <= 0)
        {
            PlayerDead();
        }
    }

    public void Regain()
    {
        if(OnDamaged)
        {
            regainTimer += Time.deltaTime;
            if(regainTimer > 3f)
            {
                prevHealth--;
                if(prevHealth == currentHealth)
                {
                    OnDamaged = false;
                    regainTimer = 0;
                }
            }
        }
    }

    public void PlayerDead()
    {

    }

    public void Jump()
    {
        if (state != Playerstate.Attack)
        {
            moveY += Physics.gravity.y * Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                moveY = jumpSpeed;
            }
            else if (controller.isGrounded && moveY < 0)
            {
                //moveY += Physics.gravity.y * 0.1f;
                moveY = 0;
            }


            controller.Move(Vector3.up * moveY * Time.deltaTime);
        }

    }

    public void Attack()
    {
        if(Input.GetButtonDown("Fire1") && !anim.GetBool("isAttack"))
        {
            anim.SetBool("isAttack",true);
            AttackStart?.Invoke();
        }

        if(anim.GetBool("isAttack") == true)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer > 1f)
            {
                anim.SetBool("isAttack", false);
                AttackEnd?.Invoke();
                attackTimer = 0;
            }
        }
    }

    public bool IsGround()
    {

        return false;
    }
}