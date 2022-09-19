using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.height / 2, Screen.width / 2));
        //RaycastHit hitPoint;

        //if (Physics.Raycast(ray, out hitPoint, 32000))
        //{
        //    transform.LookAt(hitPoint.point);
        //}

        transform.LookAt(Camera.main.transform.forward);
    }
}
