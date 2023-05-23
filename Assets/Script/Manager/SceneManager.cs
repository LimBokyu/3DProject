using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private LoadingScreen loadingScreen;
    private AsyncOperation oper;
    private YieldInstruction ins = new WaitForSeconds(1f);
    private CustomYieldInstruction ins2;

    private bool trigger = false;
    private bool isLoading = false;

    private void Awake()
    {
        loadingScreen = GetComponent<LoadingScreen>();
    }

    private void Start()
    {
        ins2 = new WaitUntil(() => { return oper.isDone; });
    }

    public void LoadScene()
    {
        if (!trigger)
        {
            isLoading = true;
            loadingScreen.StartLoading();

            //UnityEngine.SceneManagement.SceneManager.LoadScene("asd");
            oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("AfterLoadScene");
            oper.allowSceneActivation = false;
            oper.completed += OnLoadComplete;

            StartCoroutine(Routine());
            trigger = true;
        }
    }

    IEnumerator Routine()
    {
        yield return ins;
        yield return ins2;
        oper.allowSceneActivation = true;
        trigger = false;
    }

    public void OnLoadComplete(AsyncOperation oper)
    {
        loadingScreen.EndLoading();
    }
}
