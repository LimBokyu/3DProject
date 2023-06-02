using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPoint : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private void BladeModeAimThisPoint(bool value)
    {
        anim.SetBool("Aim", value);
    }

    public bool CutThisPoint()
    {
        gameObject.SetActive(false);
        return true;
    }

    private void OnCollisionStay(Collision collision)
    {
        //collision.gameObject.
    }
}
