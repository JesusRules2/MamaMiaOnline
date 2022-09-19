using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController_Platform : NetworkBehaviour 
{
   // [SyncVar]
    public Transform target;

    //[SyncVar]
    public Vector3 offset;

  //[SyncVar]
    public bool useOffsetValues;

    //[SyncVar]
    public float rotateSpeed;

   //[SyncVar]
    public Transform pivot;

   // [SyncVar]
    public Transform mainCamera;

   // [SyncVar]
    public float maxViewAngle;
   // [SyncVar]
    public float minViewAngle;

   // [SyncVar]
    public bool invertY;

    //NEW
    //[SyncVar]
    Vector3 pivotPosition;

    //public Transform initialCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        if(!useOffsetValues)
        {
            //offset = target.position - mainCamera.transform.position;
        }

       //transform.SetParent(Player.localPlayer.transform);
        
        if (hasAuthority)
        {

           // mainCamera = GameObject.FindGameObjectWithTag("PikachuCamera").transform;
            mainCamera = GameObject.Find("Pikachu Camera").transform;
            mainCamera.transform.parent = gameObject.transform;
            pivot = GameObject.FindGameObjectWithTag("PivotPikachu").transform;

            pivotPosition = target.transform.position; //new

            pivot = GameObject.FindGameObjectWithTag("PivotPikachu").transform;
            pivot.transform.position = target.transform.position;
            pivot.transform.parent = null;
        }
    }


    // Update is called once per frame
    //IEnumerator UpdateClient()
    void Update()
    {
        if (!hasAuthority) { return; }

        if (hasAuthority)
        {

            // mainCamera = GameObject.FindGameObjectWithTag("PikachuCamera").transform;
            mainCamera = GameObject.Find("Pikachu Camera").transform;
            mainCamera.transform.parent = gameObject.transform;
            pivot = GameObject.FindGameObjectWithTag("PivotPikachu").transform;

            pivotPosition = target.transform.position; //new

            pivot = GameObject.FindGameObjectWithTag("PivotPikachu").transform;
            pivot.transform.position = target.transform.position;
            pivot.transform.parent = null;
        }

       // pivotPosition = target.transform.position; //new

            //Get the X position of the mouse & rotate the target
            float horizontal = Input.GetAxis("Mouse X Pika") * rotateSpeed;
            pivot.Rotate(0.0f, horizontal, 0.0f);

            //Get the Y position of the mouse and rotate the pivot
            float vertical = Input.GetAxis("Mouse Y Pika") * rotateSpeed;
            //pivot.Rotate(-vertical, 0.0f, 0.0f);
            if (invertY)
            {
                pivot.Rotate(vertical, 0.0f, 0.0f);
            }
            else
            {
                pivot.Rotate(-vertical, 0.0f, 0.0f);

            }

            if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180.0f)
            {
                pivot.rotation = Quaternion.Euler(maxViewAngle, pivot.eulerAngles.y, 0.0f);
            }

            if (pivot.rotation.eulerAngles.x > 180.0f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
            {
                pivot.rotation = Quaternion.Euler(360.0f + minViewAngle, pivot.eulerAngles.y, 0.0f);
            }

            //Move the camera based on the current rotation of the target and the original offset
            float desiredYAngle = pivot.eulerAngles.y;
            float desiredXAngle = pivot.eulerAngles.x;

            Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
            mainCamera.transform.position = target.position - (rotation * offset);

            //  transform.position = target.position - offset;

            if (mainCamera.transform.position.y < target.position.y)
            {
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, target.position.y - 0.5f, mainCamera.transform.position.z);
            }

            mainCamera.transform.LookAt(target);

    }



    //[Command]
    //void CmdCameraMove()
    //{
    //    if (GetComponentInParent<HealthScriptPikachu>().IsDead) { return; }

    //    pivot.transform.position = target.transform.position;
    //    //pivot.transform.parent = target.transform;
    //    pivot.transform.parent = null;

    //    //Get the X position of the mouse & rotate the target
    //    float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
    //    pivot.Rotate(0.0f, horizontal, 0.0f);

    //    //Get the Y position of the mouse and rotate the pivot
    //    float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
    //    //pivot.Rotate(-vertical, 0.0f, 0.0f);
    //    if (invertY)
    //    {
    //        pivot.Rotate(vertical, 0.0f, 0.0f);
    //    }
    //    else
    //    {
    //        pivot.Rotate(-vertical, 0.0f, 0.0f);

    //    }

    //    if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180.0f)
    //    {
    //        pivot.rotation = Quaternion.Euler(maxViewAngle, pivot.eulerAngles.y, 0.0f);
    //    }

    //    if (pivot.rotation.eulerAngles.x > 180.0f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
    //    {
    //        pivot.rotation = Quaternion.Euler(360.0f + minViewAngle, pivot.eulerAngles.y, 0.0f);
    //    }

    //    //Move the camera based on the current rotation of the target and the original offset
    //    float desiredYAngle = pivot.eulerAngles.y;
    //    float desiredXAngle = pivot.eulerAngles.x;

    //    Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
    //    transform.position = target.position - (rotation * offset);

    //    //  transform.position = target.position - offset;

    //    if (transform.position.y < target.position.y)
    //    {
    //        transform.position = new Vector3(transform.position.x, target.position.y - 0.5f, transform.position.z);
    //    }

    //    transform.LookAt(target);
    //}
}
