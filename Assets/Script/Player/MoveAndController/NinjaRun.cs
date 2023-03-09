using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaRun : MonoBehaviour
{
    PlayerController controller;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private LayerMask obstacleMask;
    PlayerCamera playerCam;

    public Transform playerMiddle;

    [SerializeField]
    private float range = 1.5f;

    private float reflectAngle = 240f;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerCam = GetComponent<PlayerCamera>();

    }
    private void ReflectBullet()
    {
        Collider[] colliders = Physics.OverlapSphere(playerMiddle.position, range, bulletLayer);

        if (colliders.Length <= 0)
            return;

        for(int index = 0; index < colliders.Length; index++)
        {
            Vector3 targetDir = (colliders[index].transform.position - playerMiddle.position).normalized;
            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + reflectAngle * 0.5f);

            if (Vector3.Dot(playerMiddle.transform.forward, targetDir) < Vector3.Dot(playerMiddle.transform.forward, rightDir))
                continue;

            float distToTarget = Vector3.Distance(transform.position, colliders[index].transform.position);

            if (Physics.Raycast(transform.position, targetDir, distToTarget, obstacleMask))
                continue;


        }

        foreach (Collider col in colliders)
        {
            Vector3 pos = col.transform.position;
            playerCam.CameraShakeNormalTimeScale();
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
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
