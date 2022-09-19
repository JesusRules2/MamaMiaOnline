//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class TurnOffGame : NetworkBehaviour
//{
//    //[SerializeField] private bool isMario;

//    bool CountDownQuit = false;
//    float TimerQuit = 0f;

//    void Start()
//    {
//        //StartCoroutine(UpdateClient());
//    }


//    IEnumerator UpdateClient()
//    {
//        while (true)
//        {
//            if (netIdentity.hasAuthority)
//            {
//                if (!hasAuthority)
//                {
//                    yield return null;
//                    continue;
//                }

//                gameObject.SetActive(true);
//                // if (!hasAuthority) { return; }
//                if (Input.GetKeyDown(KeyCode.Escape))
//                {
//                   // CountDownQuit = true;

//                    Application.Quit(); //TURN ON?????????????????
//                  //  NetworkManager.singleton.StopHost();
//                   // NetworkManager.singleton.Start();

//                    //SceneManager.LoadScene(0);// DUNNNOOOOO!
//                }

//                if (CountDownQuit)
//                {
//                    TimerQuit += Time.deltaTime;
//                    if (TimerQuit > 0.32)
//                    {
//                        NetworkManager.singleton.StopHost();
//                        NetworkManager.singleton.Start();

//                        GameOverManager.instance.bHidePikachuCount = false;

//                        SceneManager.LoadScene(0);

//                        //GameOverManager.instance.bPikachuWins = false;
//                        //GameOverManager.instance.bMarioWins = false;
//                        //GameOverManager.instance.MarioWinPanel.SetActive(false);
//                        //GameOverManager.instance.PikachuWinPanel.SetActive(false);

//                        GameOverManager.instance.bHidePikachuCount = false;


//                    }
//                }


//                //if (isMario)
//                //{
//                //    if (Input.GetKeyDown(KeyCode.Escape))
//                //    {
//                //        NetworkManager.singleton.StopHost();
//                //        NetworkManager.singleton.Start();
//                //        SceneManager.LoadScene(0);
//                //    }
//                //}

//            }

//            yield return null;
//        }
//    }

//    [Command]
//    void CmdPikachuDie()
//    {
//        //GameOverManager.instance.PikachusDead++;
//        RpcPikachuDie();
//    }

//    [ClientRpc]
//    void RpcPikachuDie()
//    {
//        if (CountDownQuit) { return; }
//        GameOverManager.instance.PikachusDead++;
//    }
//}
