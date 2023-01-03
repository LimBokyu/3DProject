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
    public ConcentrateBar ConcentrateBar;

    public UnityEvent AttackStart;
    public UnityEvent AttackEnd;

    private Animator anim;

    private CharacterController controller;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool isGrounded;

    public Vector3 velocity;


    private Playerstate state;

    private float regainTimer = 0;
    private float attackTimer = 0;
    private bool OnDamaged = false;

    private bool OnFreeSlash = false;
    private int MaxConcentrate = 500;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private int maxHealth = 500;

    [SerializeField]
    private int prevHealth;

    [SerializeField]
    private int currentHealth;

    [SerializeField]
    private int concentrate;

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

        concentrate = MaxConcentrate;

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
        IsGround();
        FreeSlashMod();
    }

    public void FixedUpdate()
    {
        TestConcentrate();
    }
    public void SetHealth()
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        RegainBar.SetMaxRegainHealth(maxHealth);
        RegainBar.SetRegainHealth(prevHealth);

        ConcentrateBar.SetMax(MaxConcentrate);
        ConcentrateBar.SetGage(concentrate);
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
        Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
        Vector3 rightVec = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

        Vector3 moveInput = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

        Vector3 moveVec = forwardVec * moveInput.z + rightVec * moveInput.x;
      
        controller.Move(moveVec * moveSpeed * Time.deltaTime);
        if (moveVec.sqrMagnitude != 0)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveVec, 0.8f);
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

    public void FreeSlashMod()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab");
            if (OnFreeSlash)
            {
                Debug.Log("자유참격Off");
                OnFreeSlash = false;
            }
            else
            {
                Debug.Log("자유참격On");
                OnFreeSlash = true;
            }
        }
    }
    public void TestConcentrate()
    {
        if(concentrate == 0)
        {
            Debug.Log("집중도 -> 0");
            OnFreeSlash = false;
        }

        if(OnFreeSlash)
        {
            concentrate--;
        }
        else
        {
            if (concentrate >= MaxConcentrate)
            {
                return;
            }
            concentrate++;
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
                if(prevHealth <= currentHealth)
                {
                    OnDamaged = false;
                    regainTimer = 0;
                    prevHealth = currentHealth;
                }
            }
        }
    }

    public void RegainHelth()
    {
        Debug.Log("RegainHealth");
        int count = 0;
        while(currentHealth < prevHealth)
        {
            Debug.Log("Health+1");
            currentHealth++;
            count++;
            if(count==10)
            {
                break;
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
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = jumpSpeed;
            }
        }

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);

    }

    public void Attack()
    {
        if(Input.GetButtonDown("Fire1") && !anim.GetBool("isAttack"))
        {
            state = Playerstate.Attack;
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
                state = Playerstate.Idle;
            }
        }
    }

    public void IsGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        velocity.y += Physics.gravity.y * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
}