using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcentrateBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMax(int Concentrate)
    {
        slider.maxValue = Concentrate;
        slider.value = Concentrate;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetGage(int Concentrate)
    {
        slider.value = Concentrate;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}


