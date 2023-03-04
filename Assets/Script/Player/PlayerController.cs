using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum Playerstate { Idle, Move, Attack, Dodge, BladeMode, Hurt, Dash }
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        Camera cam;
        [Header("PlayerState")]
        //=========== PlayerState ==============
        private Playerstate state;
        private bool OnDamaged = false;
        private bool OnBladeMode = false;
        private bool BladeAttack = false;
        private bool Moving = false;
        //======================================
        [Space]

        [Header("Player UI")]
        //============= Player UI ==============
        public HealthBar healthBar;
        public RegainBar RegainBar;
        public ConcentrateBar ConcentrateBar;
        public ScreenFlash flash;
        //======================================
        [Space]

        [Header("Component")]
        //============= BladeMode ==============
        private BladeMode bladeMode;
        private PlayerAttack playerattack;
        //======================================
        [Space]

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
        private float groundDistance = 0.4f;
        public LayerMask groundMask;
        private bool isGrounded;
        private Vector3 velocity;
        //=======================================
        [Space]

        //============ Animation ===============
        public Animator anim;
        //======================================

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
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            bladeMode = GetComponent<BladeMode>();
            playerattack= GetComponent<PlayerAttack>();
        }

        public void Start()
        {
            cam = Camera.main;
            state = Playerstate.Idle;
            Cursor.lockState = CursorLockMode.Locked;
            PlayerUISet();
            SetHealth();
        }

        public void PlayerUISet()
        {
            currentHealth = maxHealth;
            prevHealth = maxHealth;
            loseHealth = maxHealth;
            concentrate = MaxConcentrate;
        }

        public void Update()
        {
            switch (state)
            {
                case Playerstate.Idle:
                    BladeModeSwitch();
                    playerattack.Attack();
                    Move();
                    Jump();
                    break;
                case Playerstate.Move:
                    BladeModeSwitch();
                    Move();
                    Jump();
                    break;
                case Playerstate.BladeMode:
                    BladeModeSwitch();
                    bladeMode.BladeModeState();
                    break;
                case Playerstate.Attack:
                    playerattack.Attack();
                    break;
            }

            StateUpdate();
            DamageTest();
            SetHealth();
            Regain();
            IsGround();
            TestConcentrate();
        }

        public void StateUpdate()
        {
            if(OnBladeMode)
            {
                state = Playerstate.BladeMode;
            }
            else if(playerattack.AttackCount > 0)
            {
                state = Playerstate.Attack;
            }
            else if(Moving)
            {
                state = Playerstate.Move;
            }
            else
            {
                state = Playerstate.Idle;
            }
        }

        public void SetHealth()
        {
            if (currentHealth > prevHealth)
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
            if (Input.GetKeyDown(KeyCode.K))
            {
                TakeDamage(150);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                concentrate = MaxConcentrate;
            }
        }

        public void Move()
        {
            string MoveSpeed = "MoveSpeed";
            Vector3 moveVec = InputMove();
            controller.Move(moveVec * moveSpeed * Time.deltaTime);

            if (moveVec.sqrMagnitude != 0)
                transform.forward = Vector3.Lerp(transform.forward, moveVec, 0.8f);

            Moving = moveVec.sqrMagnitude != 0 ? true : false;

            anim.SetBool("isMoving", Moving);
            anim.SetFloat(MoveSpeed, moveVec.sqrMagnitude);
        }

        public Vector3 InputMove()
        {
            Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
            Vector3 rightVec = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

            Vector3 moveInput = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

            Vector3 moveVec = forwardVec * moveInput.z + rightVec * moveInput.x;

            return moveVec;
        }

        public void TakeDamage(int damage)
        {
            flash.Hurt();
            if (prevHealth != currentHealth)
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

        public void BladeModeSwitch()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Tab");
                bladeMode.ModeChanger(OnBladeMode);
                OnBladeMode = !OnBladeMode;
            }
        }

        public void TestConcentrate()
        {
            if (concentrate == 0)
            {
                bladeMode.ModeChanger(OnBladeMode);
                transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0).normalized;
            }

            if (OnBladeMode)
            {
                transform.rotation = cam.transform.rotation.normalized;
                //concentrate--;
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
            if (OnDamaged)
            {
                if (prevHealth <= loseHealth)
                {
                    loseHealth--;
                }

                regainTimer += Time.deltaTime;
                if (regainTimer > 3f)
                {
                    prevHealth--;
                    if (prevHealth <= currentHealth)
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
            while (currentHealth < prevHealth)
            {
                Debug.Log("Health+1");
                currentHealth++;
                count++;
                if (count == 10)
                {
                    break;
                }
            }
        }

        public void PlayerDead()
        {
            // 사망 미구현
        }

        public void Jump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = jumpSpeed;
                anim.SetBool("isJump", true);
            }
            controller.Move(Vector3.up * velocity.y * Time.deltaTime);
        }


        public void IsGround()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            velocity.y += Physics.gravity.y * Time.deltaTime;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                anim.SetBool("isJump", false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag.Equals("Bullet"))
            {
                TakeDamage(100);
                Destroy(other.gameObject);
            }
        }
    }
}