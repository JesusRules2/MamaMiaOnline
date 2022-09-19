using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnOffGameOverPanels : MonoBehaviour
{
    //public static TurnOffGameOverPanels instance = null;

    public bool bTurnOnGameOverManager;
    public GameObject canvasGameManager;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else if (instance != this)
        //{
        //    Destroy(gameObject);
        //}
    }

    void Start()
    {
        if (!bTurnOnGameOverManager)
        {
            //GameObject.Find("GameOverCanvas").SetActive(false);
            //GameObject.Find("----GameOverManager(Clone)").SetActive(false);
            //canvasGameManager.SetActive(false);

        }
        if (bTurnOnGameOverManager)
        {
            //canvasGameManager.SetActive(true);

            StartCoroutine(Activate());
        }
    }

    void Update()
    {
        //if (!hasAuthority) { return; }

       // GameObject.Find("GameOverCanvas").SetActive(true);
       // canvasGameManager.SetActive(true);

        if (bTurnOnGameOverManager)
        {
            //GameObject.Find("----GameOverManager(Clone)").SetActive(true);
            //GameObject.Find("GameOverCanvas").SetActive(true);
            //canvasGameManager.SetActive(true);

        }

        if (!bTurnOnGameOverManager)
        {
            //GameObject.Find("----GameOverManager(Clone)").SetActive(true);
            //GameObject.Find("GameOverCanvas").SetActive(true);
            //canvasGameManager.SetActive(false);

        }
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(0.032f);

        //canvasGameManager.SetActive(true);

    }
}
