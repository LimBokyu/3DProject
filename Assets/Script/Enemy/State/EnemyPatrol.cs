using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EnemyType { None, Partol }
public class EnemyPatrol : MonoBehaviour
{
    // ===== Component =====
    private EnemyController controller;
    private EnemyMove enemyMove;
    // =====================

    // ===== Patrol Point =====
    [SerializeField]
    private Transform patrolPoint;
    [SerializeField]
    List<Transform> patrolPoints = new List<Transform>();
    [SerializeField]
    private Transform destination;
    [SerializeField]
    private int patrolPointNum = 0;
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

    // ======= Others =======
    private YieldInstruction patrolStayTime = new WaitForSeconds(3.5f);


    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        enemyMove = GetComponent<EnemyMove>();
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
        SetIsMoving(true);
        if (patrolPointRoutine == null)
        {
            patrolPointRoutine = StartCoroutine(StayPatrolPoint());
        }
    }

    private void SetIsMoving(bool value)
    {
        isArrival = value;
        controller.SetIsMoving(!value);
        enemyMove.SetNavMeshMoving(value);
    }

    private IEnumerator StayPatrolPoint()
    {
        yield return patrolStayTime;
        GoNextPatrolPoint();
        patrolPointRoutine = null;
        SetIsMoving(false);
    }

    private void GoNextPatrolPoint()
    {
        Debug.Log("MoveAgain");
        controller.SetIsMoving(true);
        if (isCyclingPatrol)
        {
            CyclingPatrol();
        }
        else
        {
            RoundTripPatrol();
        }

        enemyMove.SetNavAgentDestination(patrolPoints[patrolPointNum].position);
        destination = patrolPoints[patrolPointNum];
    }

    private void CyclingPatrol()
    {
        if ((patrolPointNum == 0 && reverse) ||
        (patrolPointNum >= patrolPoints.Count - 1 && !reverse))
        {
            reverse = !reverse;
        }

        patrolPointNum = reverse ? patrolPointNum-- : patrolPointNum++;
    }

    private void RoundTripPatrol()
    {
        if (patrolPointNum >= patrolPoints.Count -1)
        {
            patrolPointNum = 0;
        }
        else
        {
            patrolPointNum++;
        }
    }

    private void CheckPatrolPoint()
    {
        if(enemyMove.RemainDistance() <= 0.2f)
        {
            if (!isArrival)
            {
                ArriveAtPatrolPoint();
            }
        }
    }
}
