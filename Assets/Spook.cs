using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Spook : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioSource staticPlayer;
    public AudioClip[] spooks;
    public bool soundQueued = false;
    public bool isOn = true;
    public float onVol = 1.0f, offVol = 0.1f;
    public float responseTime = 1.0f;

    public GameController game;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        staticPlayer.pitch = 0.5f + 1.0f * Mathf.Clamp((game.currentSetFreq - 8800.0f) / (4000.0f), 0.0f, 1.0f);
        if(!audioPlayer.isPlaying && !soundQueued){
            StartCoroutine(PlayClip());
            soundQueued = true;
        }

        if(game.currentSetFreq == game.currentWorkingFreq || game.currentSetFreq == 14015){
            audioPlayer.volume += (offVol - audioPlayer.volume) * responseTime*  Time.deltaTime;
            staticPlayer.volume += (offVol - staticPlayer.volume)* responseTime * Time.deltaTime;
            isOn = false;
        }else{
            audioPlayer.volume += (onVol- 0.2f - audioPlayer.volume)* responseTime * Time.deltaTime;
            staticPlayer.volume += (onVol- 0.2f - staticPlayer.volume)* responseTime * Time.deltaTime;
            isOn = true;
        }
    }

    IEnumerator PlayClip(){
        float waitTime = Random.Range(3.0f, 10.0f);
        yield return new WaitForSeconds(waitTime);
        if(isOn){
            audioPlayer.clip = spooks[Random.Range(0, spooks.Length)];
            audioPlayer.Play();
            
        }
        soundQueued = false;
    }
}
