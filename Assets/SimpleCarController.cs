using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    Rigidbody rb;
    public LayerMask groundLayer;
    public float thrustAmount = 10.0f;
    public float turnAmount = 1.0f;

    public float forwardVel = 0.0f;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(Input.GetKey(KeyCode.W)){
            rb.AddForce(transform.forward * thrustAmount);
        }
        if(Input.GetKey(KeyCode.S)){
            rb.AddForce(transform.forward * -thrustAmount);
        }

        float dot = Vector3.Dot(rb.velocity, transform.forward);
        Vector3 vP = transform.forward * dot;
        forwardVel = vP.magnitude;

        float turn = ((Input.GetKey(KeyCode.D) ? 1:0) +  (Input.GetKey(KeyCode.A) ? -1:0)) * Mathf.Min(forwardVel/1.0f, 1.0f);
        rb.AddTorque(transform.up * turn * turnAmount);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 50.0f, groundLayer)){
            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
            
    }
}
