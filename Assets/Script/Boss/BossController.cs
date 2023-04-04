using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Transform trans;
    private Rigidbody rigid;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        trans = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }
}
