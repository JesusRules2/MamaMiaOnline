//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class KillCount2 : NetworkBehaviour {
//    public Text displayKillText = null;

//    //public GameObject killBoard = null;
//    //[SyncVar]
//    //bool bShowupScore = false;

//    public int kills;

//    //[SyncVar]
//    //private bool bOneKillCount = false;
//    //[SyncVar]
//    //public bool bCountdownKill = false;

//    void Start () {
//        StartCoroutine (UpdateClient ());
//    }

//    //public override void OnStartClient()
//    public override void OnStartAuthority () {
//        //displayKillText = GameObject.Find("KillText").GetComponent<Text>();
//        //StartCoroutine(UpdateClient());
//    }

//    //[Client]
//    IEnumerator UpdateClient () //DUNNO
//    {
//        while (true) {
//            //if (netIdentity.hasAuthority)
//            {

//                if (displayKillText == null) {
//                    displayKillText = GameObject.Find ("KillText").GetComponent<Text> ();
//                }

//                // savedKills = kills;

//                //if (!hasAuthority) { return; }
//                if (!hasAuthority) {
//                    yield return null;
//                    continue;
//                }

//                // savedKills = kills;

//                if (displayKillText == null) {
//                    displayKillText = GameObject.Find ("KillText").GetComponent<Text> ();
//                }

//                if (displayKillText) {
//                    displayKillText.text = "KILLS: " + kills.ToString ();
//                }

//            }

//            yield return null;

//        }
//    }

//    // [Command]
//    // public void CmdKills()
//    // {
//    //     savedKills = kills;
//    //     RpcKills();
//    // }
//    // [ClientRpc]
//    // public void RpcKills()
//    // {
//    //     savedKills = kills;
//    // }
//    //IEnumerator KillCountDown()
//    //{
//    //    bOneKillCount = true;

//    //    yield return new WaitForSeconds (3.32f);

//    //    bOneKillCount = false;
//    //}

//    public void KillPlus () { //TODO: Call this instead of updating kills directly from another script.
//        CmdKillPlus ();
//    }

//    [Command]
//    public void CmdKillPlus () {
//        kills++;

//        GameOverManager.instance.AddKillsForConnection(connectionToClient);

//        RpcKillPlus ();
//    }

//    [ClientRpc]
//    public void RpcKillPlus () {
//        kills++;
//    }

//    void TurnOffOnKillBoard () {
//        // bShowupScore = !bShowupScore;
//    }
//}