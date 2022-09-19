using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerSwitch : NetworkBehaviour
{
    public GameObject mario = null;
    public GameObject pikachu = null;

    public int matchPlayers = 0;

    bool bMarioToPikachu;


    public bool isMario;

    //[SyncVar]
    bool bPikachuMove;

    NetworkSpawnPositions[] spawnPositions;

    public string playerNameTag2 = "Jesus";

    public Component[] marioComponents;
    public Component[] pikachuComponets;

    public GameObject marioModel;
    public GameObject pikachuModel;

    public Text healthText;
    public Text ammoText;

    public bool bGameStarted = false;

    void Start()
    {
        bGameStarted = false;
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
    }

    public override void OnStartClient()
    {
        spawnPositions = FindObjectsOfType<NetworkSpawnPositions>();
        bPikachuMove = true;

        //marioSpawn = Instantiate(MarioPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        //pikachuSpawn = Instantiate(PikachuPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

        //NetworkServer.Spawn(mario, connectionToClient);
        // NetworkServer.Spawn(pikachu, connectionToClient);

        //marioSpawn.transform.parent = gameObject.transform;
        //pikachuSpawn.transform.parent = gameObject.transform;


        pikachu.SetActive(false);
        mario.SetActive(true);

        //HERE
        // SwitchToPikachu();
        SwitchToMario();
        bMarioToPikachu = true; //Mario = true
    }

    public void SwitchToMario()
    {
        
        if (!hasAuthority) { return; }

        GetComponent<HealthScriptPikachu>().enabled = false;
        isMario = true;

        mario.SetActive(true);
        pikachu.SetActive(false);

        marioModel.SetActive(true);
        pikachuModel.SetActive(false);

        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = false;
        }

         GetComponent<CharacterController>().enabled = false;

        TurnManager.instance.CursorPrefab.GetComponent<Image>().enabled = true;

        CmdSwitchToMario();


        Transform StartPosition = NetworkManager.singleton.GetStartPosition();
        transform.position = StartPosition.position;
        transform.rotation = StartPosition.rotation;

        GetComponent<HealthScript>().Health = 100; //may need command/clientrpc
        GetComponent<HealthScript>().IsDead = false;
        GetComponent<HandleShooting>().curBullets =
            GetComponent<HandleShooting>().BulletsMax;
        GetComponent<HealthScript>().anim.SetBool("Revive", true);


        return;

        //COMPONENTS


        GetComponent<KillCount>().kills = pikachu.GetComponent<KillCount>().kills;
        GetComponent<PlayerName>().playerNameTag = playerNameTag2;

        //Respawn fixes
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<HandleAnimations>().enabled = true;
        GetComponent<HandleShooting>().enabled = true;

    }

    public void SwitchToPikachu()
    {
        
        if (!hasAuthority) { return; }

        //GameOverManager.instance.PikachusLeft2++;

        GetComponent<HealthScriptPikachu>().enabled = true;
        isMario = false; //need enum for more character

        pikachu.SetActive(true);
        mario.SetActive(false);

        pikachuModel.SetActive(true);
        marioModel.SetActive(false);

        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = false;
        }
        GetComponent<CharacterController>().enabled = true;
        
        TurnManager.instance.CursorPrefab.GetComponent<Image>().enabled = false;

        CmdSwitchToPikachu();


        StartCoroutine(PikachuMoveRespawn()); //Moves when switched

        GetComponent<HealthScriptPikachu>().Health = 30;
        GetComponent<HealthScriptPikachu>().anim.SetTrigger("Revive");
        //GetComponent<HealthScriptPikachu>().networkAnimator.SetTrigger("Revive");
        GetComponent<HealthScriptPikachu>().IsDead = false;
        GetComponent<HealthScriptPikachu>().IsDeadAnimate = false;
        GetComponent<HealthScriptPikachu>().bKillCountDisable = false;

        return;

        GetComponent<KillCount>().kills = mario.GetComponent<KillCount>().kills;
        GetComponent<PlayerName>().playerNameTag = playerNameTag2;

    }

    public void MovePikachuConnector() {
        StartCoroutine(PikachuMoveRespawn()); //Moves when switched

    }

    IEnumerator PikachuMoveRespawn() //respawn PIKACHU whenever
    {
        Transform StartPosition = NetworkManager.singleton.GetStartPosition();
        //Transform StartPosition = GetRandomStartPosition();

        GetComponent<PlayerController_Platform>().bPikachuMove = false;
        yield return new WaitForSeconds(0.01f);

       // pikachu.transform.position = StartPosition.position;
        //pikachu.transform.rotation = StartPosition.rotation;
        transform.position = StartPosition.position;
        transform.rotation = StartPosition.rotation;

        yield return new WaitForSeconds(0.01f);
        GetComponent<PlayerController_Platform>().bPikachuMove = true;

    }


    [Command]
    void CmdSwitchToMario()
    {
        mario.SetActive(true);
        pikachu.SetActive(false);

        marioModel.SetActive(true);
        pikachuModel.SetActive(false);

        GetComponent<HealthScript>().Health = 100; //may need command/clientrpc

        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = false;
        }
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;

        RpcSwitchToMario();


    }
    [ClientRpc]
    void RpcSwitchToMario()
    {
        mario.SetActive(true);
        pikachu.SetActive(false);

        marioModel.SetActive(true);
        pikachuModel.SetActive(false);

        GetComponent<HealthScript>().Health = 100; //may need command/clientrpc

        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = false;
        }
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
    }

    [Command]
    void CmdSwitchToPikachu()
    {
        pikachu.SetActive(true);
        mario.SetActive(false);

        pikachuModel.SetActive(true);
        marioModel.SetActive(false);

        GetComponent<HealthScriptPikachu>().Health = 30;

        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = false;
        }
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = true;

        RpcSwitchToPikachu();

    }
    [ClientRpc]
    void RpcSwitchToPikachu()
    {
        pikachu.SetActive(true);
        mario.SetActive(false);

        pikachuModel.SetActive(true);
        marioModel.SetActive(false);

        GetComponent<HealthScriptPikachu>().Health = 30;

        foreach (MonoBehaviour comp2 in pikachuComponets)
        {
            comp2.enabled = true;
        }
        foreach (MonoBehaviour comp2 in marioComponents)
        {
            comp2.enabled = false;
        }
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = true;
    }

    //IEnumerator Update2()
    void LateUpdate()
    {
        //while (true)
        {
            if (!hasAuthority) { return; }
            //if (!hasAuthority)
            //{
            //    yield return new WaitForFixedUpdate();
            //    continue;
            //}

            if (bGameStarted == false) { return; }

            if (healthText == null)
            {
                healthText = GameObject.Find("HealthText").GetComponent<Text>();
            }
            if (ammoText == null)
            {
                ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
            }


            if (isMario) //Mario
            {
                float HealthMario = GetComponent<HealthScript>().Health;
                healthText.text = "Health: " + HealthMario.ToString();

                int curBullets = GetComponent<HandleShooting>().curBullets;
                ammoText.text = "AMMO: " + curBullets.ToString();

            }
            if (!isMario) //Pikachu
            {
                float HealthPikachu = GetComponent<HealthScriptPikachu>().Health;
                healthText.text = "Health: " + HealthPikachu.ToString();

                ammoText.text = "";
            }




            //             if (Input.GetKeyDown(KeyCode.J))
            //             {
            //                 if (bMarioToPikachu)
            //                 {
            //                     SwitchToPikachu();
            // 
            //                     bMarioToPikachu = false;
            //                 }
            //                 else if (!bMarioToPikachu)
            //                 {
            //                     SwitchToMario();
            // 
            //                     bMarioToPikachu = true;
            // 
            //                 }
            //             }

            // yield return new WaitForFixedUpdate();
        }
    }



    IEnumerator DisplayNameChange(Player player, GameObject playerSpawn)
    {
        yield return new WaitForSeconds(0.32f); //0.32f

        playerSpawn.GetComponent<PlayerName>().playerNameTag = player.displayName;
        //TimerManager.instance.timeValue = 200; //important
 
    }

    Transform GetRandomStartPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Length)].transform;
    }
}






