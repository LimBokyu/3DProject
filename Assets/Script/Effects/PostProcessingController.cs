using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static Cinemachine.CinemachineOrbitalTransposer;

public class PostProcessingController : MonoBehaviour
{
    private float normalFov = 30;
    private float zoomFov = 15;
    private float NormalVig = 0f;
    private float ZoomVig = 0.6f;
    private float NormalChrom = 0f;
    private float ZoomChrom = 1f;

    private Color blademodecolor;
    private Color overdrivecolor;

    private Vignette vig;
    private ChromaticAberration chrom;
    private ColorGrading grading;

    private float overdriveintencity = 0.4f;
    private float overdrivesmoothness = 0.8f;

    private void Awake()
    {
        vig = Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<Vignette>();
        chrom = Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<ChromaticAberration>();
        grading = Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<ColorGrading>();

        //blademodecolor = new Color(72f, 168f, 214f, 255f);
        blademodecolor = vig.color;
        overdrivecolor = new Color(214f, 72f, 88f, 255f);
    }

    private IEnumerator SetChrom(float beginchrom, float endchrom, bool OnBladeMode)
    {
        float chromval = 0.2f;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            if (OnBladeMode)
            {
                beginchrom += chromval;
                if (beginchrom >= endchrom)
                {
                    CameraChromSet(ZoomChrom);
                    yield break;
                }
                CameraChromSet(beginchrom);
            }
            else
            {
                beginchrom -= chromval;
                if (beginchrom <= endchrom)
                {
                    CameraChromSet(NormalChrom);
                    yield break;
                }
                CameraChromSet(beginchrom);
            }
        }
    }

    private IEnumerator SetVig(float beginvig, float endvig, bool OnBladeMode)
    {
        float vigval = 0.05f;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            if (OnBladeMode)
            {
                beginvig += vigval;
                if (beginvig >= endvig)
                {
                    CameraVigSet(ZoomVig);
                    yield break;
                }
                CameraVigSet(beginvig);
            }
            else
            {
                beginvig -= vigval;
                if (beginvig <= endvig)
                {
                    CameraVigSet(NormalVig);
                    yield break;
                }
                CameraVigSet(beginvig);
            }
        }
    }

    public void BladeModeSetPostProcessing(bool OnBladeMode)
    {
        float startvig = OnBladeMode ? NormalVig : ZoomVig;
        float endvig = OnBladeMode ? ZoomVig : NormalVig;

        float startchrom = OnBladeMode ? NormalChrom : ZoomChrom;
        float endchrom = OnBladeMode ? ZoomChrom : NormalChrom;

        vig.color.Override(blademodecolor);
        StartCoroutine(SetChrom(startchrom, endchrom, OnBladeMode));
        StartCoroutine(SetVig(startvig, endvig, OnBladeMode));
    }

    public void OverDrivePostProcessing()
    {
        vig.color.Override(Color.red);
        BladeModeSetPostProcessing(true);
    }

    private void CameraVigSet(float vigval)
    {
        vig.intensity.value = vigval;
    }

    private void CameraChromSet(float chromval)
    {
        chrom.intensity.value = chromval;
    }

    public void OnOffColorGrading(bool value)
    {
        //Camera.main.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<ColorGrading>().active = value;
        //grading.active = value;
        grading.enabled.Override(value);
    }
}
