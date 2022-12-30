using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;

    private float currentZoom = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position;
        currentZoom = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