//public void SwitchToMario()
//{
//    // MarioGO.SetActive(true);
//    // PikachuGO.SetActive(false);
//    // NetworkServer.ReplacePlayerForConnection(connectionToClient, MarioGO, true); //works on server, not client


//    Transform StartPosition = NetworkManager.singleton.GetStartPosition();

//    if (pikachuSpawn != null)
//    {
//        NetworkServer.Destroy(pikachuSpawn.gameObject);
//        Destroy(pikachuSpawn.gameObject);
//    }

//    if (marioSpawn == null)
//    {
//        marioSpawn = Instantiate(MarioPrefab, //on server, put over network
//               StartPosition.position, StartPosition.rotation);

//        NetworkServer.Spawn(marioSpawn, connectionToClient);
//        NetworkServer.ReplacePlayerForConnection(connectionToClient, marioSpawn, true); //works on server, not client
//    }

//    //StartCoroutine(DisplayNameChange(player, marioSpawn));
//}
//public void SwitchToPikachu()
//{
//    Transform StartPosition = NetworkManager.singleton.GetStartPosition();


//    //if (pikachuSpawn == null)
//    {
//        pikachuSpawn = Instantiate(PikachuPrefab, //on server, put over network
//               StartPosition.position, StartPosition.rotation);

//        NetworkServer.Spawn(pikachuSpawn, connectionToClient);
//        NetworkServer.ReplacePlayerForConnection(connectionToClient, pikachuSpawn, true); //works on server, not client
//    }

//    //if (marioSpawn != null)
//    {
//        NetworkServer.Destroy(marioSpawn.gameObject);
//        Destroy(marioSpawn.gameObject);
//    }
//}








//Transform StartPosition = NetworkManager.singleton.GetStartPosition();

//if (pikachuSpawn != null)
//{
//    NetworkServer.Destroy(pikachuSpawn.gameObject);
//    Destroy(pikachuSpawn.gameObject);
//}

//if (marioSpawn == null)
//{
//    marioSpawn = Instantiate(MarioPrefab, //on server, put over network
//           StartPosition.position, StartPosition.rotation); 

//    NetworkServer.Spawn(marioSpawn, connectionToClient);
//    NetworkServer.ReplacePlayerForConnection(connectionToClient, marioSpawn, true); //works on server, not client
//}

//StartCoroutine(DisplayNameChange(player, marioSpawn));