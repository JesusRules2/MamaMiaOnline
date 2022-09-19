using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] public GameObject quitButton;

    // Start is called before the first frame update
    void Start()
    {
        //if (!PlayerPrefs.HasKey("gameVolume"))
        //{
        //    PlayerPrefs.SetFloat("gameVolume", 1);
        //    Load();
        //}
        //else
        //{
        //    Load();
        //}
        volumeSlider.value = 1; //default
    }

    public void GameBeginVolume() {
        volumeSlider.value = 1;
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("gameVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("gameVolume", volumeSlider.value);
    }

    public void QuitGame()
    {
        //GameOverManager.instance.bHidePanelsBeginning = true;
        Application.Quit();
    }
}
