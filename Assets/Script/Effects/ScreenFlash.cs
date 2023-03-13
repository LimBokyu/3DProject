using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    private Image image;
    public Image Hurtimage;
    public Image flashImage;

    private float flashTime = 0.25f;
    private float Alpha;
    private Color BladeModeFlash = Color.white;
    private Color ChangeColor;
    private bool BladeMode = true;

    private Coroutine flashcoroutine = null;

    private void Start()
    {
        SetColor();
    }
    private void SetColor()
    {
        image = BladeMode ? flashImage : Hurtimage;
        Alpha = BladeMode ? 0.5f : 1f;
        ChangeColor = BladeModeFlash;
    }

    public void Hurt()
    {
        BladeMode = false;
        SetColor();
        OrderFlash();
    }

    public void Dead()
    {
        BladeMode = false;
        SetColor();
        DeadUIFadeIn();
    }

    public void BladeFlash()
    {
        BladeMode = true;
        SetColor();
        OrderFlash();
    }

    private void OrderFlash()
    {
        StartFlash(flashTime, Alpha, ChangeColor);
    }

    private void DeadUIFadeIn()
    {
        OrderFadeInUI(flashTime, Alpha, ChangeColor);
    }

    private void OrderFadeInUI(float flash, float alpha, Color color)
    {
        image.color = color;

        alpha = Mathf.Clamp(alpha, 0, 1);

        if (flashcoroutine != null)
        {
            StopCoroutine(flashcoroutine);
        }
            

        flashcoroutine = StartCoroutine(FadeInUI(flash, alpha));
    }

    private void StartFlash(float flash, float alpha, Color color)
    {
        image.color = color;

        alpha = Mathf.Clamp(alpha, 0, 1);

        if (flashcoroutine != null)
            StopCoroutine(flashcoroutine);

        flashcoroutine = StartCoroutine(Flash(flash, alpha));
    }

    private IEnumerator FadeInUI(float flash, float alpha)
    {
        float duration = flash / 2f;
        float time = 0;

        while(true)
        {
            time += Time.unscaledDeltaTime;
            Color curColor = image.color;
            Mathf.Lerp(0, alpha, time / flash);
            image.color = curColor;

            if (duration <= time)
            {
                break;
            }
            
            yield return null;
        }

    }

    private IEnumerator Flash(float flash, float alpha)
    {
        float duration = flash / 2;

        for (float time = 0; time <= flash; time += Time.unscaledDeltaTime)
        {
            Color curColor = image.color;
            curColor.a = time <= duration ?
            2 * Mathf.Lerp(0, alpha, time / flash) :
            2 * Mathf.Lerp(alpha, 0, time / flash);
            image.color = curColor;
            yield return null;
        }

        image.color = new Color32(0, 0, 0, 0);
    }
}
