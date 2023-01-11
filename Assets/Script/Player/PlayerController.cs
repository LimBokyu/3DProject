using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.UIElements;

enum Playerstate { Idle, Move, Attack, Dodge, BladeMode, Hurt, Dash }
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("PlayerState")]
        //=========== PlayerState ==============
        private Playerstate state;
        private bool OnDamaged = false;
        private bool OnBladeMode = false;
        private bool BladeAttack = false;
        private bool Moving = false;
        //======================================
        [Space]

        //=========== PlayerAttack =============
        private Dictionary<string, AttackTime> MeleeAttack;
        private int AttackCount = 0;
        private bool Combo = false;
        private bool CallNextAttack = false;
        private string prevkey;
        private string key = "melee";
        //======================================

        [Header("Player UI")]
        //============= Player UI ==============
        public HealthBar healthBar;
        public RegainBar RegainBar;
        public ConcentrateBar ConcentrateBar;
        //======================================
        [Space]

        [Header("Weapon Collider")]
        //========= Weapon Collider ============
        public UnityEvent AttackStart;
        public UnityEvent AttackEnd;
        public UnityEvent BladeStart;
        public UnityEvent BladeEnd;
        //======================================
        [Space]

        [Header("BladeMode")]
        //============= BladeMode ==============
        public TimeManager timemanager;
        public Transform CutPlane;
        //======================================
        [Space]

        [Header("Camera")]
        //=========== Player Camera ============
        Camera cam;
        public CinemachineBrain    CineMachine;
        public CinemachineFreeLook PlayerCamera;
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
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        private bool isGrounded;
        public Vector3 velocity;
        //=======================================
        [Space]

        //============ Animation ===============
        private Animator anim;
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
            SetAttack();
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
            TestConcentrate();
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
            string RightVec = "Horizontal";
            string FowardVec = "Vertical";
            if (state == Playerstate.BladeMode || state == Playerstate.Attack)
            {
                Moving = false;
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
            Moving = moveVec.sqrMagnitude != 0 ? true : false;

            MoveState();
            anim.SetFloat(RightVec, moveVec.x);
            anim.SetFloat(FowardVec, moveVec.z);


        }

        private void MoveState()
        {
            state = Moving ? Playerstate.Move : Playerstate.Idle ;
            anim.SetBool("isMoving", Moving);
        }


        public void TakeDamage(int damage)
        {
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

        public void BladeMode()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Tab");
                ModeChanger(OnBladeMode);
            }

            CutPlane.Rotate(0f, 0f, Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * 100);


            if (state == Playerstate.BladeMode)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    BladeStart?.Invoke();
                    BladeAttack = true;
                }

                if (BladeAttack)
                {
                    attackTimer += Time.unscaledDeltaTime;
                    if (attackTimer >= 0.15f)
                    {
                        BladeEnd?.Invoke();
                        attackTimer = 0;
                        BladeAttack = !BladeAttack;
                    }
                }
            }
        }

        public void ModeChanger(bool BladeMode)
        {
            OnBladeMode = !BladeMode;
            // 원래의 OnBladeMode의 반대되는 값을 OnBladeMode에 입력하고
            // OnBladeMode의 True/False 값을 기준으로 BladeMode/Idle 을 구분할 값을 변경
            state = OnBladeMode ? Playerstate.BladeMode : Playerstate.Idle;
            PlayerCamera.m_Priority = OnBladeMode ? 5 : 15;
            CutPlane.gameObject.SetActive(OnBladeMode);
            CutPlane.localEulerAngles = Vector3.zero;
            CineMachine.m_DefaultBlend = OnBladeMode ?
                new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.02f) :
                new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f)  ;
            attackTimer = 0;
            string debug = OnBladeMode ? "BladeModeOn" : "BladeModeOff";
            Debug.Log(debug);
            if (!OnBladeMode)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0).normalized;
                timemanager.SlowMotionOut();
            }
            else
            {
                timemanager.SlowMotion();
            }
        }

        public void TestConcentrate()
        {
            if (concentrate == 0)
            {
                ModeChanger(OnBladeMode);
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
            if (state == Playerstate.BladeMode)
            {
                return;
            }

            if(state == Playerstate.Idle)
            {
                key = "melee";
            }

            if (AttackCount == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    key = "melee";
                    state = Playerstate.Attack;
                    AttackCount = 1;
                    key += AttackCount.ToString();
                    anim.SetBool(key, true);
                }
            }


            if (CallNextAttack)
            {
                key = "melee";
                if (AttackCount >= 3)
                {
                    AttackCount = 1;
                }
                else
                {
                    AttackCount += 1;
                }
                key += AttackCount.ToString();
                attackTimer = 0;
                anim.SetBool(key, true);
                anim.SetBool(prevkey, false);
                Combo = false;
                CallNextAttack = false;
            }
            

            if (state == Playerstate.Attack)
            {
                attackTimer += Time.deltaTime;
                AttackTime attack;
                MeleeAttack.TryGetValue(key,out attack);
                prevkey = key;

                if(attackTimer >= attack.GetStart() &&
                   attackTimer <  attack.GetEndAnim())
                {
                    CheckCombo();
                }

                if (attackTimer >= attack.GetStart() &&
                    attackTimer <  attack.GetEnd())
                {
                    AttackStart?.Invoke();
                    
                }
                else if(attackTimer >= attack.GetEnd() &&
                        attackTimer <= attack.GetEndAnim())
                {
                    AttackEnd?.Invoke();
                    if(Combo)
                    {
                        CallNextAttack = true;
                    }
                }
                else if(attackTimer > attack.GetEndAnim())
                {
                    anim.SetBool(key, false);
                    state = Playerstate.Idle;
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

    class AttackTime
    {
        private string name;
        private float startattack;
        private float endattack;
        private float endanim;

        public AttackTime(string name, float Start, float End, float endanim)
        {
            this.name = name;
            this.startattack = Start;
            this.endattack = End;
            this.endanim = endanim;
        }

        public float GetStart()
        {
            return startattack;
        }

        public float GetEnd()
        {
            return endattack;
        }

        public float GetEndAnim()
        {
            return endanim;
        }
    }
}