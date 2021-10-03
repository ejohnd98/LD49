using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public GameController gameController;

    int currentFreq = 10000;
    float adjustment = 0.0f;
    public int actualFreq = 10000;
    float holdCounter = 0.0f;
    float holdAmount = 1.0f;
    float sensitivity = 250.0f;

    public bool keypadVisible = false;
    public GameObject keypadObject;

    public int keypadInput = 0;
    public Text radioText;

    // Update is called once per frame
    void Update()
    {
        // HANDLE KEYPAD
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            currentFreq-=5;
            holdCounter = 0.0f;
        }else if(Input.GetKeyDown(KeyCode.RightArrow)){
            currentFreq+=5;
            holdCounter = 0.0f;
        }else if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)){
            currentFreq += (int)adjustment;
            adjustment = 0.0f;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            if(holdCounter <= holdAmount){
                holdCounter += Time.deltaTime;
            }else{
                adjustment -= Time.deltaTime * sensitivity;
            }
        }else if(Input.GetKey(KeyCode.RightArrow)){
            if(holdCounter <= holdAmount){
                holdCounter += Time.deltaTime;
            }else{
                adjustment += Time.deltaTime * sensitivity;
            }
        }

        actualFreq = (currentFreq + (int)adjustment) / 5;
        actualFreq *= 5;

        gameController.SetCurrentRadioFreq(actualFreq);
        if(keypadInput != 0){
            radioText.text = keypadInput.ToString();
        }else{
            radioText.text = actualFreq.ToString();
        }

        // Check for clicks on objects
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)){
                if(hit.transform.gameObject.tag == "radioDial"){
                    keypadVisible = true;
                    keypadObject.SetActive(true);
                }
                if(hit.transform.gameObject.tag == "phone"){
                    gameController.OpenPhone(true);
                }
            }
        }
    }

    public void SendKey(int value){
        if(value == -1){
            keypadInput = 0;
        }else if(value == 999){
            if(keypadInput >= 8000){
                currentFreq = keypadInput;
            }
            keypadInput = 0;
            
        }else if(value == -999){
            keypadInput = 0;
            keypadVisible = false;
            keypadObject.SetActive(false);
        }else{
            if(keypadInput <= 9999){
                keypadInput *= 10;
                keypadInput += value;
            }else{
                keypadInput = value;
            }
            
        }
    }
}
