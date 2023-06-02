using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime
{

    private string name;
    private float startattack;
    private float endattack;
    private float endanim;

    public string Name
    { 
        get { return name; }
        set { name = value; }
    }
    public float StartAttack
    {
        get { return startattack; }
        set { startattack = value; }
    }
    public float EndAttack
    {
        get { return endattack; }
        set { endattack = value; }
    }
    public float Endanim
    {
        get { return endanim; }
        set { endanim = value; }
    }

    public AttackTime(string name, float Start, float End, float endanim)
    {
        Name = name;
        StartAttack = Start;
        EndAttack = End;
        Endanim = endanim;
    }

}
