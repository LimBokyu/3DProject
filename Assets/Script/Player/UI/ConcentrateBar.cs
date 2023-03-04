using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcentrateBar : MonoBehaviour
{

    //==================================
    public Slider slider;
    // ㄴ> 게이지 슬라이더
    public Gradient gradient;
    // ㄴ> 게이지 그라디에이션
    public Image fill;
    // ㄴ> 게이지 이미지
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


