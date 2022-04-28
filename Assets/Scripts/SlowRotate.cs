using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, 0.2f));
        //simply rotates the model
    }
}
