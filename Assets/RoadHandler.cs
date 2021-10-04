using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadHandler : MonoBehaviour
{
    public GameObject[] straightPrefabs, forkPrefabs;
    public GameObject signPrefab;
    public Text signText;
    public GameObject forkSign, nonForkSign;
    public Transform player;

    RoadSegment leftRoad, rightRoad;
    PathChoice chosenRoad = PathChoice.LEFT;

    public bool extendRoad = false;
    public bool createFork = false;

    public float genDist = 20.0f;

    // Start is called before the first frame update
    void Start(){

        GameObject newSegment = GameObject.Instantiate(straightPrefabs[0]);
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
        int index = Random.Range(0, straightPrefabs.Length);
        if(chosenRoad == PathChoice.LEFT){
            leftRoad = AddSegmentTo(straightPrefabs[index], leftRoad, 0);
        }else{
            rightRoad = AddSegmentTo(straightPrefabs[index], rightRoad, 0);
        }
        
    }

    public void CreateSign(int freq, bool fork){
        if(chosenRoad == PathChoice.LEFT){
            leftRoad = AddSegmentTo(signPrefab, leftRoad, 0);
        }else{
            rightRoad = AddSegmentTo(signPrefab, rightRoad, 0);
        }
        string radioString = (freq/100).ToString();
        radioString += ".";
        int len = freq.ToString().Length;
        radioString += freq.ToString()[len-2];
        radioString += freq.ToString()[len-1];
        radioString += " FM";

        signText.text = radioString;

        forkSign.SetActive(fork);
        nonForkSign.SetActive(!fork);
    }

    public void CreateFork(){
        RoadSegment forkSegment;
        int index = Random.Range(0, forkPrefabs.Length);
        //create fork
        if(chosenRoad == PathChoice.LEFT){
            forkSegment = AddSegmentTo(forkPrefabs[index], leftRoad, 0);
        }else{
            forkSegment = AddSegmentTo(forkPrefabs[index], rightRoad, 0);
        }
        // Create two new roads
        leftRoad = AddSegmentTo(straightPrefabs[Random.Range(0, straightPrefabs.Length)], forkSegment, 0);
        rightRoad = AddSegmentTo(straightPrefabs[Random.Range(0, straightPrefabs.Length)], forkSegment, 1);

        //extend out roads long enough
        for(int i = 0; i < 2; i++){
            leftRoad = AddSegmentTo(straightPrefabs[Random.Range(0, straightPrefabs.Length)], leftRoad, 0);
            rightRoad = AddSegmentTo(straightPrefabs[Random.Range(0, straightPrefabs.Length)], rightRoad, 0);
        }
    }

    RoadSegment AddSegmentTo(GameObject newSegment, RoadSegment addTo, int exit){
        // create new segment and align to desired exit
        GameObject temp = GameObject.Instantiate(newSegment);
        Transform exitTransform = addTo.GetExitTransform(exit);

        temp.transform.SetParent(transform);
        temp.transform.position = exitTransform.position;
        temp.transform.rotation = exitTransform.rotation;

        return temp.GetComponent<RoadSegment>();
    }
}
