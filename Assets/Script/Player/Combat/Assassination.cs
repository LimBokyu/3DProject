using Cinemachine;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassination : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] private TimeManager timeManager;
    private PlayerCamera playercam;
    private PostProcessingController ppcontroller;
    [SerializeField] LayerMask enemyMask;
 
    private bool activate = false;
    private bool assassinationOrder = false;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        playercam = GetComponent<PlayerCamera>();
        ppcontroller =GetComponent<PostProcessingController>();
    }

    public void SetActivate(bool value)
    {
        activate = value;
    }
    
    public bool GetActivate()
    {
        return activate;
    }

    public void CheckAssasination()
    {
        if (activate)
        {
            SetAssassinationUI();
            AssassinationOrder();
        }

    }

    public void AssassinationOrder()
    {
        if(activate && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("¾Ï»ì");
            assassinationOrder = true;
            DoAssassination();
        }
    }

    private void DoAssassination()
    {
        if(assassinationOrder)
        {
            pc.anim.SetBool("Executions",true);
            //pc.anim.updateMode = AnimatorUpdateMode.Normal;
            playercam.OnVirtualCam();
            ppcontroller.BladeModeSetPostProcessing(true);
            timeManager.SlowMotion(true);
        }
    }

    private void SetAssassinationUI()
    {
        //go.SetActive(true);
    }
}
