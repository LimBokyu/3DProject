using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundDetection : MonoBehaviour
{
    EnemyController ec;

    private void Awake()
    {
        ec = GetComponent<EnemyController>();
    }
    public void ReactSound()
    {
        ec.Alert();
    }
}
