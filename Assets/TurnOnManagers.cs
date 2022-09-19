using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnManagers : MonoBehaviour
{
    public GameObject[] objectsOn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnManagers2());
    }

    IEnumerator TurnOnManagers2()
    {
        yield return new WaitForSeconds(0.16f);
    
        for (int i = 0; i < objectsOn.Length; i++)
        {
            objectsOn[i].SetActive(true);
        }
    }
}
