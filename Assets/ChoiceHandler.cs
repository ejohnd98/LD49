using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceHandler : MonoBehaviour
{
    public PathChoice roadChoice;
    public GameController game;

    void Start(){
        game = FindObjectsOfType<GameController>()[0];
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "car"){
            game.ChooseTurn(roadChoice);
        }
    }
}
