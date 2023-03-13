using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOff : MonoBehaviour
{
    [SerializeField] private Transform healthbar;
    [SerializeField] private Transform concentratebar;

    public void SetOffUI()
    {
        healthbar.gameObject.SetActive(false);
        concentratebar.gameObject.SetActive(false);
    }
}
