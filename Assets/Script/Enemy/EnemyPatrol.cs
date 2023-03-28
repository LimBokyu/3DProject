using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    private Transform patrolPoint;

    public void PatrolBehaviour()
    {
        Vector3 startposition = transform.position;

        if(Vector3.SqrMagnitude(patrolPoint.position - transform.position) <= 0.5f)
        {

        }

        Vector3.Lerp(startposition, transform.position, 0.5f);
    }

   
}
