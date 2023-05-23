using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Start Game");
    }
    public void OpenOptions()
    {
        Debug.Log("Click Options");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
