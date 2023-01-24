using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField]
    private float viewRange;
    [SerializeField, Range(0f, 360f)]
    private float viewAngle;
    [SerializeField]
    private LayerMask PlayerMask;
    [SerializeField]
    private LayerMask obstacleMask;

    public Transform Target;
    public EnemyController con;
    private bool OnCombat = false;

    private void Start()
    {
        con = GetComponent<EnemyController>();
        viewAngle = 120f;
    }

    private void Update()
    {
        FindTarget();
        CheckCombat();
        SetRange();
    }

    private void CheckCombat()
    {
        OnCombat = con.GetCombat();
    }

    private void SetRange()
    {
        viewRange = OnCombat ? 15f : 30f;
    }

    private void SetTarget(Transform trans)
    {
        Target = trans;
    }

    public Transform GetTarget()
    {
        return Target;
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRange, PlayerMask);
        for(int i=0; i< colliders.Length; i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            Vector3 rightDir = AngelToDir(transform.eulerAngles.y + viewAngle * 0.5f);

            if (Vector3.Dot(transform.forward, dirToTarget) < Vector3.Dot(transform.forward, rightDir))
                continue;

            float distToTarget = Vector3.Distance(transform.position, colliders[i].transform.position);

            if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                continue;

            SetTarget(colliders[i].transform);
            Debug.Log("SeePlayer");
            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.red);
        }

        if (colliders.Length == 0)
            SetTarget(null);

    }

    private Vector3 AngelToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
