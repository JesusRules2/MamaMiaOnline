using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillCount : NetworkBehaviour
{
    public Text displayKillText = null;

    //public GameObject killBoard = null;
    //[SyncVar]
    //bool bShowupScore = false;

    [SyncVar]
    public int kills;

    //[SyncVar]
    //private bool bOneKillCount = false;
    //[SyncVar]
    //public bool bCountdownKill = false;

    public override void OnStartClient()
    {
        //displayKillText = GameObject.Find("KillText").GetComponent<Text>();
    }

    public override void OnStartLocalPlayer()
    {
        displayKillText = GameObject.Find("KillText").GetComponent<Text>();
        displayKillText.text = "KILLS: 32";
    }

    void Start()
    {
        displayKillText = GameObject.Find("KillText").GetComponent<Text>();
    }

    void Update()
    {
     //   while (true)
        {
            //if (netIdentity.hasAuthority)
            {
                if (!hasAuthority) { return; }
                if (GetComponent<PlayerSwitch>().bGameStarted == false) { return; }


                if (displayKillText == null)
                {
                    displayKillText = GameObject.Find("KillText").GetComponent<Text>();
                }

                if (displayKillText != null)
                {
                    displayKillText.text = "KILLS: " + kills.ToString();
                }

               // int kills2 = GetComponentInParent<PlayerSwitch>().kills2; //for counter
               // GetComponent<KillCount>().kills = kills2; //for scoreboard

                // if (displayKillText)
                {
                }

            }

           // yield return null;

        }
    }

    // [Command]
    // public void CmdKills()
    // {
    //     savedKills = kills;
    //     RpcKills();
    // }
    // [ClientRpc]
    // public void RpcKills()
    // {
    //     savedKills = kills;
    // }
    //IEnumerator KillCountDown()
    //{
    //    bOneKillCount = true;

    //    yield return new WaitForSeconds (3.32f);

    //    bOneKillCount = false;
    //}

    public void KillPlus()
    { //TODO: Call this instead of updating kills directly from another script.
        //CmdKillPlus();

        kills++;
       // GameOverManager.instance.AddKillsForConnection(connectionToClient); //SHOW JARED

       RpcKillPlus();
    }

    [Command]
    public void CmdKillPlus()
    {
        kills++;

        //GameOverManager.instance.AddKillsForConnection(connectionToClient);

        RpcKillPlus();
    }

    [ClientRpc]
    public void RpcKillPlus()
    {
        //kills++;
    }

    void TurnOffOnKillBoard()
    {
        // bShowupScore = !bShowupScore;
    }
}


//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class KillCount : NetworkBehaviour
//{
//    public Text displayKillText = null;

//    //public GameObject killBoard = null;
//    //[SyncVar]
//    //bool bShowupScore = false;

//    [SyncVar] 
//    public int kills;
//    [SyncVar]
//    public int savedKills; //my attempt at fixing the killcount problem

//    //[SyncVar]
//    //private bool bOneKillCount = false;
//    //[SyncVar]
//    //public bool bCountdownKill = false;

//    void Start()
//    {
//        StartCoroutine(UpdateClient());
//    }

//    //public override void OnStartClient()
//    public override void OnStartAuthority()
//    {
//        //displayKillText = GameObject.Find("KillText").GetComponent<Text>();
//        //StartCoroutine(UpdateClient());
//    }

//    //[Client]
//    IEnumerator UpdateClient() //DUNNO
//    {
//        while (true)
//        {
//            //if (netIdentity.hasAuthority)
//            {


//                if (displayKillText == null)
//                {
//                    displayKillText = GameObject.Find("KillText").GetComponent<Text>();
//                }

//                savedKills = kills;


//                //if (!hasAuthority) { return; }
//                if (!hasAuthority)
//                {
//                    yield return null;
//                    continue;
//                }

//                savedKills = kills;

//                if (displayKillText == null)
//                {
//                    displayKillText = GameObject.Find("KillText").GetComponent<Text>();
//                }

//                if (displayKillText)
//                {
//                    displayKillText.text = "KILLS: " + kills.ToString();
//                }


//            }

//            yield return null;

//        }
//    }

//    [Command]
//    public void CmdKills()
//    {
//        savedKills = kills;
//        RpcKills();
//    }
//    [ClientRpc]
//    public void RpcKills()
//    {
//        savedKills = kills;
//    }
//    //IEnumerator KillCountDown()
//    //{
//    //    bOneKillCount = true;

//    //    yield return new WaitForSeconds (3.32f);

//    //    bOneKillCount = false;
//    //}

//    //[Command]
//    //public void CmdKillPlus()
//    //{
//    //    kills++;
//    //    RpcKillPlus();
//    //}
//    //[ClientRpc]
//    //public void RpcKillPlus()
//    //{
//    //    kills++;
//    //}

//    void TurnOffOnKillBoard()
//    {
//       // bShowupScore = !bShowupScore;
//    }
//}
