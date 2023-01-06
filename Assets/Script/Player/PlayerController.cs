using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.UIElements;

enum Playerstate { Idle, Move, Attack, Dodge, BladeMode, Hurt, Dash }
public class PlayerController : MonoBehaviour
{
    [Header("PlayerState")]
    //=========== PlayerState ==============
    private Playerstate state;
    private bool OnDamaged = false;
    private bool OnBladeMode = false;
    private bool BladeAttack = false;
    //======================================
    [Space]

    [Header("Player UI")]
    //============= Player UI ==============
    public HealthBar healthBar;
    public RegainBar RegainBar;
    public ConcentrateBar ConcentrateBar;
    public Transform CutPlane;
    //======================================
    [Space]

    [Header("Weapon Collider")]
    //========- Weapon Collider ============
    public UnityEvent AttackStart;
    public UnityEvent AttackEnd;
    public UnityEvent BladeStart;
    public UnityEvent BladeEnd;
    //======================================
    [Space]

    //============ Animation ===============
    private Animator anim;
    //======================================

    //=========== Player Camera ============
    Camera cam;
    public CinemachineFreeLook PlayerCamera;
    //======================================

    [Header("Player Movement")]
    //========= Player MoveMent ============
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private CharacterController controller;
    //======================================
    [Space]


    [Header("GroundCheck Adn Velocity")]
    //===== GroundCheck And Velocity ======
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    public Vector3 velocity;
    //=======================================
    [Space]


    //=============== Timer =================
    private float regainTimer = 0;
    private float attackTimer = 0;
    //=======================================


    [Header("PlayerStatus")]
    //========== PlayerStatus ===============
    [SerializeField] private int MaxConcentrate = 500;
    [SerializeField] private int maxHealth = 500;
    [SerializeField] private int prevHealth;
    [SerializeField] private int loseHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int concentrate;
    //=======================================

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
        loseHealth = maxHealth;

        concentrate = MaxConcentrate;
        PlayerCamera.m_Priority = 15;
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
        BladeMode();
    }

    public void FixedUpdate()
    {
        TestConcentrate();
    }
    public void SetHealth()
    {
        if(currentHealth > prevHealth)
        {
            prevHealth = currentHealth;
            loseHealth = currentHealth;
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        RegainBar.SetMaxRegainHealth(maxHealth);
        RegainBar.SetRegainHealth(loseHealth);

        ConcentrateBar.SetMax(MaxConcentrate);
        ConcentrateBar.SetGage(concentrate);
    }


    // 테스트용 함수 테스트 끝나면 제거할것
    public void DamageTest()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(150);
        }

        if(Input.GetKeyDown(KeyCode.J)) 
        {
            concentrate = MaxConcentrate;
        }
    }

    public void Move()
    {
        if (state == Playerstate.BladeMode)
        {
            return;
        }
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
        if(prevHealth != currentHealth)
        {
            loseHealth = prevHealth;
        }

        prevHealth = currentHealth;
        currentHealth -= damage;
        OnDamaged = true;
        regainTimer = 0;
        if (currentHealth <= 0)
        {
            PlayerDead();
        }
    }

    public void BladeMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab");
            ModeChanger(OnBladeMode);
        }

        CutPlane.Rotate(0f,0f, Input.GetAxis("Horizontal")* Time.deltaTime * 100);

        if(Input.GetButtonDown("Fire1"))
        {
            BladeStart?.Invoke();
            BladeAttack = true;
        }

        if(BladeAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= 0.15f)
            {
                BladeEnd?.Invoke();
                attackTimer= 0;
            }
        }
    }

    public void ModeChanger(bool value)
    {
        OnBladeMode = !value;
        state = OnBladeMode ? Playerstate.BladeMode : Playerstate.Idle;
        PlayerCamera.m_Priority = OnBladeMode ? 5 : 15;
        CutPlane.gameObject.SetActive(OnBladeMode);
        CutPlane.localEulerAngles = Vector3.zero;
        string debug = OnBladeMode ? "BladeModeOn" : "BladeModeOff";
        Debug.Log(debug);
        if (!OnBladeMode)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0).normalized;
        }
    }

    public void TestConcentrate()
    {
        if(concentrate == 0)
        {
            ModeChanger(OnBladeMode);
            transform.rotation = Quaternion.Euler(0,transform.rotation.y,0).normalized;
        }

        if (OnBladeMode)
        {
            transform.rotation = cam.transform.rotation;
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
            if(prevHealth <= loseHealth)
            {
                loseHealth--;
            }
            
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

    public void RegainHealth()
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
        if (state == Playerstate.Attack && state == Playerstate.BladeMode)
        {
            return;
        }

        if(Input.GetButtonDown("Fire1") && !anim.GetBool("isAttack"))
        {
            state = Playerstate.Attack;
            anim.SetBool("isAttack",true);
            AttackStart?.Invoke();
        }

        if(anim.GetBool("isAttack") == true)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer > 1.5f)
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