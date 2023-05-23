using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
