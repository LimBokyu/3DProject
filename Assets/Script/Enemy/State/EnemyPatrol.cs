using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyType { None, Partol }
public class EnemyPatrol : MonoBehaviour
{

    // ===== Component =====
    private EnemyController controller;
    // =====================

    // ===== Patrol Point =====
    [SerializeField]
    private Transform patrolPoint;
    private List<Transform> patrolPoints = new List<Transform>();
    private int patrolPointnum = 0;
    // =========================

    // ===== Patrol Type =====
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private bool isCyclingPatrol;
    // =======================

    // ===== Patrol State =====
    private bool reverse = false;
    private bool isArrival = false;
    private Coroutine patrolPointRoutine = null;
    // ========================

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }
    private void Start()
    {
        for (int index = 0; index < patrolPoint.childCount; index++)
        {
            patrolPoints.Add(patrolPoint.GetChild(index));
        }

        IsPatrolling();

        // Start Patrol
        //controller.SetMoveSpeed();
        GoNextPatrolPoint();
    }

    private void IsPatrolling()
    {
        controller.isPatrol = enemyType == EnemyType.Partol ? true : false;
    }

    public void PatrolBehaviour()
    {
        CheckPatrolPoint();
    }

    private void ArriveAtPatrolPoint()
    {
        isArrival = true;
        controller.SetIsMoving(false);
        if (patrolPointRoutine == null)
        {
            patrolPointRoutine = StartCoroutine(StayPatrolPoint());
        }
    }

    private IEnumerator StayPatrolPoint()
    {
        yield return new WaitForSeconds(2f);
        GoNextPatrolPoint();
        patrolPointRoutine = null;
        isArrival = false;
    }

    private void GoNextPatrolPoint()
    {
        Debug.Log("MoveAgain");
        controller.SetIsMoving(true);
        if (isCyclingPatrol)
        {
            if ((patrolPointnum == 0 && reverse) ||
               (patrolPointnum >= patrolPoints.Count - 1 && !reverse))
            {
                reverse = !reverse;
            }

            patrolPointnum = reverse ? patrolPointnum-- : patrolPointnum++;
        }
        else
        {
            if (patrolPointnum >= patrolPoints.Count - 1)
            {
                patrolPointnum = 0;
            }
            else
            {
                patrolPointnum++;
            }
        }

        //controller.SetNextPatrolPoiont(patrolPoints[patrolPointnum]);
    }

    private void CheckPatrolPoint()
    {
        float dis = Vector3.Distance(transform.position, patrolPoints[patrolPointnum].position);

        if(dis <= 0.2f)
        {
            if (!isArrival)
            {
                ArriveAtPatrolPoint();
            }
        }
    }
}
