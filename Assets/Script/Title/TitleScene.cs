using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    private GameObject optionMenu;

    [SerializeField]
    private SceneManager scene;

    private void Start()
    {
        optionMenu = GameManager.Instance.GetOptions();
        scene = GameManager.Instance.GetSceneManager();
    }
    public void StartGame()
    {
        Debug.Log("Start Game");
        scene.LoadSceneSetting(false);
    }
    public void OpenOptions()
    {
        Debug.Log("Click Options");
        optionMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
