using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveController : MonoBehaviour
{
    public WheelCollider[] frontWheels, rearWheels;
    public GameObject[] frontWheelMeshes, rearWheelMeshes;
    public GameObject steeringWheel;
    public Vector3 com;
    public Rigidbody carRB;
    public float torque = 200.0f;
    public float frontTurnAngle = 30.0f, rearTurnAngle = 0.0f, turningSpeed = 10.0f;

    public float forwardVel = 0.0f;
    public float maxVel;
    public float wheelPosition = 0.0f;

    public bool cruiseControl = false;

    public void ToggleCruiseControl(){
        cruiseControl = !cruiseControl;
    }

    public AudioSource ambient;

    // Update is called once per frame
    void Update()
    {
        // Get Input
        float a = (Input.GetKey(KeyCode.W)?1:0) + (Input.GetKey(KeyCode.S)?-1:0);
        if(cruiseControl){
            a = 1.0f;
        }
        float turn = (Input.GetKey(KeyCode.D)?1:0) + (Input.GetKey(KeyCode.A)?-1:0);
        
        wheelPosition += (turn - wheelPosition)*turningSpeed*Time.deltaTime;

        Vector3 rot = steeringWheel.transform.eulerAngles;
        rot.z = wheelPosition * -80.0f;
        steeringWheel.transform.eulerAngles = rot;

        DriveWheels(a, wheelPosition);
        carRB.centerOfMass = com;

        float dot = Vector3.Dot(carRB.velocity, transform.forward);
        Vector3 vP = transform.forward * dot;
        forwardVel = vP.magnitude;
        ambient.pitch = 0.4f + Mathf.Clamp(forwardVel / maxVel, 0.0f, 1.0f);
    }

    void DriveWheels(float a, float turn){
        a = Mathf.Clamp(a, -1, 1);
        turn = Mathf.Clamp(turn, -1, 1);

        float thrustTorque =  a * torque;
        for(int i = 0; i < frontWheels.Length; i++){
            frontWheels[i].motorTorque = thrustTorque;
            frontWheels[i].steerAngle = turn*frontTurnAngle;

            Quaternion rot;
            Vector3 pos;

            frontWheels[i].GetWorldPose(out pos, out rot);

            frontWheelMeshes[i].transform.position = pos;
            frontWheelMeshes[i].transform.rotation = rot;

            rearWheels[i].motorTorque = thrustTorque;
            rearWheels[i].steerAngle = turn*rearTurnAngle;

            rearWheels[i].GetWorldPose(out pos, out rot);

            rearWheelMeshes[i].transform.position = pos;
            rearWheelMeshes[i].transform.rotation = rot;
        }
    }
}
