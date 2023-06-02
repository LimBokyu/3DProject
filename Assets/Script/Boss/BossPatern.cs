using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatern
{
    // ====== field ======
    private float coolTime;
    private int damage;
    // ===================

    public float CoolTime {
        get { return coolTime;  } 
        set { coolTime = value; }
    }
    public int Damage
    {
        get { return damage;  }
        set { damage = value; }
    }

    public BossPatern(float cool, int damage)
    {
        CoolTime = cool;
        Damage = damage;
    }
}
