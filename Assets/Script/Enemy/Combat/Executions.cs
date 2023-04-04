using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Executions : MonoBehaviour
{
    EnemyController controller;

    private float range = 1.5f;
    private float outrange = 3f;

    [SerializeField] private Transform offsetTransform;
    [SerializeField] private Transform assassinationZone;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private float angleRange = 30f;
    private bool inRange = false;

    private void Start()
    {
        controller = GetComponent<EnemyController>();
    }

    public Transform GetAssassinationZone()
    {
        return assassinationZone;
    }

    public void GetAssassinationRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, playerMask);
        Assassination assassination = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angleRange * 0.5f);

            if (Vector3.Dot(-transform.forward, dirToTarget) < Vector3.Dot(-transform.forward, rightDir))
            {
                colliders[i].gameObject.TryGetComponent<Assassination>(out assassination);
                if(assassination != null)
                {
                    assassination.SetActivate(false);
                }
                continue;
            }

            float distToTarget = Vector3.Distance(transform.position, colliders[i].transform.position);

            if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                continue;

            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.green);

            colliders[i].gameObject.TryGetComponent<Assassination>(out assassination);

            if (assassination != null)
            {
                Debug.Log("플레이어가 범위 내로 들어옴");
                inRange = true;
                assassination.SetTarget(offsetTransform);
                assassination.SetActivate(inRange);
            }
            else
            {
                inRange = false;
            }
        }

        assassination = null;

        if (colliders.Length == 0)
        {
            CheckOutRanged();
        }
    }

    public void Execution()
    {
        controller.Dead();
        Destroy(this);
    }

    private void CheckOutRanged()
    {
        Assassination assassination = null;
        if (inRange)
            return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, outrange, playerMask);
        for(int index=0; index < colliders.Length; index++)
        {
            colliders[index].gameObject.TryGetComponent<Assassination>(out assassination);
            if (assassination != null)
            {
                assassination.SetActivate(false);
            }
        }
    }
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
