using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameOptions : MonoBehaviour
{
    [SerializeField]
    private GameObject inGameObject;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private SceneManager sceneManager;

    [SerializeField]
    private TimeManager timeManager;
    
    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inGameObject.SetActive(false);
        timeManager.ResumeTime();
        GameManager.Instance.SetBoolOpendMenu();
    }

    public void OpenOptions()
    {
        options.SetActive(true);
    }

    public void ReturnToTitle()
    {
        timeManager.ResumeTime();
        inGameObject.SetActive(false);
        sceneManager.LoadSceneSetting(true);
        GameManager.Instance.SetBoolOpendMenu();
    }
}
