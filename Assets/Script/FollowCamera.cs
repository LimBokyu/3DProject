using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public CinemachineFreeLook followcam;

    private Transform trans;

    private void Start()
    {
        trans = GetComponent<Transform>();
    }

    public void Update()
    {
          trans.position = followcam.transform.position;
    }
}
