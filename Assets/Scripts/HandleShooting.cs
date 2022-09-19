//NEW
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HandleShooting : NetworkBehaviour
{
    StateManager states;
    HealthScript marioHealth;
    public Animator weaponAnim;
    public float fireRate;
    float timer;
    public Transform bulletSpawnPoint;
    public GameObject smokeParticle;
    public ParticleSystem[] muzzle;
    public GameObject casingPrefab;
    public Transform caseSpawn;

    //NEW CODE
    public Camera thirdPersonCam;
    //NEW CODE (PUT IN HUD SCRIPT EVENTUALLY!)
    public Text ammoText;
    //NEW CODE
    public LayerMask layerMask = new LayerMask();

    public int BulletsMax = 30;
    [SyncVar]
    public int curBullets;

    bool bKill = false;

    KillCount killCount = null;

    public bool bShowAmmoText = true; //CHEK HERE

    // [SyncVar]
    public Transform aimHelper;
    public Collider ownCollider;

    public bool bGameOver = false;


    //public override void OnStartLocalPlayer () {
    void Start()
    {
        StartCoroutine(BulletDelaySpawn());

        curBullets = BulletsMax;
        //ownCollider = GetComponent<Collider>();
        states = GetComponent<StateManager>();
        killCount = GetComponent<KillCount>();
        marioHealth = GetComponent<HealthScript>();

        // ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
        // ammoText.text = "AMMO: " + curBullets.ToString();
        ammoText.text = "AMMO: " + curBullets.ToString();

        thirdPersonCam = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponentInChildren<Camera>();

        bKill = false;

        //GameOverManager.instance.CursorPrefab.SetActive(true);

        //StartCoroutine(UpdateClient()); //FixedUpdate


        //thirdPersonCam = GameObject.Find("Camera Holder (Mario)/Pivot/Holder 2/Main Camera 2").GetComponent<Camera>();

    }

    bool shoot;
    bool dontShoot;

    bool emptyGun;


    public override void OnStartAuthority() //Doesn spawn at lobby, in scene
    {
        thirdPersonCam = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponentInChildren<Camera>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();

    }

    IEnumerator BulletDelaySpawn()
    {
        yield return new WaitForSeconds(0.32f);
        curBullets = 0;
        yield return new WaitForSeconds(0.32f);
        curBullets = BulletsMax;
    }

    //IEnumerator UpdateClientFixed() //DUNNO
    void FixedUpdate()
    {
        //while (true)
        {

            //if (netIdentity.hasAuthority)
            {

                if (!hasAuthority || bGameOver == true) { return; }


                //ISSUES
                if (ammoText == null)
                {
                    //ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();

                }
                if (ammoText)
                {
                   // ammoText.text = "AMMO: " + curBullets.ToString();
                }

                shoot = states.shoot;

                if (shoot)
                {
                    if (timer <= 0)
                    {
                        weaponAnim.SetBool("Shoot", false);

                        if (curBullets > 0)
                        {
                            emptyGun = false;
                            // states.audioManager.PlayGunSound();

                            GameObject go = Instantiate(casingPrefab, caseSpawn.position, caseSpawn.rotation) as GameObject;
                            Rigidbody rig = go.GetComponent<Rigidbody>();
                            rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
                            rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);

                            for (int i = 0; i < muzzle.Length; i++)
                            {
                                muzzle[i].Emit(1);
                            }

                            RaycastShoot();
                            states.audioManager.CmdPlayGunSound();

                            curBullets = curBullets - 1;

                        }
                        else
                        {
                            if (emptyGun)
                            {
                                states.handleAnim.StartReload();
                                //states.audioManager.PlayGunReload();
                                states.audioManager.CmdPlayGunReload();
                                curBullets = BulletsMax;
                            }
                            else
                            {
                                states.audioManager.PlayEffect("empty_gun");
                                emptyGun = true;
                            }
                        }

                        timer = fireRate;
                    }
                    else
                    {
                        weaponAnim.SetBool("Shoot", true);

                        timer -= Time.deltaTime;
                    }
                }
                else
                {
                    timer = -1;
                    weaponAnim.SetBool("Shoot", false);
                }

            }

            if (thirdPersonCam == null)
            {
                thirdPersonCam = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponentInChildren<Camera>();
            }

            // yield return new WaitForFixedUpdate();
        }
    }

    void RaycastShoot()
    {
        Vector3 posOrigin = thirdPersonCam.transform.position;
        if (Physics.Raycast(posOrigin, thirdPersonCam.transform.forward, out RaycastHit hit, 3200, layerMask))
        {
            //Debug.Log($"CmdRaycastShoot {posOrigin} > {hit.point}");
            Debug.DrawRay(posOrigin, thirdPersonCam.transform.forward * 3000, Color.red, 2.0f);

            CmdRaycastShoot(hit.transform.gameObject, hit.point);


        }
    }

    [Command]
    void CmdRaycastShoot(GameObject hitTarget, Vector3 hitPoint)
    {

        if (hitTarget != null)
        {
            //RpcTakeDamage(hitTarget);

            GameObject go = Instantiate(smokeParticle, hitPoint, Quaternion.identity) as GameObject;
            go.transform.LookAt(bulletSpawnPoint.position);


            if (go.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker)) //Need Jareds help
            {
                networkMatchChecker.matchId = GetComponent<Player>().matchID.ToGuid();
            }

            NetworkServer.Spawn(go, connectionToClient);



           // return;


            if (hitTarget.GetComponent<HealthScript>()) //Mario
            {
                hitTarget.GetComponent<HealthScript>().Health -= 10;
                //hitTarget.GetComponent<HealthScript>().bBulletShot = true;

            }

            if (hitTarget.GetComponent<HealthScriptPikachu>() && hitTarget.GetComponent<HealthScriptPikachu>().Health > 0) //Pikachu
            {
                if (hitTarget.GetComponent<HealthScriptPikachu>().pikachuStar.bStarMode) { return; }

                hitTarget.GetComponent<HealthScriptPikachu>().Health -= 10;

                if (hitTarget.GetComponent<HealthScriptPikachu>().Health <= 0
                    && hitTarget.GetComponent<HealthScriptPikachu>().IsDead == false
                    && hitTarget.GetComponent<HealthScript>().isActiveAndEnabled == false)
                {
                    killCount.kills++;
                }
            }

        }
        else //Hits no target
        {
            GameObject go = Instantiate(smokeParticle, hitPoint, Quaternion.identity) as GameObject;
            go.transform.LookAt(bulletSpawnPoint.position);

            if (go.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker)) //Need Jareds help
            {
                // networkMatchChecker.matchId = _matchID.ToGuid();
                networkMatchChecker.matchId = GetComponent<Player>().matchID.ToGuid();
            }

            NetworkServer.Spawn(go, connectionToClient);
        }
    }

    // [ClientRpc]
    // void RpcTakeDamage(GameObject hitTarget)
    // {
    //     if (hitTarget.GetComponent<HealthScript>()) //Mario
    //     {
    //         //hitTarget.GetComponent<HealthScript>().Health -= 10;
    //         hitTarget.GetComponent<HealthScript>().TakeDamage(10);

    //         if (hitTarget.GetComponent<HealthScript>().Health <= 0
    //             && hitTarget.GetComponent<HealthScript>().IsDead == false)
    //         {
    //             //killCount.kills++; //MARIO KILL COUNT +1
    //         }
    //     }


    //     if (hitTarget.GetComponent<HealthScriptPikachu>() && hitTarget.GetComponent<HealthScriptPikachu>().Health > 0) //Pikachu
    //     {
    //         //if (hitTarget.GetComponent<HealthScriptPikachu>().pikachuStar.bStarMode) { return; }
    //         hitTarget.GetComponent<HealthScriptPikachu>().Health -= 10;

    //         if (hitTarget.GetComponent<HealthScriptPikachu>().Health <= 0
    //             && hitTarget.GetComponent<HealthScriptPikachu>().IsDead == false
    //             && hitTarget.GetComponent<HealthScript>().isActiveAndEnabled == false)
    //         {
    //             killCount.kills++;
    //         }
    //     }
    // }

}






