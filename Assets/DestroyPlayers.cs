using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayers : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(DestroyPlayers2());
    }
    void Start()
    {
        StartCoroutine(DestroyPlayers2());

    }

    IEnumerator DestroyPlayers2()
    {
        yield return new WaitForSeconds(0.2f);
        //GameOverManager.instance.bHidePanelsBeginning = true;

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allPlayers)
        {
            Destroy(player.gameObject);
        }
    }
}
