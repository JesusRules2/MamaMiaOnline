using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyShort : MonoBehaviour
{
    public float waitTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeleteFunction());
    }


    IEnumerator DeleteFunction()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
