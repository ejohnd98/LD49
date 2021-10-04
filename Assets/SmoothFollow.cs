using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float followSpeed = 2f;
    public float rotationFollowSpeed = 2f;
    public bool matchRotation = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);
        if(matchRotation){
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationFollowSpeed);
        }
    }
}
