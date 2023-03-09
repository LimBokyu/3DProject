using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinationUI : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void startAnimation()
    {
        anim.Play("Click");
    }

    public void EndAnimation()
    {
        anim.Play("None");
    }
}
