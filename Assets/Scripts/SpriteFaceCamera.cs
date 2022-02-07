using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{
    Transform cameraTF;
    Camera cam;

    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        cameraTF = cam.GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        transform.LookAt(cameraTF);
    }
}
