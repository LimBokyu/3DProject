using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    SceneManager sceneManager;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private GameObject backToTitleButton;

    public void OptionsClose()
    {
        options.SetActive(false);
    }

    public void ReturnToTitle()
    {
        sceneManager.LoadSceneSetting(true);
    }
}
