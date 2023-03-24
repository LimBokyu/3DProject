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

    private int killCount = 0;

    public void SlowMotion(bool BladeMode)
    {
        Time.timeScale = BladeMode ? SlowDownFactor : normalFactor;
        Time.fixedDeltaTime =  Time.timeScale * 0.02f;
    }

    public void ResetCount()
    {
        killCount = 0;
    }

    public void AddKillCount()
    {
        killCount++;
    }

}
