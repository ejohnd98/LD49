using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHandler : MonoBehaviour
{
    public GameObject straightPrefab, forkPrefab, signPrefab;
    public Transform player;

    RoadSegment leftRoad, rightRoad;
    PathChoice chosenRoad = PathChoice.LEFT;

    public bool extendRoad = false;
    public bool createFork = false;

    public float genDist = 20.0f;

    // Start is called before the first frame update
    void Start(){

        GameObject newSegment = GameObject.Instantiate(straightPrefab);
        leftRoad = newSegment.GetComponent<RoadSegment>();
    }

    // Update is called once per frame
    void Update(){
        if(extendRoad){
            ExtendRoad();
            extendRoad = false;
        }
        if(createFork){
            CreateFork();
            createFork = false;
        }

        if(Vector3.Distance(player.position, GetCurrentRoad().transform.position) < genDist){
            ExtendRoad();
        }
    }

    RoadSegment GetCurrentRoad(){
        if(chosenRoad == PathChoice.LEFT){
            return leftRoad;
        }else{
            return rightRoad;
        }
    }

    public void ChoosePath(PathChoice choice){
        chosenRoad = choice;
    }

    public void ExtendRoad(){
        if(chosenRoad == PathChoice.LEFT){
            leftRoad = AddSegmentTo(straightPrefab, leftRoad, 0);
        }else{
            rightRoad = AddSegmentTo(straightPrefab, rightRoad, 0);
        }
        
    }

    public void CreateSign(string txt){
        if(chosenRoad == PathChoice.LEFT){
            leftRoad = AddSegmentTo(signPrefab, leftRoad, 0);
            leftRoad.SetSignText(txt);
        }else{
            rightRoad = AddSegmentTo(signPrefab, rightRoad, 0);
            rightRoad.SetSignText(txt);
        }
        
    }

    public void CreateFork(){
        RoadSegment forkSegment;
        //create fork
        if(chosenRoad == PathChoice.LEFT){
            forkSegment = AddSegmentTo(forkPrefab, leftRoad, 0);
        }else{
            forkSegment = AddSegmentTo(forkPrefab, rightRoad, 0);
        }

        // Create two new roads
        leftRoad = AddSegmentTo(straightPrefab, forkSegment, 0);
        rightRoad = AddSegmentTo(straightPrefab, forkSegment, 1);

        //extend out roads long enough
        for(int i = 0; i < 2; i++){
            leftRoad = AddSegmentTo(straightPrefab, leftRoad, 0);
            rightRoad = AddSegmentTo(straightPrefab, rightRoad, 0);
        }
    }

    RoadSegment AddSegmentTo(GameObject newSegment, RoadSegment addTo, int exit){
        // create new segment and align to desired exit
        GameObject temp = GameObject.Instantiate(newSegment);
        Transform exitTransform = addTo.GetExitTransform(exit);

        temp.transform.position = exitTransform.position;
        temp.transform.rotation = exitTransform.rotation;

        return temp.GetComponent<RoadSegment>();
    }
}
