using Cinemachine;
using Player;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Assassination : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;

    // ====== Component ======
    private PlayerController pc;
    private PlayerCamera playercam;
    private PostProcessingController ppcontroller;
    private ScreenFlash flash;
    // ========================

    // ===== Target Enemy =====
    [SerializeField] private Transform target = null;
    [SerializeField] private Executions nearEnemy;
    // ========================

    [SerializeField] private AssassinationUI ui;

    [SerializeField] private Transform playermid;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 executionszone = Vector3.zero;

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask obstacleMask;

    private bool assassinationOrder = false;
    private bool lockon = false;
    private bool duringAssassination = false;

    private float range = 1.5f;
    private float angleRange = 30f;
    private float inputTimer = 0;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        playercam = GetComponent<PlayerCamera>();
        ppcontroller = GetComponent<PostProcessingController>();
    }
    private void Start()
    {
        flash = pc.flash;
    }

    public void AssassinationBehaviour()
    {
        inputTimer += Time.unscaledDeltaTime;
        if (inputTimer >= 2f)
        {
            EndAssassination();
            inputTimer = 0f;
        }
    }

    public bool GetLockOn()
    {
        return lockon;
    }

    public void CheckAssasination()
    {
        if (lockon)
        {
            SetAssassinationUI();
            AssassinationOrder();
        }
        else if (!lockon)
        {
            NullTarget();
        }

    }

    private void SetPlayerRotation()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0).normalized;
    }

    public void AssassinationOrder()
    {
        if (lockon && Input.GetButtonDown("Fire1"))
        {
            ui.EndAnimation();
            pc.invincible = true;
            Debug.Log("암살");
            assassinationOrder = true;
            DoAssassination();
        }
    }

    private void NullTarget()
    {
        nearEnemy = null;
        ui.EndAnimation();
    }

    public void LockOnTarget()
    {
        if (assassinationOrder)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyMask);

        int nullcount = 0;

        for (int index = 0; index < colliders.Length; index++)
        {
            Vector3 dirToTarget = (colliders[index].transform.position - transform.position).normalized;
            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angleRange * 0.5f);
            Vector3 leftDir = AngleToDir(transform.eulerAngles.y + -angleRange * 0.5f);

            if (colliders[index].transform.gameObject.GetComponent<Executions>() != null)
            {
                lockon = true;
                nearEnemy = colliders[index].transform.gameObject.GetComponent<Executions>();
                nearEnemy.GetAssassinationRange();
                ui.startAnimation();
                break;
            }
            else
            {
                nullcount++;
            }

            if (Vector3.Dot(transform.forward, dirToTarget) < Vector3.Dot(transform.forward, rightDir))
            {
                lockon = false;
                continue;
            }

            float distToTarget = Vector3.Distance(transform.position, colliders[index].transform.position);

            if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
            {
                lockon = false;
                continue;
            }

            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.red);
            Debug.DrawRay(transform.position, rightDir * range, Color.magenta);
            Debug.DrawRay(transform.position, leftDir * range, Color.magenta);
        }

        if (nullcount == colliders.Length)
        {
            lockon = false;
        }
    }

    private void MoveAssassinationPosition()
    {
        offset = nearEnemy.GetAssassinationZone().position;
        transform.position = offset;
    }

    private void DoAssassination()
    {
        if (assassinationOrder && lockon)
        {
            pc.anim.SetBool("Executions", true);
            OnAssassinationSetting();
            OnAssassinationEffect();
            nearEnemy.SetAnimSpeed(true);
            nearEnemy.Execution();
            SetAssassinationBool();
        }
    }

    private void EndAssassination()
    {
        pc.invincible = false;
        pc.anim.SetBool("Executions", false);
        nearEnemy.SetAnimSpeed(false);
        nearEnemy = null;
        OffAssassination();
        duringAssassination = false;
        assassinationOrder = false;
        lockon = false;
        offset = Vector3.zero;
        pc.executions = false;
        SetPlayerRotation();
        
        // 암살 수행이 끝났을 경우 세팅을 원상복귀하기 위한 함수
    }

    private void OnAssassinationEffect()
    {
        ppcontroller.OverDrivePostProcessing();
        flash.BladeFlash();
        playercam.OnVirtualCam();
        timeManager.SlowMotion(true);

        // 암살 수행시의 효과를 세팅해주는 함수
    }

    private void OffAssassination()
    {
        ppcontroller.BladeModeSetPostProcessing(false);
        timeManager.SlowMotion(false);
        playercam.OffVirtualCam();

        // 암살 종료시의 효과를 꺼주는 함수
    }

    private void OnAssassinationSetting()
    {
        MoveAssassinationPosition();
        SetDirection();
    }

    private void SetAssassinationBool()
    {
        duringAssassination = true;
        pc.executions = true;
    }

    private void BloodLust()
    {

    }

    private void SetAssassinationUI()
    {
        //go.SetActive(true);
    }

    private void SetDirection()
    {
        target = nearEnemy.GetOffsetTransform();
        transform.LookAt(target.position);
    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }

}
