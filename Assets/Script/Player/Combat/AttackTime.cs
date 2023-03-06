using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime
{

    private string name;
    public float startattack { get; set; }
    public float endattack { get; set; }
    public float endanim { get; set; }

    public AttackTime(string name, float Start, float End, float endanim)
    {
        this.name = name;
        this.startattack = Start;
        this.endattack = End;
        this.endanim = endanim;
    }

}
