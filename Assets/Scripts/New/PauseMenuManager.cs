using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Mirror;

public class PauseMenuManager : MonoBehaviour
{
    bool bShowupMenu = false;
    public GameObject pauseMenu = null;
    private GameObject quitButton = null;


    void Start()
    {
        quitButton = pauseMenu.GetComponent<PauseMenu>().quitButton;
    }

    //IEnumerator UpdateClient()
    void Update()
    {
        //while (true)
        {
            PauseMenuFunction();

            //yield return null;
        }
    }

    void PauseMenuFunction()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("ShowMenu"))
        {
            bShowupMenu = !bShowupMenu;
             //clear selected object
            EventSystem.current.SetSelectedGameObject(null);
            //select a new selected object
            EventSystem.current.SetSelectedGameObject(quitButton);
        }

        if (bShowupMenu)
        {
            pauseMenu.SetActive(true);
        }
        if (!bShowupMenu)
        {
            pauseMenu.SetActive(false);
        }

    }

    public void GameStartVolume() {
        pauseMenu.GetComponent<PauseMenu>().GameBeginVolume();
    }
}
