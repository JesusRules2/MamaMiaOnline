using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StarScript : NetworkBehaviour {
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _speed;

    public float spawnTime = 0;
    public float spawnDelay = 5;

    public AudioSource starTwinkleSound;

    public override void OnStartClient () {
        StarSound ();
    }

    //public override void OnStartServer () {
    //    //InvokeRepeating("StarSound", spawnTime, spawnDelay);
    //    StartCoroutine (DeleteStar ());
    //}
    void Start()
    {
        StartCoroutine(DeleteStar());
    }

    IEnumerator DeleteStar () {
        yield return new WaitForSeconds (15.0f); //15.0 REGULAR

        NetworkServer.Destroy (gameObject);
    }

    void StarSound () {
        //CmdPlaySFX();
        //starTwinkleSound.Play();
        if (NetworkClient.active == true) //DUNNO
        {
            // CmdPlaySFX ();
            starTwinkleSound.Play ();
        } else {
            starTwinkleSound.Stop ();

        }

    }

    // Update is called once per frame
    void Update () {
        transform.Rotate (_rotation * _speed * Time.deltaTime);
        transform.Translate (Vector3.forward * Time.deltaTime);
    }

    [Command]
    void CmdPlaySFX()
    {
        RpcPlaySFX();
    }

    [ClientRpc]
    void RpcPlaySFX()
    {
        starTwinkleSound.Play();

    }

}