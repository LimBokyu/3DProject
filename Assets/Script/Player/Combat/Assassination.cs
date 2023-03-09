using Cinemachine;
using Player;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Assassination : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;

    private PlayerController pc;
    private PlayerCamera playercam;
    private PostProcessingController ppcontroller;
    private ScreenFlash flash;
    private Transform target = null;

    [SerializeField] private Executions[] nearenemies;
    [SerializeField] private Executions nearEnemy;

    private Vector3 assassinationoffset = new Vector3(0f, 0f, -0.2f);

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask obstacleMask;

    private bool activate = false;
    private bool assassinationOrder = false;
    private bool lockon = false;

    private float radius = 0.3f;
    private float angleRange = 30f;

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
            Debug.Log("¾Ï»ì");
            assassinationOrder = true;
            DoAssassination();
        }
    }

    public void LockOnTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyMask);

        int count = 0;

        for (int index = 0; index < colliders.Length; index++)
        {
            Vector3 dirToTarget = (colliders[index].transform.position - transform.position).normalized;
            Vector3 rightDir = AngelToDir(transform.eulerAngles.y + angleRange * 0.5f);

            if (Vector3.Dot(transform.forward, dirToTarget) < Vector3.Dot(transform.forward, rightDir))
                continue;

            float distToTarget = Vector3.Distance(transform.position, colliders[index].transform.position);

            if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                continue;

            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.yellow);

            if (colliders[index].gameObject.GetComponent<Executions>() != null)
            {
                nearenemies[count] = colliders[index].gameObject.GetComponent<Executions>();
                count++;
            }
        }
    } 

    private void MoveAssassinationPosition()
    {
        if (nearenemies.Length == 0)
        {
            lockon = false;
            return;
        }
        else if (nearenemies.Length == 1)
        {
            SetAssassinationPositionTarget(nearenemies[0].GetAssassinationZone().position);
        }
        else
        {
            nearEnemy = nearenemies[0].gameObject.GetComponent<Executions>();

            for (int index = 0; index < nearenemies.Length; index++)
            {
                nearEnemy = (nearenemies[index].transform.position - transform.position).sqrMagnitude <
                            (nearenemies[index + 1].transform.position - transform.position).sqrMagnitude ?
                             nearenemies[index + 1].gameObject.GetComponent<Executions>() :
                             nearenemies[index].gameObject.GetComponent<Executions>();
            }

            SetAssassinationPositionTarget(nearEnemy.GetAssassinationZone().position);
        }

    }

    private void SetAssassinationPositionTarget(Vector3 pos)
    {
        transform.position = pos;
    }

    private void DoAssassination()
    {
        if(assassinationOrder)
        {
            pc.anim.SetBool("Executions",true);
            //pc.anim.updateMode = AnimatorUpdateMode.Normal;
            MoveAssassinationPosition();
            SetDirection();
            flash.BladeFlash();
            playercam.OnVirtualCam();
            ppcontroller.BladeModeSetPostProcessing(true);
            timeManager.SlowMotion(true);
        }
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
    private Vector3 AngelToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }

}
