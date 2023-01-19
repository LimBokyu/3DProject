using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public Image image;

    [SerializeField]
    private float flashTime = 0.25f;
    [SerializeField]
    private float Alpha = 0.5f;
    [SerializeField]
    private Color BladeModeFlash = Color.white;
    [SerializeField]
    private Color HurtFlash = Color.red;
    [SerializeField]
    private Color ChangeColor;
    public bool BladeMode = true;

    private Coroutine flashcoroutine = null;


    private void Start()
    {
        SetColor();
    }
    public void SetColor()
    {
        ChangeColor = BladeMode ? BladeModeFlash : HurtFlash;
    }

    public void Hurt()
    {
        BladeMode = false;
        SetColor();
        OrderFlash();
    }

    public void OrderFlash()
    {
        StartFlash(flashTime, Alpha, ChangeColor);
    }

    public void StartFlash(float flash, float alpha, Color color)
    {
        image.color = color;

        alpha = Mathf.Clamp(alpha, 0, 1);

        if (flashcoroutine != null)
            StopCoroutine(flashcoroutine);

        flashcoroutine = StartCoroutine(Flash(flash, alpha));
    }

    private IEnumerator Flash(float flash, float alpha)
    {
        float duration = flash / 2;
        for(float time = 0; time <= duration; time+=Time.unscaledDeltaTime)
        {
            Color curColor = image.color;
            curColor.a = Mathf.Lerp(0, alpha, time / duration);
            image.color = curColor;
            yield return null;
        }

        duration = flash / 2;
        for (float time = 0; time <= duration; time += Time.unscaledDeltaTime)
        {
            Color curColor = image.color;
            curColor.a = Mathf.Lerp(alpha, 0 , time / duration);
            image.color = curColor;
            yield return null;
        }

        image.color = new Color32(0, 0, 0, 0);
        BladeMode = true;
        SetColor();
    }
}
