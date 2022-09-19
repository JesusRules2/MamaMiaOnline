using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioManager : NetworkBehaviour
{
    public AudioSource gunSounds;
    public AudioSource gunReload;
    public AudioSource runFoley;
    public AudioSource jumpSound;
    public AudioSource landSound;
    public bool landed = false;
    public float jumpTimer = 0f;

    public AudioClip[] jumpSoundClips;


    public AudioSource deathSound;
    public AudioClip[] deathClips;

    public float footStepTimer;
    public float walkThreshold;
    public float runThreshold;
    public AudioSource footStep1;
    public AudioSource footStep2;
    public AudioClip[] footStepClips;
    public AudioSource effectsSource;
    public AudioClipsList[] effectList;
    StateManager states;

    float startingVolumeRun;
    float characterMovement;


    void Start()
    {
        states = GetComponent<StateManager>();
        startingVolumeRun = runFoley.volume;

        runFoley.volume = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        characterMovement = Mathf.Abs(states.horizontal) + Mathf.Abs(states.vertical);
        characterMovement = Mathf.Clamp01(characterMovement);

        float targetThreshold = 0;

        if (!states.walk && !states.aiming && !states.reloading)
        {
            runFoley.volume = startingVolumeRun * characterMovement;
            targetThreshold = runThreshold;
        }
        else
        {
            targetThreshold = walkThreshold;

            runFoley.volume = Mathf.Lerp(runFoley.volume, 0, Time.deltaTime * 2);
        }

        if (characterMovement > 0 && states.onGround)
        {
            runFoley.volume = startingVolumeRun * characterMovement;

            footStepTimer += Time.deltaTime;

            if (footStepTimer > targetThreshold)
            {
                CmdPlayFootStep();
                footStepTimer = 0;
            }
        }
        else
        {
            runFoley.volume = 0;
            footStep1.Stop();
            footStep2.Stop();
        }

        jumpTimer += Time.deltaTime;

        if (!landed && states.onGround && jumpTimer > 0.32f) //Landing sound turned off
        {
            landed = true;
            //PlayLandSound();
        }
    }

    [Command]
    public void CmdPlayGunSound()
    {
        //  if (!hasAuthority) { return; }
        //gunSounds.Play();
        RpcPlayGunSound();
    }
    [ClientRpc]
    void RpcPlayGunSound()
    {
        gunSounds.Play();
    }


    [Command]
    public void CmdPlayGunReload()
    {
        //if (!hasAuthority) { return; }
        //gunReload.Play();
        RpcPlayGunReload();
    }
    [ClientRpc]
    void RpcPlayGunReload()
    {
        gunReload.Play();

    }

    [Command]
    public void CmdPlayFootStep()
    {
        //if (!hasAuthority) { return; }

        RpcPlayFootStep();

    }
    [ClientRpc]
    void RpcPlayFootStep()
    {
        if (!footStep1.isPlaying)
        {
            int ran = Random.Range(0, footStepClips.Length);
            footStep1.clip = footStepClips[ran];

            footStep1.Play();
        }
        else
        {
            if (!footStep2.isPlaying)
            {
                int ran2 = Random.Range(0, footStepClips.Length);
                footStep2.clip = footStepClips[ran2];
                footStep2.Play();
            }
        }
    }


    public void PlayEffect(string name)
    {
        AudioClip clip = null;

        for (int i = 0; i < effectList.Length; i++)
        {
            if (string.Equals(effectList[i].name, name))
            {
                clip = effectList[i].clip;
                break;
            }
        }

        effectsSource.clip = clip;
        effectsSource.Play();
    }

    public void PlayDeathSound()
    {
      //  if (!hasAuthority) { return; }

        int ran = Random.Range(0, deathClips.Length);
        deathSound.clip = deathClips[ran];

        deathSound.Play();
    }

    [Command]
    public void CmdPlayJumpSound()
    {
        // if (!hasAuthority) { return; }
        int ran = Random.Range(0, jumpSoundClips.Length);
        jumpSound.clip = jumpSoundClips[ran];
        GetComponent<HandleAnimations>().JumpAnim();

        RpcPlayJumpSound();
    }
    [ClientRpc]
    void RpcPlayJumpSound()
    {
        jumpSound.Play();
    }


    public void PlayLandSound()
    {
        //if (!hasAuthority) { return; }

        landSound.Play();
    }
}




[System.Serializable]
public class AudioClipsList
{
    public string name;
    public AudioClip clip;
}
