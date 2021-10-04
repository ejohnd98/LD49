using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeypadGen : MonoBehaviour
{
    public GameObject buttonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        buttonPrefab.GetComponentInChildren<Text>().text = "Back";
        buttonPrefab.GetComponent<KeypadButton>().value = -999;

        GameObject newButton = GameObject.Instantiate(buttonPrefab, transform);
        newButton.GetComponentInChildren<Text>().text = 0.ToString();
        newButton.GetComponent<KeypadButton>().value = 0;
        
        newButton = GameObject.Instantiate(buttonPrefab, transform);
        newButton.GetComponentInChildren<Text>().text = "Enter";
        newButton.GetComponent<KeypadButton>().value = 999;

        for(int i = 1; i <= 9; i++){
            newButton = GameObject.Instantiate(buttonPrefab, transform);
            newButton.GetComponentInChildren<Text>().text = i.ToString();
            newButton.GetComponent<KeypadButton>().value = i;
        }
    }
}
