using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNetworkManager : MonoBehaviour
{
    //public static CreateNetworkManager instance = null;
    //public GameObject networkManagerPrefab = null;
    //private GameObject manager;

    private void Awake()
    {
        NetworkManagerMario marioNetworkManager = GameObject.FindObjectOfType<NetworkManagerMario>();
        //if (marioNetworkManager)
        {
            Destroy(marioNetworkManager.gameObject);
        }
    }

    private void Start()
    {
        NetworkManagerMario marioNetworkManager = GameObject.FindObjectOfType<NetworkManagerMario>();
       // if (marioNetworkManager)
        {
            Destroy(marioNetworkManager.gameObject);
        }
    }
}