////NEW
//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;
//using UnityEngine.UI;

//public class HandleShooting : NetworkBehaviour
//{
//    StateManager states;
//    HealthScript marioHealth;
//    public Animator weaponAnim;
//    public float fireRate;
//    float timer;
//    public Transform bulletSpawnPoint;
//    public GameObject smokeParticle;
//    public ParticleSystem[] muzzle;
//    public GameObject casingPrefab;
//    public Transform caseSpawn;

//    //NEW CODE
//    public Camera thirdPersonCam;
//    //NEW CODE (PUT IN HUD SCRIPT EVENTUALLY!)
//    public Text ammoText;
//    //NEW CODE
//    public LayerMask layerMask = new LayerMask();

//    public int BulletsMax = 30;
//    [SyncVar]
//    public int curBullets;

//    bool bKill = false;

//    KillCount killCount = null;

//    public bool bShowAmmoText = true; //CHEK HERE

//    // [SyncVar]
//    public Transform aimHelper;
//    private Collider ownCollider;


//    //public override void OnStartLocalPlayer () {
//    void Start()
//    {
//        StartCoroutine(BulletDelaySpawn());

//        curBullets = BulletsMax;
//        ownCollider = GetComponent<Collider>();
//        states = GetComponent<StateManager>();
//        killCount = GetComponent<KillCount>();
//        marioHealth = GetComponent<HealthScript>();

