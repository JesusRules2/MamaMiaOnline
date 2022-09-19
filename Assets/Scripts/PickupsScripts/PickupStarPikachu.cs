using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupStarPikachu : NetworkBehaviour
{
    [SyncVar]
    public bool bStarMode;
    [SyncVar]
    public float starModeDuration = 8.2f;
    public AudioSource starModeSFX;
    public GameObject starModeSFXObject;

    PlayerController_Platform playerController;
    public float starMoveSpeed = 32.0f;
    public Material yellowSkin;
    public Material starModeSkin;
    public Renderer rend = null;

    [SyncVar]
    bool bPlayStar;

    NetworkMatchChecker networkMatchChecker;

    // public AudioClip[] deathClips; example

    void Awake()
    {
        networkMatchChecker = GetComponent<Player>().networkMatchChecker;
    }
    // Start is called before the first frame update
    void Start()
    {
        bPlayStar = false;
        playerController = GetComponent<PlayerController_Platform>();
        //rend.sharedMaterial = yellowSkin;
    }

    // Update is called once per frame
    void Update()
    {

        if (bStarMode)
        {
            playerController.moveSpeed = starMoveSpeed;
            rend.sharedMaterial = starModeSkin;
        }
        if (!bStarMode)
        {
            playerController.moveSpeed = playerController.moveSpeedMax;
            rend.sharedMaterial = yellowSkin;
        }

        starModeSFXObject.SetActive(true);
        starModeSFX.enabled = true;

        //if (bPlayStar)
        //{
        //    starModeSFX.Play();
        //    StartCoroutine(StopSound());
        //}

    }

    void OnTriggerEnter(Collider col)
    {
            if (GetComponent<PlayerSwitch> ().bGameStarted == false) {
            return; //game not loaded
        }


        if (col.gameObject.CompareTag("Star") && col.gameObject.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker matchChecker))
        {
            if (matchChecker.matchId == networkMatchChecker.matchId)
            {
                if (bStarMode) { return; }
                if (GetComponent<PlayerMovement>().isActiveAndEnabled) { return; }

                rend.sharedMaterial = starModeSkin;
                StartCoroutine(StarMode());
                DestroyStar(col.gameObject);

                if (hasAuthority)
                {
                    //CmdStarSoundOnly();
                    RpcStarSoundOnly();
                }

                RpcStarSoundOnly();
            }
        }
    }

    //IEnumerator DestroyStar(GameObject star)
    void DestroyStar(GameObject star)
    {
       // yield return new WaitForSeconds(0.132f);

        Destroy(star.gameObject);
        NetworkServer.Destroy(star.gameObject);
    }

    IEnumerator StarMode()
    {
        //CmdPlayStarSFX();
        //starModeSFX.Play();
        bStarMode = true;

        yield return new WaitForSeconds(starModeDuration);

        bStarMode = false;
        rend.sharedMaterial = yellowSkin;
        CmdStopStarSFX();

    }

    //[Command]
    //public void CmdPlayStarSFX()
    //{
    //    rend.sharedMaterial = starModeSkin;
    //    RpcPlayStarSFX();
    //    //starModeSFX.Play();


    //}
    //[ClientRpc]
    //void RpcPlayStarSFX()
    //{
    //    rend.sharedMaterial = starModeSkin;
    //    starModeSFX.Play();
    //    //bPlayStar = true;
    //}
    IEnumerator PlaySFX()
    {
        yield return new WaitForSeconds(0.032f);
        CmdStarSoundOnly();
        yield return new WaitForSeconds(0.032f);
        CmdStarSoundOnly();
        yield return new WaitForSeconds(0.032f);
        CmdStarSoundOnly();
        yield return new WaitForSeconds(0.032f);
        CmdStarSoundOnly();
    }

    [Command]
    public void CmdStarSoundOnly()
    {
        RpcStarSoundOnly();
    }
    [ClientRpc]
    void RpcStarSoundOnly()
    {
        starModeSFX.Play();
    }




    [Command]
    public void CmdStopStarSFX()
    {
        RpcStopStarSFX();
        rend.sharedMaterial = yellowSkin;
    }
    [ClientRpc]
    void RpcStopStarSFX()
    {
        starModeSFX.Stop();
        rend.sharedMaterial = yellowSkin;

    }



    IEnumerator StopSound()
    {
        yield return new WaitForSeconds(0.1f);
        bPlayStar = false;
    }

}
