using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

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
        public ScreenFlash flash;
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
        private float NormalVig = 0f;
        private float ZoomVig = 0.6f;
        private float NormalChrom = 0f;
        private float ZoomChrom = 1f;
        private float cutTimer = 0.2f;
        private bool cutable = true;
        private Coroutine cutcool = null;
        //======================================
        [Space]

        [Header("Camera")]
        //=========== Player Camera ============
        Camera cam;
        public CinemachineBrain CineMachine;
        public CinemachineFreeLook PlayerCamera;
        private CinemachineComposer[] composers;
        private float normalFov = 30;
        private float zoomFov = 15;
        public  Vector3 zoomOffset;
        private Vector3 normalOffset;
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
            state = Playerstate.Idle;
            Cursor.lockState = CursorLockMode.Locked;
            CameraSet();
            PlayerUISet();
            SetHealth();
            SetAttack();
        }

        public void PlayerUISet()
        {
            currentHealth = maxHealth;
            prevHealth = maxHealth;
            loseHealth = maxHealth;
            concentrate = MaxConcentrate;
        }

        private void CameraSet()
        {
            cam = Camera.main;
            composers = new CinemachineComposer[3];
            for (int i = 0; i < 3; i++)
            {
                composers[i] = PlayerCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            }
            normalOffset = composers[0].m_TrackedObjectOffset;
        }

        public void CameraOffSet(float xOffset)
        {
            foreach (CinemachineComposer com in composers)
            {
                com.m_TrackedObjectOffset.Set(xOffset, com.m_TrackedObjectOffset.y, com.m_TrackedObjectOffset.z);
            }
        }

#region PostProcessing Coroutine

        public IEnumerator SetChrom(float beginchrom, float endchrom)
        {
            float chromval = 0.2f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                if (OnBladeMode)
                {
                    beginchrom += chromval;
                    if (beginchrom >= endchrom)
                    {
                        CameraChromSet(ZoomChrom);
                        yield break;
                    }
                    CameraChromSet(beginchrom);
                }
                else
                {
                    beginchrom -= chromval;
                    if (beginchrom <= endchrom)
                    {
                        CameraChromSet(NormalChrom);
                        yield break;
                    }
                    CameraChromSet(beginchrom);
                }
            }
        }

        public IEnumerator SetVig(float beginvig, float endvig)
        {
            float vigval = 0.05f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.02f);
                if (OnBladeMode)
                {
                    beginvig += vigval;
                    if (beginvig >= endvig)
                    {
                        CameraVigSet(ZoomVig);
                        yield break;
                    }
                    CameraVigSet(beginvig);
                }
                else
                {
                    beginvig -= vigval;
                    if (beginvig <= endvig)
                    {
                        CameraVigSet(NormalVig);
                        yield break;
                    }
                    CameraVigSet(beginvig);
                }
            }
        }

        #endregion

