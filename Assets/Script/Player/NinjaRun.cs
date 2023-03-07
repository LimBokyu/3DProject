using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaRun : MonoBehaviour
{
    PlayerController controller;
    [SerializeField] private LayerMask bulletLayer;
    PlayerCamera playerCam;

    [SerializeField]
    private float range = 3f;
    private void Start()
    {
        controller = GetComponent<PlayerController>();
        playerCam = GetComponent<PlayerCamera>();   
    }
    private void ReflectBullet()
    {
        Collider[] colliders = Physics.OverlapSphere(controller.transform.position, range, bulletLayer);

        if (colliders.Length <= 0)
            return;

        foreach(Collider col in colliders)
        {
            Vector3 pos = col.transform.position;
            playerCam.CameraShake();
            Destroy(col.gameObject);
        }
    }

    public void NinjaRunBehaviour()
    {
        ReflectBullet();
    }

    private void SetPostProcessing()
    {

    }
}
