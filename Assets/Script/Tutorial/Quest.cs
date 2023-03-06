using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    string info { get; set; }
    string questgoal { get; set; }
    string mission { get; set; }

    int currentvalue { get; set; }
    int goalvalue { get; set; }

    public Quest(string info, string qusetgoal, string mission,
                 int currentvalue, int goalvalue)
    {
        this.info = info;
        this.questgoal = qusetgoal;
        this.mission = mission;
        this.currentvalue = currentvalue;
        this.goalvalue = goalvalue;
    }
}
