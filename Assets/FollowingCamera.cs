using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 focus;

    private void Start()
    {
        transform.position = Vector3.Lerp(target.position, focus, 0.75f);
    }

    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, Vector3.Lerp(target.position, focus, 0.75f), 0.02f);
    }
}