#region PostProcessing Setting

        public void CameraVigSet(float vigval)
        {
            Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value = vigval;
        }

        public void CameraChromSet(float chromval)
        {
            Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().intensity.value = chromval;
        }

        #endregion

        public void Update()
        {
            switch (state)
            {
                case Playerstate.Idle:
                    BladeModeSwitch();
                    Attack();
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
                    BladeMode();
                    break;
                case Playerstate.Attack:
                    Attack();
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
            else if(AttackCount>0)
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

        #region BladeModeCamara Setting

        public IEnumerator Settingoffset(float start, float end)
        {
            float offsetval = 0.3f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                if (OnBladeMode)
                {
                    start += offsetval;
                    if (start >= end)
                    {
                        CameraOffSet(zoomOffset.x);
                        yield break;
                    }
                    CameraOffSet(start);
                }
                else
                {
                    start -= offsetval;
                    if (start <= end)
                    {
                        CameraOffSet(normalOffset.x);
                        yield break;
                    }
                    CameraOffSet(start);
                }
            }
        }

        public IEnumerator SetFov(float start, float end)
        {
            float fovspeed = 1f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                if (OnBladeMode)
                {
                    start -= fovspeed;
                    if (start <= end)
                    {
                        SetFieldOfView(zoomFov);
                        yield break;
                    }
                    SetFieldOfView(start);
                }
                else
                {
                    start += fovspeed;
                    if (start >= end)
                    {
                        SetFieldOfView(normalFov);
                        yield break;
                    }
                    SetFieldOfView(start);
                }
            }
        }

        public void SetFieldOfView(float fov)
        {
            PlayerCamera.m_Lens.FieldOfView = fov;
        }

        #endregion

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

        private Vector3 InputMove()
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
                ModeChanger(OnBladeMode);
            }
        }

        public void BladeMode()
        {
            if(!BladeAttack)
            CutPlane.Rotate(0f, 0f, Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * 100);

            if (OnBladeMode)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    BladeStart?.Invoke();
                    CameraShake();
                    BladeAttack = true;
                    cutable = false;
                    if(cutcool == null)
                        cutcool = StartCoroutine(CutCoolTime());
                }

                if (BladeAttack)
                {
                    attackTimer += Time.unscaledDeltaTime;
                    if (attackTimer >= cutTimer)
                    {
                        BladeEnd?.Invoke();
                        attackTimer = 0;
                        BladeAttack = !BladeAttack;
                    }
                }
            }
        }

        private IEnumerator CutCoolTime()
        {
            yield return new WaitForSecondsRealtime(cutTimer);
            cutable = true;
            cutcool = null;
            float ran = Random.Range(10f, 30f);
            int randompos = Random.Range(0, 2);
            float positive = randompos == 0 ? 1f : -1f;
            CutPlane.Rotate(0f, 0f, ran * positive);
        }

        public void ModeChanger(bool BladeMode)
        {
            OnBladeMode = !BladeMode;
            CutPlane.gameObject.SetActive(OnBladeMode);
            CutPlane.localEulerAngles = Vector3.zero;

            BladeModeCameraSetting();
            BladeModeSetPostProcessing();
            anim.SetBool("BladeMode", OnBladeMode);
            attackTimer = 0;
            timemanager.SlowMotion(OnBladeMode);
            
            string debug = OnBladeMode ? "BladeModeOn" : "BladeModeOff";
            Debug.Log(debug);
            if (!OnBladeMode)
            {
                float y = transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(Vector3.zero);
                transform.Rotate(new Vector3(0, y, 0));
                // ㄴ BladeMode 시에 바라보고 있던 방향으로의 캐릭터를 회전 
            }
            else
                flash.BladeFlash();
        }

        private void CameraShake()
        {
            PlayerCamera.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }

        public void BladeModeSetPostProcessing()
        {
            float startvig = OnBladeMode ? NormalVig : ZoomVig;
            float endvig = OnBladeMode ?  ZoomVig: NormalVig;

            float startchrom = OnBladeMode ? NormalChrom : ZoomChrom;
            float endchrom = OnBladeMode ? ZoomChrom: NormalChrom;

            StartCoroutine(SetChrom(startchrom, endchrom));
            StartCoroutine(SetVig(startvig, endvig));
        }

        public void BladeModeCameraSetting()
        {
            Vector3 startoffset = OnBladeMode ? normalOffset : zoomOffset;
            Vector3 endoffset   = OnBladeMode ? zoomOffset : normalOffset;

            float startfov = OnBladeMode ? normalFov : zoomFov;
            float endfov   = OnBladeMode ? zoomFov : normalFov;

            StartCoroutine(Settingoffset(startoffset.x, endoffset.x));
            StartCoroutine(SetFov(startfov, endfov));
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

        public void Attack()
        {
            if (AttackCount == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    key = "melee";
                    AttackCount = 1;
                    key += AttackCount.ToString();
                    anim.SetBool(key, true);
                }
            }


            if (CallNextAttack)
            {
                key = "melee";
                Vector3 moveVec = InputMove();
                if (moveVec.sqrMagnitude != 0)
                    transform.forward = Vector3.Lerp(transform.forward, moveVec, 0.8f);

                AttackCount = AttackCount >= 3 ? 1 : AttackCount +=1;
                key += AttackCount.ToString();
                attackTimer = 0;
                anim.SetBool(key, true);
                anim.SetBool(prevkey, false);
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
                    anim.SetBool(key, false);
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