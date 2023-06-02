using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : MonoBehaviour
{
    [SerializeField]
    private BossController controller;

    public void BossDodge()
    {
        OnInvincible();
    }


    private void OnInvincible()
    {
        controller.Invincible = true;
    }

    private void OffInvincible()
    {
        controller.Invincible = false;
    }
}