//       // ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
//       // ammoText.text = "AMMO: " + curBullets.ToString();

//        bKill = false;

//        //GameOverManager.instance.CursorPrefab.SetActive(true);

//        //StartCoroutine(UpdateClient()); //FixedUpdate


//        //thirdPersonCam = GameObject.Find("Camera Holder (Mario)/Pivot/Holder 2/Main Camera 2").GetComponent<Camera>();

//    }

//    bool shoot;
//    bool dontShoot;

//    bool emptyGun;


//    public override void OnStartAuthority() //Doesn spawn at lobby, in scene
//    {
//        //GameOverManager.instance.CursorPrefab.SetActive(true); //Pikachu in HealthScriptPikachu

//       // ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
//       // ammoText.text = "AMMO: " + curBullets.ToString();
//    }

//    IEnumerator BulletDelaySpawn()
//    {
//        yield return new WaitForSeconds(0.32f);
//        curBullets = 0;
//        yield return new WaitForSeconds(0.32f);
//        curBullets = BulletsMax;
//    }

//    //IEnumerator UpdateClientFixed() //DUNNO
//    [Client]
//    void FixedUpdate()
//    {
//        //while (true)
//        {

//            //if (netIdentity.hasAuthority)
//            {
//                //if (thirdPersonCam == null)
//                {
//                    //thirdPersonCam = FreeCameraLook.GetInstance().GetComponentInChildren<Camera>();
//                    //thirdPersonCam = GameObject.FindGameObjectWithTag("MarioCamera").GetComponentInChildren<Camera>();

//                    //thirdPersonCam = GameObject.Find("/Mario Shooter/Camera Holder (Mario)/Pivot/Holder 2/Main Camera 2").GetComponent<Camera>();
//                }



//                if (!hasAuthority && TurnManager.instance.bPikachuWins) { return; }

//                //if (ammoText == null)
//                //{
//                //    //ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
//                //    ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();

//                //}
//                //if (ammoText)
//                //{
//                //    ammoText.text = "AMMO: " + curBullets.ToString();
//                //}

//                ////if (aimHelper == null)
//                ////{
//                ////    aimHelper = GameObject.FindGameObjectWithTag("CameraCenter").transform;
//                ////}

//                shoot = states.shoot;

//                if (shoot)
//                {
//                    if (timer <= 0)
//                    {
//                        weaponAnim.SetBool("Shoot", false);

//                        if (curBullets > 0)
//                        {
//                            emptyGun = false;
//                            // states.audioManager.PlayGunSound();

//                            GameObject go = Instantiate(casingPrefab, caseSpawn.position, caseSpawn.rotation) as GameObject;
//                            Rigidbody rig = go.GetComponent<Rigidbody>();
//                            rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
//                            rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);

//                            for (int i = 0; i < muzzle.Length; i++)
//                            {
//                                muzzle[i].Emit(1);
//                            }

//                            RaycastShoot();
//                            states.audioManager.CmdPlayGunSound();

//                            curBullets = curBullets - 1;

//                        }
//                        else
//                        {
//                            if (emptyGun)
//                            {
//                                states.handleAnim.StartReload();
//                                //states.audioManager.PlayGunReload();
//                                states.audioManager.CmdPlayGunReload();
//                                curBullets = BulletsMax;
//                            }
//                            else
//                            {
//                                states.audioManager.PlayEffect("empty_gun");
//                                emptyGun = true;
//                            }
//                        }

