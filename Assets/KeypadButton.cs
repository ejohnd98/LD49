using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public int value;
    public InputHandler inputHandler;
    
    public void PressButton(){
        inputHandler.SendKey(value);
    }
}
