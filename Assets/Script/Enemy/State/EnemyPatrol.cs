using System.Collections.Generic;
using UnityEngine;

enum EnemyType { None, Partol }
public class EnemyPatrol : MonoBehaviour
{
    private EnemyController controller;

    [SerializeField]
    private Transform[] patrolPoint;

    [SerializeField]
    private EnemyType enemyType;

    private List<Transform> patrolPoints;

    private int patrolPointnum = 0;

    private bool isCyclingPatrol;
    private bool reverse = false;

    private void Awake()
    {
        isCyclingPatrol = enemyType == EnemyType.None ? false : true;
        controller = GetComponent<EnemyController>();
    }
    private void Start()
    {
        patrolPoints = new List<Transform>();

        for(int index = 0; index < patrolPoint.Length; index++)
        {
            patrolPoints.Add(patrolPoint[index]);
        }

        IsPatrolling();
    }

    private void IsPatrolling()
    {
        controller.isPatrol = enemyType == EnemyType.Partol ? true : false;
    }

    public void PatrolBehaviour()
    {
        CheckPatrolPoint();
    }

    private void CheckPatrolPoint()
    {
        float dis = Vector3.Distance(transform.position, patrolPoints[patrolPointnum].position);

        if(dis <= 0.5f)
        {
            if(isCyclingPatrol)
            {
                if((patrolPointnum == 0 && reverse) ||
                   (patrolPointnum >= patrolPoints.Count && !reverse))
                {
                    reverse = !reverse;
                }

                patrolPointnum = reverse ? patrolPointnum-- : patrolPointnum++;
            }
            else
            {
                if (patrolPointnum >= patrolPoints.Count)
                {
                    patrolPointnum = 0;
                }
                else
                {
                    patrolPointnum++;
                }
            }

            controller.SetNextPatrolPoiont(patrolPoint[patrolPointnum]);
        }
    }
}
