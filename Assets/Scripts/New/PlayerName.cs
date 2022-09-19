using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : NetworkBehaviour {
    public TMP_Text displayNameText = null;
    public TMP_Text displayNameTextPika = null;

    [SyncVar] //KEEP ON
    public string playerNameTag;

    public TMP_InputField nameInput;
    public Canvas canvasDisplay = null;
    public Canvas canvasDisplayPika = null;

    private Transform mainCameraTransform;

    public override void OnStartAuthority () 
    {
         //nameInput = GameObject.Find("Name_InputField").GetComponent<TMP_InputField>();
         playerNameTag = "Test32";

    }


    void Start () 
    {
        mainCameraTransform = Camera.main.transform;
    }

    //IEnumerator UpdateClient () 
    void Update()
    {
        //Debug.Log($"Starting UpdateClient on PlayerName");
        //while (true) 
        {
           // if (GetComponent<PlayerSwitch>().isMario == true)
            {
                if (canvasDisplay != null && TurnManager.instance && !TurnManager.instance.bShowScreenGet) //Mario and GAME IS NOT LOADING
                {
                    //GameObject camera1 = GameObject.FindGameObjectWithTag("PikachuCamera");

                    canvasDisplay.transform.forward = Camera.main.transform.forward;
                    //canvasDisplay.transform.forward = camera1.transform.forward;
                    //canvasDisplay.transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                    //    mainCameraTransform.rotation * Vector3.up);
                }
            }
            //if (GetComponent<PlayerSwitch>().isMario == false)
            {
                if (canvasDisplayPika != null) //Mario
                {
                   // GameObject camera2 = GameObject.FindGameObjectWithTag("PikachuCamera");

                    //canvasDisplayPika.transform.forward = Camera.main.transform.forward;
                    //canvasDisplayPika.transform.forward = camera2.transform.forward;
                }
            }


            if (displayNameText != null) 
            {
                displayNameText.text = playerNameTag;
            }
            if (displayNameTextPika != null)
            {
                displayNameTextPika.text = playerNameTag;
            }

            if (!hasAuthority) { return; }



            playerNameTag = GetComponent<Player>().displayName;

            if (displayNameText != null)
            {
                displayNameText.text = "";
            }
            if (displayNameTextPika != null)
            {
                displayNameTextPika.text = "";
            }
            // yield return null;
        }

    }

    private void LateUpdate () //TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! YOU ATE WIT DAD
    {
        if (!hasAuthority) { return; }


        if (displayNameText)
        {
            displayNameText.text = "";
        }
        if (displayNameTextPika)
        {
            displayNameTextPika.text = "";
        }
    }


    //?????????
    [Server]
    void ServerCheck () {
        {
            //displayNameText.text = playerNameTag;
        }
    }

    [Command]
    void CmdNameChange () {

        //if (displayNameText)
        {
            //displayNameText.text = playerNameTag;
        }

        RpcNameChange ();

    }

    [ClientRpc]
    void RpcNameChange () {
        if (canvasDisplay) {
            //canvasDisplay.transform.forward = Camera.main.transform.forward;
        }

        // playerNameTag = nameInput.text;

        // if (displayNameText)
        {
            //displayNameText.text = playerNameTag;
        }
    }
}