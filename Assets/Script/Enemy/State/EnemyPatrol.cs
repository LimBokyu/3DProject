using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    private Transform[] patrolPoint;

    private List<Transform> patrolPoints;

    private bool onPatrolPoint = false;
    private int patrolPointnum = 0;

    [SerializeField]
    private bool isCyclingPatrol;

    private void Start()
    {
        patrolPoints = new List<Transform>();

        for(int index = 0; index < patrolPoint.Length; index++)
        {
            patrolPoints.Add(patrolPoint[index]);
        }
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
            if (patrolPointnum >= patrolPoints.Count)
            {
                patrolPointnum = 0;
            }
            else
            {
                patrolPointnum++;
            }
        }
    }
}
