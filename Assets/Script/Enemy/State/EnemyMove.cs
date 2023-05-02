using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent nav;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    public void SetNavMeshMoving(bool value)
    {
        nav.isStopped = value;
    }

    public void SetNavAgentDestination(Vector3 pos)
    {
        nav.SetDestination(pos);
    }

    public Vector3 GetNavAgentDestination()
    {
        return nav.destination;
    }

    public void SetNavSpeed(bool value)
    {
        nav.speed = value ? 1f : 3.5f;
    }
}
