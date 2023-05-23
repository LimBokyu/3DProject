using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField]
    private float SlowDownFactor = 0.05f;
    [SerializeField]
    private float normalFactor = 1f;
    [SerializeField]
    private float timeRecovery = 0.02f;

    public void SlowMotion(bool BladeMode)
    {
        Time.timeScale = BladeMode ? SlowDownFactor : normalFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void RecoverNormalTime()
    {
        if(Time.timeScale < 1f)
        {
            Time.timeScale += timeRecovery;
        }
        
        if(Time.timeScale > 1f)
        {
            Time.timeScale = 1f;
        }
    }
}
