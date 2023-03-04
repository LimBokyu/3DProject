using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegainBar : MonoBehaviour
{
    public Slider regainslider;

    public void SetMaxRegainHealth(int health)
    {
        regainslider.maxValue = health;
        regainslider.value = health;
    }

    public void SetRegainHealth(int regain)
    {
        regainslider.value = regain;
    }
}
