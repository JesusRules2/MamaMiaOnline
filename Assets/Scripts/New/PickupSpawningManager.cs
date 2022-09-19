using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupSpawningManager : NetworkBehaviour
{
   // public static PickupSpawningManager instance = null; //Server problem?????????????????

    public GameObject starPrefab;
    public bool bStopSpawningOLD = false;
    public float spawnTime = 8.0f;
    public float spawnDelay = 23.0f;

    public bool bStopSpawning = false;

    public override void OnStartServer()
    {
       // instance = this;
        InvokeRepeating("SpawnStarPickup", spawnTime, spawnDelay);
        //Debug.Log("OnStartServer function call");
        //StartCoroutine(SpawnStarPickup());
        //StartCoroutine(SpawnStarPickupDelay());

    }

    void Start()
    {
        InvokeRepeating("SpawnStarPickup", spawnTime, spawnDelay);
    }

    IEnumerator SpawnStarPickupDelay()
    {
        yield return new WaitForSeconds(8.2f);

        SpawnStarPickup();
    }

    // IEnumerator SpawnStarPickup()
    //[Command]
    void SpawnStarPickup()
    {
        //yield return new WaitForSeconds(6.3f);

        //if (bStopSpawning) { return; }

        Debug.Log("STAR SPAWNED!!!");

        Transform startPosition = NetworkManager.singleton.GetStartPosition();

        GameObject newGameObject = Instantiate(starPrefab,
            startPosition.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);


        if (newGameObject.TryGetComponent<NetworkMatchChecker>(out NetworkMatchChecker networkMatchChecker)) //Need Jareds help
        {
            // Debug.Log("Check Console for line 40"); 
            //networkMatchChecker.matchId = GetComponent<NetworkMatchChecker>().matchId;
            networkMatchChecker.matchId = GameObject.Find("----TurnManager").GetComponent<NetworkMatchChecker>().matchId; //1
            // Debug.Log($"matchID: {networkMatchChecker.matchId}");
        }

        NetworkServer.Spawn(newGameObject);

        //CmdSpawnStars(startPosition);

        if (bStopSpawningOLD)
        {
            CancelInvoke("SpawnStarPickup");
        }
    }

   // [Command]
    void CmdSpawnStars(Transform startPosition)
    {
        //GameObject newGameObject = Instantiate(starPrefab,
            //startPosition.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);

        //NetworkServer.Spawn(newGameObject);

        //RpcSpawnStars(startPosition);

    }
  //  [ClientRpc]
    void RpcSpawnStars(Transform startPosition)
    {
        GameObject newGameObject = Instantiate(starPrefab,
            startPosition.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);

        NetworkServer.Spawn(newGameObject);
    }

    public void ResetGamePickups()
    {
        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        foreach (GameObject star in stars)
        {
            Destroy(star);
        }

        //InvokeRepeating("SpawnStarPickup", spawnTime, spawnDelay);
    }

    void Update()
    {
        //SpawnStarPickup();
    }
}
