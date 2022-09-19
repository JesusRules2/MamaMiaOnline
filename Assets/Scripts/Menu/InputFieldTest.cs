using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldTest : MonoBehaviour
{
    private InputField inputField;
    private NewControls action;
    private TouchScreenKeyboard keyboard;
    private bool selected;
    private void Awake() {
        action = new NewControls();
    }

    void Start() {
        inputField = GetComponent<InputField>();
    }

    void Update() {
        if (EventSystem.current.currentSelectedGameObject == inputField.gameObject
            && !selected)
            {
                action.Player.DeSelect.Disable();
                action.Player.Select.Enable();
                action.Player.Select.performed += _ => Selected();
                // inputField.text = "Press South Button on gamepad to type 32";
            }
            else {
                action.Player.Select.Disable();
            }

            if (EventSystem.current.currentSelectedGameObject == inputField.gameObject
            && selected)
            {
                action.Player.DeSelect.Enable();
                action.Player.DeSelect.performed += _ => DeSelected();
            }
    }

    private void Selected() {
        selected = true;
        inputField.interactable = true;
        action.Player.Select.performed -= _ => Selected();
        keyboard = TouchScreenKeyboard.Open("",
        TouchScreenKeyboardType.ASCIICapable);
        Debug.Log("South Button Pressed");
    }

    private void DeSelected() {
        keyboard = null;
        selected = false;
        inputField.interactable = false;
        inputField.Select();
        action.Player.DeSelect.performed -= _ => DeSelected();
        Debug.Log("East Button Presse");

    }

    private void OnEnable() {
        action.Enable();
    }

    private void OnDisable() {
        action.Disable();
    }
}
