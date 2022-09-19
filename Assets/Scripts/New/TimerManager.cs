using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class TimerManager : NetworkBehaviour
{
    public static TimerManager instance = null;

    //[SyncVar]
    //public float timeStart = 90;
   [SyncVar]
    public float timeValue;
   // [SyncVar]
    public Text timeText;
   [SyncVar]
    public bool GameOver = false;
    [SyncVar]
    public bool ShowTimer = true;

    TurnManager turnManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //Destroy(gameObject);
        }
    }

    void Start()
    {
        ShowTimer = true;
        turnManager = GetComponent<TurnManager>();
        timeValue = turnManager.timeStart;
    }

    void Update()
    {
        //if (timeValue > 0 && GameOverManager.instance.bMarioWins == false)
        if (timeValue > 0 && turnManager.bMarioWins == false)
        {
            // timeValue -= Time.deltaTime; //DISABLED TIMER
        }

        if (timeValue <= 0)
        {
            timeValue = 0;
            turnManager.bPikachuWins = true;
            // turnManager.bMarioWins = false;
            // timeValue += 90;
        }

        if (ShowTimer)
        {
            DisplayTime(timeValue);
        }
        if (ShowTimer == false)
        {
           // timeValue = GameOverManager.instance.timeStart;
            timeText.text = "";
        }

    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
            
        }
        else if(timeToDisplay > 0) //OFF!!!!!!!!
        {
            timeToDisplay += 1;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        //float milliseconds = timeToDisplay % 1 * 1000; //OFF

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        //timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

}






    //public Text textDisplay;
    //public int secondLeft = 30;
    //public bool takingAway = false;

    //void Start()
    //{
    //    textDisplay.text = "00:" + secondLeft;

    //}

    //void Update()
    //{
    //    if (takingAway == false && secondLeft > 0)
    //    {
    //        StartCoroutine(TimerTake());
    //    }
    //}

    //IEnumerator TimerTake()
    //{
    //    takingAway = true;
    //    yield return new WaitForSeconds(1);
    //    secondLeft -= 1;
    //    if (secondLeft < 10)
    //    {
    //        textDisplay.text = "00:0" + secondLeft
    //    }
    //    else
    //    {
    //       textDisplay.text = "00:" + secondLeft;
    //    }
    //    takingAway = false;
    //}