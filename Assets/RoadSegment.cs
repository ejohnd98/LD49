using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadSegment : MonoBehaviour
{
    public GameObject[] exitPoints;
    public Text sign;

    public Transform GetExitTransform(int exit){
        return exitPoints[exit % exitPoints.Length].transform;
    }

    public int NumberOfExits(){
        return exitPoints.Length;
    }

    public void SetSignText(string txt){
        if(sign != null){
            sign.text = txt;
        }
    }
}
