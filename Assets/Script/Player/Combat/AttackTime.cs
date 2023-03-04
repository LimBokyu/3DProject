using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime
{

    private string name;
    private float startattack;
    private float endattack;
    private float endanim;

    public AttackTime(string name, float Start, float End, float endanim)
    {
        this.name = name;
        this.startattack = Start;
        this.endattack = End;
        this.endanim = endanim;
    }

    public float GetStart()
    {
        return startattack;
    }

    public float GetEnd()
    {
        return endattack;
    }

    public float GetEndAnim()
    {
        return endanim;
    }
}
