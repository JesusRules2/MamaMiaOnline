using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenRotate : MonoBehaviour
{

     Vector3 rotationEuler;

    // Update is called once per frame
    void Update()
    {
         rotationEuler-= Vector3.forward*60*Time.deltaTime; //increment 30 degrees every second
        transform.rotation = Quaternion.Euler(rotationEuler); 
    }
}
