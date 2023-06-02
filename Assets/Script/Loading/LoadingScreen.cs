using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Canvas canvas;

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    public void StartLoading()
    {
        anim.Play("Start");
    }

    public void EndLoading()
    {
        anim.Play("End");
    }
}
