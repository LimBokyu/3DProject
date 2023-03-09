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
    private Transform target = null;
    [SerializeField] private Executions nearEnemy;
    // ========================

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask obstacleMask;

    private bool activate = false;
    private bool assassinationOrder = false;
    private bool lockon = false;
    private bool duringassassination = false;

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

    private void Update()
    {
        if (!duringassassination)
            return;

        inputTimer += Time.unscaledDeltaTime;
        if (inputTimer > 2f)
        {
            ppcontroller.BladeModeSetPostProcessing(false);
            timeManager.SlowMotion(false);
            playercam.OffVirtualCam();
            duringassassination = false;
        }
    }

    public void SetActivate(bool value)
    {
        activate = value;
    }

    public bool GetActivate()
    {
        return activate;
    }

    public void CheckAssasination()
    {
        if (activate)
        {
            SetAssassinationUI();
            AssassinationOrder();
        }

    }

    public void AssassinationOrder()
    {
        if (activate && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("�ϻ�");
            assassinationOrder = true;
            DoAssassination();
        }
    }

    public void LockOnTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyMask);

        int nullcount = 0;

        for (int index = 0; index < colliders.Length; index++)
        {
            Vector3 dirToTarget = (colliders[index].transform.position - transform.position).normalized;
            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angleRange * 0.5f);
            Vector3 leftDir = AngleToDir(transform.eulerAngles.y + -angleRange * 0.5f);

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

            if (colliders[index].transform.gameObject.GetComponent<Executions>() != null)
            {
                nearEnemy = colliders[index].transform.gameObject.GetComponent<Executions>();
                lockon = true;
                break;
            }
            else
            {
                nullcount++;
            }
        }

        if(nullcount == colliders.Length)
        {
            nearEnemy = null;
            lockon = false;
        }
    } 

    private void MoveAssassinationPosition()
    {
        transform.position = nearEnemy.GetAssassinationZone().position;
    }

    private void DoAssassination()
    {
        if(assassinationOrder && lockon)
        {
            pc.anim.SetBool("Executions",true);
            MoveAssassinationPosition();
            SetDirection();
            flash.BladeFlash();
            playercam.OnVirtualCam();
            ppcontroller.BladeModeSetPostProcessing(true);
            timeManager.SlowMotion(true);
            duringassassination = true;
        }
    }

    private void EndAssassination()
    {

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
        if (target != null)
            transform.LookAt(target.position);
    }

    public void SetTarget(Transform transform)
    {
        target = transform;
    }
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }

}