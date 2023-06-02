using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private LoadingScreen loadingScreen;

    private AsyncOperation oper;
    private YieldInstruction waitAnimation = new WaitForSeconds(1f);
    private YieldInstruction progressUpdate = new WaitForSeconds(0.1f);
    private CustomYieldInstruction loadingComplete;

    [SerializeField]
    private GameObject progressBar;
    private Slider slider;

    private bool repeat = true;
    private string sceneName = null;

    StringBuilder str = new StringBuilder();
    StringBuilder debugLog = new StringBuilder();

    private void Start()
    {
        loadingComplete = new WaitUntil(() => { return oper.isDone; });
        slider = progressBar.GetComponent<Slider>();
    }

    public void StartLoadingAnimation()
    {
        loadingScreen.StartLoading();
    }

    public void LoadSceneSetting(bool value)
    {
        sceneName = value ? "GameTitle" : "SampleScene";
        StartLoadingAnimation();
        StartCoroutine(StartLoadScene());
    }

    public void LoadingSceneCompleted()
    {
        oper.allowSceneActivation = true;
    }

    public void LoadScene()
    {
        OnScreenProgress();

        oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        debugLog.Append("SceneLoading : ");
        debugLog.Append(sceneName);
        Debug.Log(debugLog.ToString());
        oper.allowSceneActivation = false;
        debugLog.Clear();
        oper.completed += OnLoadComplete;
    }

    private void OnScreenProgress()
    {
        progressBar.SetActive(true);
        StartCoroutine(ShowProgress());
    }

    IEnumerator StartLoadScene()
    {
        yield return waitAnimation;
        LoadScene();
        LoadingSceneCompleted();
    }

    IEnumerator ShowProgress()
    {
        while (repeat)
        {
            yield return progressUpdate;
            str.Append("Progress : ");
            str.Append(oper.progress);
            str.Append("%");
            Debug.Log(str.ToString());
            str.Clear();
            slider.value = oper.progress;
        }
    }

    public void OnLoadComplete(AsyncOperation operation)
    {
        Debug.Log("LoadCompleted!");
        repeat = false;
        StopCoroutine(ShowProgress());
        progressBar.SetActive(false);
        loadingScreen.EndLoading();
    }

    public string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
}
