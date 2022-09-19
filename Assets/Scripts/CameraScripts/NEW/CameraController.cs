using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    //[SyncVar]
    GameObject camObject;
    public GameObject marioCamera;
    public GameObject pikachuCamera;
    public GameObject lobbyCam;
    public FreeCameraLook cameraLook;
    public bool bMario;
    //[SyncVar]
    public bool bHideCamera;
    StateManager states;
    void Start()
    {
        states = GetComponent<StateManager>();
        marioCamera = GameObject.Find("Camera Holder (Mario)");
        pikachuCamera = GameObject.Find("Pikachu Camera");
        lobbyCam = GameObject.Find("LobbyCam");
        cameraLook = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponent<FreeCameraLook>();

        StartCoroutine(DelaySpawnEnableCamera());
    }

    IEnumerator DelaySpawnEnableCamera()
    {
        yield return new WaitForSeconds(0.032f);
        bHideCamera = true;
        yield return new WaitForSeconds(0.032f);
        bHideCamera = false;
        yield return new WaitForSeconds(0.032f);
        
        bHideCamera = false;

        lobbyCam.SetActive(true);
        marioCamera.SetActive(false);
        pikachuCamera.SetActive(false);
    }

    void Update()
    {
        //if (marioCamera == null)
        //{
        //    marioCamera = GameObject.Find("Camera Holder (Mario)");
        //}
        //if (pikachuCamera == null)
        //{
        //    pikachuCamera = GameObject.Find("Pikachu Camera");
        //}
        //if (lobbyCam == null)
        //{
        //    lobbyCam = GameObject.Find("LobbyCam");
        //}
        states = GetComponent<StateManager>();


        if (bHideCamera) //Lobby (hide Mario/Pikachu Cameras)
        {
            marioCamera.SetActive(false);
            pikachuCamera.SetActive(false);
        }
        else //Go into Game! (show Mario/Pikachu Cameras)
        {
            marioCamera.gameObject.SetActive(true);
            pikachuCamera.SetActive(true);
        }

        if (!hasAuthority) { return; }

        if (marioCamera == null)
            marioCamera = GameObject.Find("Camera Holder (Mario)");

        if (cameraLook == null)
            cameraLook = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponent<FreeCameraLook>();

        // Adjust camera speed when aiming
        if (cameraLook != null) {
            if (states.aiming) {
                cameraLook.turnSpeed = 6f;
            }
            else if (states.aiming == false) {
                cameraLook.turnSpeed = 14.5f;
            }
        }
    }
}
