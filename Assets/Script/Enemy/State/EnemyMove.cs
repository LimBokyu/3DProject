using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent nav;

    private float walkSpeed = 1f;
    private float runSpeed = 3.5f;

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
        nav.speed = value ? walkSpeed : runSpeed;
    }
}
