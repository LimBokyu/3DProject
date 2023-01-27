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

    private float NormalAngle;
    private float SearchAngle;

    private float ShootRange = 10f;
    private float SeeRange = 20f;
    private float CombatRange = 30f;
    private bool InShootingRange = false;

    private void Start()
    {
        con = GetComponent<EnemyController>();
        NormalAngle = 120f;
        SearchAngle = 360f;

        viewAngle = NormalAngle;
    }

    private void Update()
    {
        FindTarget();
        InShootRange();
        SetRange();
    }

    private void CheckCombat()
    {
        OnCombat = con.GetCombat();
    }

    private void SetRange()
    {
        CheckCombat();
        viewRange = OnCombat ? SeeRange : CombatRange;
        viewAngle = con.GetSearch() ? SearchAngle : NormalAngle;
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

    private void InShootRange()
    {
        float Range = InShootingRange ? SeeRange : ShootRange;
        Collider[] colliders = Physics.OverlapSphere(transform.position, Range, PlayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            Vector3 rightDir = AngelToDir(transform.eulerAngles.y + viewAngle * 0.5f);

            if (Vector3.Dot(transform.forward, dirToTarget) < Vector3.Dot(transform.forward, rightDir))
                continue;

            float distToTarget = Vector3.Distance(transform.position, colliders[i].transform.position);

            if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                continue;

            Debug.DrawRay(transform.position, dirToTarget * distToTarget, Color.blue);
            InShootingRange = true;
        }

        if (colliders.Length == 0)
            InShootingRange = false;
    }

    public bool isInRange()
    {
        return InShootingRange;
    }
    private Vector3 AngelToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
