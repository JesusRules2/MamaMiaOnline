using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSwitcher : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;

    private void Awake()
    {
        object1.SetActive(false);
        object2.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        object1.SetActive(true);
    }


    public void SwitchActive()
    {
        if (object1.activeSelf == true)
        {
            object1.SetActive(false);
            object2.SetActive(true);
        }
        else if (object1.activeSelf == false)
        {
            object1.SetActive(true);
            object2.SetActive(false);
        }
    }
}