//                        timer = fireRate;
//                    }
//                    else
//                    {
//                        weaponAnim.SetBool("Shoot", true);

//                        timer -= Time.deltaTime;
//                    }
//                }
//                else
//                {
//                    timer = -1;
//                    weaponAnim.SetBool("Shoot", false);
//                }

//            }

//            if (thirdPersonCam == null)
//            {
//                thirdPersonCam = GameObject.FindGameObjectWithTag("MarioCamera2").GetComponentInChildren<Camera>();
//            }

//            // yield return new WaitForFixedUpdate();
//        }
//    }

//    void RaycastShoot()
//    {
//        Vector3 posOrigin = thirdPersonCam.transform.position;
//        if (Physics.Raycast(posOrigin, thirdPersonCam.transform.forward, out RaycastHit hit, 3200, layerMask))
//        {
//            //Debug.Log($"CmdRaycastShoot {posOrigin} > {hit.point}");
//            Debug.DrawRay(posOrigin, thirdPersonCam.transform.forward * 3000, Color.red, 2.0f);

//            CmdRaycastShoot(hit.transform.gameObject, hit.point);


//        }
//    }

//    [Command]
//    void CmdRaycastShoot(GameObject hitTarget, Vector3 hitPoint)
//    {

//        if (hitTarget != null)
//        {
//            RpcTakeDamage(hitTarget);

//            GameObject go = Instantiate(smokeParticle, hitPoint, Quaternion.identity) as GameObject;
//            go.transform.LookAt(bulletSpawnPoint.position);

//            NetworkServer.Spawn(go, connectionToClient);

//            if (go.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker)) //Need Jareds help
//            {
//               // networkMatchChecker.matchId = _matchID.ToGuid();
//            }




//            return;


//            if (hitTarget.GetComponent<HealthScript>()) //Mario
//            {
//                hitTarget.GetComponent<HealthScript>().Health -= 10;
//                //hitTarget.GetComponent<HealthScript>().bBulletShot = true;

//            }

//            if (hitTarget.GetComponent<HealthScriptPikachu>()) //Pikachu
//            {
//                if (hitTarget.GetComponent<PickupStarPikachu>().bStarMode == false)
//                {
//                    hitTarget.GetComponent<HealthScriptPikachu>().Health -= 10;

//                    if (hitTarget.GetComponent<HealthScriptPikachu>().Health <= 0 && hitTarget.GetComponent<HealthScriptPikachu>().bKillCountDisable == false)
//                    {
//                        killCount.kills++;
//                        hitTarget.GetComponent<HealthScriptPikachu>().bKillCountDisable = true;
//                    }
//                }
//                //hitTarget.GetComponent<HealthScriptPikachu>().TakeDamage(10, gameObject);
//            }

//        }
//        else //Hits no target
//        {
//            GameObject go = Instantiate(smokeParticle, hitPoint, Quaternion.identity) as GameObject;
//            go.transform.LookAt(bulletSpawnPoint.position);

//            NetworkServer.Spawn(go, connectionToClient);

//            //?? HIt spawn TWICE
//        }
//    }

//    [ClientRpc]
//    void RpcTakeDamage(GameObject hitTarget)
//    {
//        if (hitTarget.GetComponent<HealthScript>()) //Mario
//        {
//            //hitTarget.GetComponent<HealthScript>().Health -= 10;
//            hitTarget.GetComponent<HealthScript>().TakeDamage(10);

//            if (hitTarget.GetComponent<HealthScript>().Health <= 0
//                && hitTarget.GetComponent<HealthScript>().IsDead == false)
//            {
//                //killCount.kills++; //MARIO KILL COUNT +1
//            }
//        }


//        if (hitTarget.GetComponent<HealthScriptPikachu>()) //Pikachu
//        {
//            //if (hitTarget.GetComponent<HealthScriptPikachu>().pikachuStar.bStarMode) { return; }
//            hitTarget.GetComponent<HealthScriptPikachu>().Health -= 10;

//            if (hitTarget.GetComponent<HealthScriptPikachu>().Health <= 0
//                && hitTarget.GetComponent<HealthScriptPikachu>().IsDead == false)
//            {
//                killCount.kills++;
//            }
//        }
//    }

//}


