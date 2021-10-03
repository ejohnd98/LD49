using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum PathChoice{
    LEFT,
    CENTER,
    RIGHT,

    NONE
}

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
        WIN_STATE,
        LOSE_STATE,

        NULL_STATE
    }

    public RoadHandler roadHandler;

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

    //Winning Number variables
    public Text winningNumberText;
    public List<int> winningNumbers;
    public int requiredNumbers = 4;

    //Health variables
    public Text healthText;
    public float health = 100.0f;
    public float lossPerFail = 10.0f;

    //Misc flags
    bool enteredFirstRadio = false;
    public PathChoice correctPath = PathChoice.NONE;
    public PathChoice chosenPath = PathChoice.NONE;
    bool pathChosen = false;
    bool phoneOpen = false;
    bool snakePlayed = false;

    int fillersPlayed = 0;
    int leftsPlayed = 0;
    int rightsPlayed = 0;
    int numbersPlayed = 0;

    int successes = 0;

    public void AdvanceState(){
        if(!advancing){
            advanceState = true;
        }
    }

    // Start is called before the first frame update
    void Start() {
        winningNumbers = new List<int>();
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

        if(Input.GetKeyDown(KeyCode.P)){
            radioPlayer.Stop();
            AdvanceState();
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
                    roadHandler.CreateSign(currentWorkingFreq.ToString());
                }
                if(currentSetFreq == currentWorkingFreq){ //if radio frequency entered
                    enteredFirstRadio = true;
                    AdvanceState();
                }
                if(stateTime > 10.0f){
                    if(!enteredFirstRadio){
                        overrideState = GameState.SHOW_SIGN;
                    }
                    AdvanceState();
                }
                if(ExitState()){
                }
                break;
            case GameState.RADIO_FILLER:
                if(EnterState()){
                    //Play filler radio clip
                    if(currentSetFreq == currentWorkingFreq){
                        EnqueueSound("f"+(fillersPlayed+1).ToString());
                        fillersPlayed = (fillersPlayed + 1) % 6;
                    }
                    
                }
                AdvanceAfterSound();
                if(ExitState()){
                }
                break;
            case GameState.SHOW_SECOND_SIGN:
                if(EnterState()){
                    //Spawn in second upcoming sign
                    roadHandler.CreateSign(currentWorkingFreq.ToString() + " Fork up ahead");
                }
                AdvanceState();
                if(ExitState(10.0f)){
                }
                break;
            case GameState.RADIO_DIRECTIONS:
                if(EnterState()){
                    GenerateNextTurn();
                    if(currentSetFreq == currentWorkingFreq){
                        //Play radio clip telling correct path
                        if(correctPath == PathChoice.LEFT){
                            EnqueueSound("left"+(leftsPlayed+1).ToString());
                            leftsPlayed = (leftsPlayed + 1) % 6;
                        }
                        if(correctPath == PathChoice.RIGHT){
                            EnqueueSound("right"+(rightsPlayed+1).ToString());
                            rightsPlayed = (rightsPlayed + 1) % 6;
                        }
                    }
                }
                AdvanceAfterSound();
                if(ExitState()){
                    GenerateNextRadioFreq();
                }
                break;
            case GameState.ROAD_FORK:
                if(EnterState()){
                    //Create fork in road
                    roadHandler.CreateFork();
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
                    if(numbersPlayed < 5){
                        GetNextWinningNumber(); //will want to delay to match audio
                        EnqueueSound("number"+(numbersPlayed+1).ToString());
                        //queue up other sound to play david lynch
                        numbersPlayed++;
                    }
                    successes++;
                }
                
                AdvanceAfterSound();

                if(ExitState()){
                    overrideState = GameState.SHOW_SIGN;
                }
                break;
            case GameState.PATH_FAIL:
                if(EnterState()){
                    //reduce gas
                    DealDamage(lossPerFail);
                    //increase radio static
                    //flash warning indicator?
                }
                
                AdvanceState();

                if(ExitState()){
                    overrideState = GameState.SHOW_SIGN;
                }
                break;
            case GameState.LOSE_STATE:
                if(EnterState()){
                    Debug.Log("GAME OVER");  
                }
                overrideState = GameState.GAME_END;
                AdvanceState();
                if(ExitState(5.0f)){

                }
                break;
            case GameState.WIN_STATE:
                if(EnterState()){
                    Debug.Log("GAME WON");  
                }
                overrideState = GameState.GAME_END;
                AdvanceState();
                if(ExitState(5.0f)){

                }
                break;
            case GameState.GAME_END:
                if(EnterState()){
                    Debug.Log("GAME END");  
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
        if(!snakePlayed && freq == 14015){
            snakePlayed = true;
            EnqueueSound("snake");
        }
    }

    public void OpenPhone(bool open){
        phoneOpen = open;
    }

    public void GetNextWinningNumber(){
        int number = (int)Random.Range(0.0f, 9.99f);
        winningNumbers.Add(number);
        string winningString = "Winning Numbers: ";
        foreach(int i in winningNumbers){
            winningString += (i.ToString() + " ");
        }
        winningNumberText.text = winningString;

        if(winningNumbers.Count >= requiredNumbers){
            overrideState = GameState.WIN_STATE;
            AdvanceState();
            ExitState(0.0f);
        }
    }

    public void DealDamage(float damage){
        health = Mathf.Max(0.0f, health - damage);
        healthText.text = health.ToString();
        //degrade quality of things here depending on damage
        
        if(health <= 0.0f){
            overrideState = GameState.LOSE_STATE;
            AdvanceState();
            ExitState(0.0f);
        }
    }

    void GenerateNextTurn(){
        if(Random.Range(0.0f, 1.0f) >= 0.5f){
            correctPath = PathChoice.RIGHT;
        }else{
            correctPath = PathChoice.LEFT;
        }
    }

    public void ChooseTurn(PathChoice choice){
        chosenPath = choice;
        pathChosen = true;
        roadHandler.ChoosePath(choice);
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
