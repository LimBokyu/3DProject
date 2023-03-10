using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum Playerstate { Idle, Move, Attack, Dodge, BladeMode, Hurt, NinjaRun, Executions }
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        Camera cam;
        [Header("PlayerState")]
        //=========== PlayerState ==============
        [SerializeField] private Playerstate state;
        private bool OnDamaged = false;
        private bool OnBladeMode = false;
        private bool Moving = false;
        private bool onNinjaRun = false;
        //======================================
        [Space]

        [Header("Player UI")]
        //============= Player UI ==============
        public ScreenFlash flash;
        //======================================
        [Space]

        [Header("Component")]
        //============= BladeMode ==============
        private BladeMode bladeMode;
        private PlayerAttack playerattack;
        private PlayerHealth health;
        private NinjaRun ninjarun;
        private Assassination assassination;
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

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            bladeMode = GetComponent<BladeMode>();
            playerattack= GetComponent<PlayerAttack>();
            health = GetComponent<PlayerHealth>();
            ninjarun = GetComponent<NinjaRun>();
            assassination = GetComponent<Assassination>();
        }

        public void Start()
        {
            cam = Camera.main;
            state = Playerstate.Idle;
            Cursor.lockState = CursorLockMode.Locked;
            health.PlayerUISet();
            health.SetHealth();
        }

        public void Update()
        {
            switch (state)
            {
                case Playerstate.Idle:
                    BladeModeSwitch();
                    playerattack.AttackOrder();
                    playerattack.DashAttackEnd();
                    assassination.CheckAssasination();
                    assassination.LockOnTarget();
                    Move();
                    Jump();
                    break;
                case Playerstate.Move:
                    BladeModeSwitch();
                    NinjaRunOrder();
                    playerattack.DashAttackOrder();
                    playerattack.DashAttack();
                    assassination.LockOnTarget();
                    Move();
                    Jump();
                    break;
                case Playerstate.NinjaRun:
                    NinjaRunOrder();
                    playerattack.DashAttackEnd();
                    ninjarun.NinjaRunBehaviour();
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
            health.SetHealth();
            health.Regain();
            IsGround();
            ConcentrateControl();
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
            else if(onNinjaRun)
            {
                state = Playerstate.NinjaRun;
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

        private void NinjaRunOrder()
        {
            if(Moving)
            {
                onNinjaRun = Input.GetKey(KeyCode.LeftShift) ? true : false;
                moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 15f : 10f;
            }
        }

        // 테스트용 함수 테스트 끝나면 제거할것
        public void DamageTest()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                health.TakeDamage(150);
            }
        }

        public void Move()
        {
            Vector3 moveVec = InputMove();
            controller.Move(moveVec * moveSpeed * Time.deltaTime);

            if (moveVec.sqrMagnitude != 0)
                transform.forward = Vector3.Lerp(transform.forward, moveVec, 0.8f);

            Moving = moveVec.sqrMagnitude != 0 ? true : false;

            anim.SetBool("isMoving", Moving);
            anim.SetFloat("MoveSpeed", moveVec.sqrMagnitude);
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

        public void BladeModeSwitch()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Tab");
                bladeMode.ModeChanger(OnBladeMode);
                OnBladeMode = !OnBladeMode;
            }
        }

        public void ConcentrateControl()
        {
            if (health.GetConcentrate() == 0)
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
                health.ChargeConcentrate();
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
                health.TakeDamage(100);
                Destroy(other.gameObject);
            }
        }
    }
}