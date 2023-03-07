using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassination : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] LayerMask enemyMask;

    GameObject go;

    private bool activate = false;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
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
            if(activate)
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
        }
    }

    private void SetAssassinationUI()
    {
        //go.SetActive(true);
    }
}
