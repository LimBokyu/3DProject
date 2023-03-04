using Cinemachine;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera cam;
    private CinemachineBrain brain;
    private CinemachineComposer[] composers;
    public CinemachineFreeLook freeLook;
    private float normalFov = 30;
    private float zoomFov = 15;
    public Vector3 zoomOffset;
    private Vector3 normalOffset;

    private void Start()
    {
        cam = Camera.main;
        brain = cam.GetComponent<CinemachineBrain>();
        CameraSet();
    }
    private IEnumerator Settingoffset(float start, float end, bool OnBladeMode)
    {
        float offsetval = 0.3f;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            if (OnBladeMode)
            {
                start += offsetval;
                if (start >= end)
                {
                    CameraOffSet(zoomOffset.x);
                    yield break;
                }
                CameraOffSet(start);
            }
            else
            {
                start -= offsetval;
                if (start <= end)
                {
                    CameraOffSet(normalOffset.x);
                    yield break;
                }
                CameraOffSet(start);
            }
        }
    }

    private IEnumerator SetFov(float start, float end, bool OnBladeMode)
    {
        float fovspeed = 1f;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            if (OnBladeMode)
            {
                start -= fovspeed;
                if (start <= end)
                {
                    SetFieldOfView(zoomFov);
                    yield break;
                }
                SetFieldOfView(start);
            }
            else
            {
                start += fovspeed;
                if (start >= end)
                {
                    SetFieldOfView(normalFov);
                    yield break;
                }
                SetFieldOfView(start);
            }
        }
    }
    public void BladeModeCameraSetting(bool OnBladeMode)
    {
        Vector3 startoffset = OnBladeMode ? normalOffset : zoomOffset;
        Vector3 endoffset = OnBladeMode ? zoomOffset : normalOffset;

        float startfov = OnBladeMode ? normalFov : zoomFov;
        float endfov = OnBladeMode ? zoomFov : normalFov;

        StartCoroutine(Settingoffset(startoffset.x, endoffset.x , OnBladeMode));
        StartCoroutine(SetFov(startfov, endfov, OnBladeMode));
    }


    public void SetFieldOfView(float fov)
    {
        freeLook.m_Lens.FieldOfView = fov;
    }

    private void CameraSet()
    {
        cam = Camera.main;
        composers = new CinemachineComposer[3];
        for (int i = 0; i < 3; i++)
        {
            composers[i] = freeLook.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
        }
        normalOffset = composers[0].m_TrackedObjectOffset;
    }

    public void CameraOffSet(float xOffset)
    {
        foreach (CinemachineComposer com in composers)
        {
            com.m_TrackedObjectOffset.Set(xOffset, com.m_TrackedObjectOffset.y, com.m_TrackedObjectOffset.z);
        }
    }


}
