using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField]
    private float SlowDownFactor = 0.05f;

    public void SlowMotion()
    {
        Time.timeScale = SlowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void SlowMotionOut()
    {
        Time.timeScale = 1f;
    }

}
