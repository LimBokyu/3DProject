using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [SerializeField]
    private PoolManager pool;
    [SerializeField]
    private TimeManager time;
    [SerializeField]
    private SceneManager scene;
    [SerializeField]
    private GameObject inGameOptions;
    [SerializeField]
    private GameObject options;

    private bool isTitle = true;
    private bool opendMenu = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GameManager Instance
    {
        get 
        {
            if (instance == null)
            {
                return null;
            }
            return instance; 
        }
    }

    public GameObject GetOptions()
    {
        return options;
    }

    public SceneManager GetSceneManager()
    {
        return scene;
    }

    private void SetDefault()
    {
    }

    private bool OnTitle()
    {
        return scene.GetCurrentSceneName() != "GameTitle" ? false : true;
    }

    public void SetBoolOpendMenu()
    {
        opendMenu = false;
    }

    private void Update()
    {
        if (!OnTitle())
        {
            if (!opendMenu)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    opendMenu = true;
                    Cursor.lockState = CursorLockMode.None;
                    time.StopTime();
                    inGameOptions.SetActive(true);
                }
            }
        }
    }
}
