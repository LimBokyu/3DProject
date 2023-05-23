using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PoolManager pool;
    [SerializeField]
    private TimeManager time;
    [SerializeField]
    private SceneManager scene;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
