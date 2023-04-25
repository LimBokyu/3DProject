using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Executions : MonoBehaviour
{
    EnemyController controller;
    Assassination ass;

    private float range = 1.5f;
    private float outrange = 3f;

    [SerializeField] private Transform offsetTransform;
    [SerializeField] private Transform assassinationZone;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private float angleRange = 30f;
    private bool inRange = false;
    private bool isDead = false;

    private void Start()
    {
        controller = GetComponent<EnemyController>();
    }

    public Transform GetAssassinationZone()
    {
        return assassinationZone;
    }

    public Transform GetOffsetTransform()
    {
        return offsetTransform;
    }

    public void SetAnimSpeed(bool slow)
    {
        controller.anim.speed = slow ? 0.2f : 1f;
    }

    public void GetAssassinationRange()
    {
        if (controller.GetCombatState())
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, range, playerMask);
        Assassination assassination = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angleRange * 0.5f);
            float distToTarget = Vector3.Distance(transform.position, colliders[i].transform.position);

            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.cyan);

            if (Vector3.Dot(-transform.forward, dirToTarget) < Vector3.Dot(-transform.forward, rightDir))
            {
                colliders[i].gameObject.TryGetComponent<Assassination>(out assassination);
                if (assassination != null)
                {
                    ass = assassination;
                    Debug.Log("플레이어가 범위 내로 들어옴");
                    inRange = true;
                    break;
                }
                continue;
            }
        }
    }

    public void Execution()
    {
        controller.Dead();
        isDead = true;
    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
