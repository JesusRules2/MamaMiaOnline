using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class GamepadInputFieldHelper : MonoBehaviour {
 
    InputField _inputField;
 
 
    // Use this for initialization
    void Start () {
        _inputField = GetComponent<InputField>();
    }
    
    // Update is called once per frame
    void Update () {
        if(Input.GetButtonUp("Submit") || Input.GetButtonUp("Cancel"))
        {
            _inputField.DeactivateInputField();
        }
    }
}