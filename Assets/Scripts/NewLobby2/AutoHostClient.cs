using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AutoHostClient : MonoBehaviour {
    [SerializeField] NetworkManager networkManager;

    [SerializeField] bool forceServer = true;

    private void Start () {
        if (!Application.isBatchMode && !forceServer) //headless build
        {
            Debug.Log ($"==== Client Build ===");
            networkManager.StartClient ();
        } else {
            Debug.Log ($"=== Server Build ===");
#if UNITY_EDITOR
            networkManager.StartServer ();
#endif
        }
    }

    public void JoinLocal () {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient ();
    }
}