using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public enum GameState{
        PRE_START,
        INSTRUCTIONS,
        INTRO_RADIO,
        PHONE_CALL,
        PHONE_SCREEN,
        DARK_SWITCH,
        INTRO_RADIO_2,
        SHOW_SIGN,
        RADIO_FILLER,
        SHOW_SECOND_SIGN,
        RADIO_DIRECTIONS,
        ROAD_FORK,
        PATH_SUCCESS,
        PATH_FAIL,
        GAME_END,

        NULL_STATE
    }

    public enum PathChoice{
        LEFT,
        CENTER,
        RIGHT,

        NONE
    }

    //State variables
    public GameState state = GameState.PRE_START;
    public GameState overrideState = GameState.NULL_STATE;
    public bool advanceState = false;
    bool advancing = false;
    bool enterFlag = true;
    public float stateTime = 0.0f;

    //Radio system variables
    public AudioSource radioPlayer;
    public AudioClip[] audioFiles;
    public AudioClip defaultSound;
    Dictionary<string, AudioClip> audioLibrary;
    Queue<AudioClip> radioQueue = new Queue<AudioClip>();

    //Radio game variables
    public int currentWorkingFreq = 10000;
    public int currentSetFreq = 10000;

    //Misc flags
    bool enteredFirstRadio = false;
    public PathChoice correctPath = PathChoice.NONE;
    public PathChoice chosenPath = PathChoice.NONE;
    bool pathChosen = false;
    bool phoneOpen = false;

    int successes = 0;

    public void AdvanceState(){
        if(!advancing){
            advanceState = true;
        }
    }

    // Start is called before the first frame update
    void Start() {
        audioLibrary = new Dictionary<string, AudioClip>();
        for(int i = 0; i < audioFiles.Length; i++){
            audioLibrary.Add(audioFiles[i].name, audioFiles[i]);
        }
    }

    // Update is called once per frame
    void Update() {
        //Handle audio queue
        if(radioQueue.Count != 0 && !radioPlayer.isPlaying){
            radioPlayer.clip = radioQueue.Dequeue();
            radioPlayer.Play();
        }

        stateTime += Time.deltaTime;

        //Main game loop
        switch (state){

            case GameState.PRE_START:
                if(EnterState()){

                }
                AdvanceState(); //temp

                if(ExitState()){
                }
                break;
            case GameState.INSTRUCTIONS:
                if(EnterState()){
                    //Display instruction overlay
                }
                AdvanceState(); //temp

                if(ExitState()){
                    //Disable instruction overlay
                }
                break;
            case GameState.INTRO_RADIO:
                if(EnterState()){
                    //Play intro radio
                    EnqueueSound("radio_intro");
                }
                AdvanceAfterSound();
                if(ExitState()){
                }
                break;
            case GameState.PHONE_CALL:
                if(EnterState()){
                    //Play ringtone
                    radioPlayer.loop = true;
                    EnqueueSound("phone_ring");
                }
                
                if(phoneOpen){
                    AdvanceState();
                }

                if(ExitState(0.0f)){
                    radioPlayer.Stop();
                    radioPlayer.loop = false;

                    //bring phone screen up to face
                }
                break;
            case GameState.PHONE_SCREEN:
                if(EnterState()){
                    //Play phone call
                    EnqueueSound("phone_call"); // car crash sounds
                }
                AdvanceAfterSound();
                if(ExitState()){
                    //Change lighting
                }
                break;
            case GameState.DARK_SWITCH:
                if(EnterState()){
                    //Lower phone
                    phoneOpen = false;
                }
                AdvanceState();
                if(ExitState()){
                }
                break;
            case GameState.INTRO_RADIO_2:
                if(EnterState()){
                    //Play second intro radio
                    EnqueueSound("radio_intro_2");
                }
                AdvanceAfterSound();
                if(ExitState()){
                    GenerateNextRadioFreq(); //generate first new radio freq
                }
                break;

            // --------------
            // MAIN GAME LOOP
            // --------------
            case GameState.SHOW_SIGN:
                if(EnterState()){
                    //Spawn in upcoming sign
                }
                if(currentSetFreq == currentWorkingFreq){ //if radio frequency entered
                    enteredFirstRadio = true;
                    AdvanceState();
                }
                if(!enteredFirstRadio && stateTime > 10.0f){
                    overrideState = GameState.SHOW_SIGN;
                    AdvanceState();
                }
                if(ExitState()){
                }
                break;
            case GameState.RADIO_FILLER:
                if(EnterState()){
                    //Play filler radio clip
                    EnqueueSound("radio_filler");
                }
                AdvanceAfterSound();
                if(ExitState()){
                }
                break;
            case GameState.SHOW_SECOND_SIGN:
                if(EnterState()){
                    //Spawn in second upcoming sign
                }
                AdvanceState();
                if(ExitState()){
                }
                break;
            case GameState.RADIO_DIRECTIONS:
                if(EnterState()){
                    GenerateNextTurn();
                    //Play radio clip telling correct path
                    EnqueueSound("radio_path_directions");
                }
                AdvanceAfterSound();
                if(ExitState()){
                    GenerateNextRadioFreq();
                }
                break;
            case GameState.ROAD_FORK:
                if(EnterState()){
                    //Create fork in road
                }

                if(pathChosen && chosenPath != PathChoice.NONE){ //once path has been chosen
                    if(chosenPath == correctPath){
                        overrideState = GameState.PATH_SUCCESS;
                    }else{
                        overrideState = GameState.PATH_FAIL;
                    }
                    chosenPath = PathChoice.NONE;
                    correctPath = PathChoice.NONE;
                    AdvanceState();
                    pathChosen = false;
                }

                if(ExitState()){
                }
                break;
            case GameState.PATH_SUCCESS:
                if(EnterState()){
                    //broadcast next winning number
                    //add number to stickynote
                    EnqueueSound("radio_next_number");
                    successes++;
                }
                overrideState = GameState.SHOW_SIGN;
                AdvanceAfterSound();

                if(ExitState()){
                }
                break;
            case GameState.PATH_FAIL:
                if(EnterState()){
                    //reduce gas
                    //increase radio static
                    //flash warning indicator?
                }
                overrideState = GameState.SHOW_SIGN;
                AdvanceState();

                if(ExitState()){
                }
                break;
            case GameState.GAME_END:
                if(EnterState()){
                        
                }
                if(ExitState()){

                }
                break;
            default:
                break;
        }
    }

    void GenerateNextRadioFreq(){
        currentWorkingFreq = Random.Range(8800/5, 10800/5) * 5;
    }

    public void SetCurrentRadioFreq(int freq){
        currentSetFreq = freq;
    }

    public void OpenPhone(bool open){
        phoneOpen = open;
    }

    void GenerateNextTurn(){
        if(Random.Range(0.0f, 1.0f) >= 0.5f){
            correctPath = PathChoice.RIGHT;
        }else{
            correctPath = PathChoice.LEFT;
        }
    }

    public void ChooseTurn(int choice){
        chosenPath = (PathChoice) choice;
        pathChosen = true;
    }

    void EnqueueSound(string sndName){
        if(audioLibrary.ContainsKey(sndName)){
            radioQueue.Enqueue(audioLibrary[sndName]);
        }else{
            radioQueue.Enqueue(defaultSound);
            Debug.Log("Sound does not exist: " + sndName);
        }

        if(radioQueue.Count != 0 && !radioPlayer.isPlaying){
            radioPlayer.clip = radioQueue.Dequeue();
            radioPlayer.Play();
        }
    }

    void AdvanceAfterSound(){
        if(!radioPlayer.isPlaying && !advancing){
            advanceState = true;
        }
    }

    bool EnterState(){
        if(enterFlag){
            enterFlag = false;
            stateTime = 0.0f;
            return true;
        }
        return false;
    }

    bool ExitState(float delay = 1.0f){
        if(advanceState){
            advanceState = false;
            advancing = true;
            StartCoroutine(AdvanceStateDelay(delay));
            return true;
        }
        return false;
    }

    IEnumerator AdvanceStateDelay(float delay){
        yield return new WaitForSeconds(delay);

        if(overrideState != GameState.NULL_STATE){
            state = overrideState;
            overrideState = GameState.NULL_STATE;
        }else{
            state++;
        }
        
        advancing = false;
        enterFlag = true;
    }
}
