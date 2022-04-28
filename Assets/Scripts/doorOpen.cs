using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    GameObject door;
    public bool open;
    float lerpDuration = 0.5f;

    void Awake()
    {
        open = false;
        door = this.gameObject;
    }

    public void openDoor()
    {
        open = true;
        StartCoroutine(rotateDoor());
    }

    IEnumerator rotateDoor()
    {
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, -95, 0);
        while (timeElapsed < lerpDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}
