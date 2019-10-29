﻿using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Level1Manager : MonoBehaviour, ILevelManager
{
    public static Level1Manager Instance { get; private set; }

    public int state;

    [SerializeField] GameObject[] enemySpawners = default;
    [SerializeField] GameObject[] timelines = default;
    [SerializeField] AudioClip audioClipBackgroundMusic = default;
    [SerializeField] AudioClip audioClipWowGreatShot = default;
    [SerializeField] AudioClip audioClipYouGotAllTheTargets = default;
    [SerializeField] AudioClip[] sound_effects= default;

    private AudioSource audioSourceBackgroundMusic;
    private AudioSource audioSourceVoiceOver;
    private AudioManager audioManager;


    private void Awake()
    {
        Instance = this;
        audioSourceBackgroundMusic = GetComponents<AudioSource>()[0];
        audioSourceVoiceOver = GetComponents<AudioSource>()[1];
        audioManager = AudioManager.GetAudioManager();
    }

    void Start()
    {
        QualitySettings.shadowDistance = 450;
        UpdateState();
    }

    public void GetSoundEffects(out AudioClip[] fx)
    {
        fx = sound_effects;
    }

    public void UpdateState()
    {
        if (state == 0) //Opening scene, audio introduction
        {
            timelines[0].SetActive(true);
            NextState((float)timelines[0].GetComponent<PlayableDirector>().duration + 1);
        }
        else if (state == 1) //Spawn enemies
        {
            enemySpawners[0].SetActive(true);  //Chinook Spawner
            NextState(0);
        }
        else if (state == 2) //Waiting for the Player to defeat all the targets
        {
            if (EnemyManager.Instance.GetEnemyCount() == 9)
            {
                //PlayAudio(audioClipWowGreatShot, 0);
                audioManager.PlayNarration(audioClipWowGreatShot);
            }

            if (EnemyManager.Instance.GetEnemyCount() <= 0)
            {
                //PlayAudio(audioClipYouGotAllTheTargets, 0);
                audioManager.PlayNarration(audioClipYouGotAllTheTargets);
                NextState(3);
            }
        }
        else if (state == 3)
        {
            timelines[1].SetActive(true);
            NextState(3);
        }
        else if (state == 4)    //Attack Helicopter Spawner
        {
            enemySpawners[1].SetActive(true);
            NextState(0);
        }
        else if (state == 5)    //Waiting for the Player to defeat all the targets
        {
            if (EnemyManager.Instance.GetEnemyCount() <= 0)
            {
                NextState(0);
            }
        }
        else if (state == 6)
        {
            timelines[2].SetActive(true);
        }
    }

    private void PlayAudio(AudioClip clip, float time)
    {
        StartCoroutine(PlayAudioInTime(clip, time));
    }

    IEnumerator PlayAudioInTime(AudioClip clip, float time)
    {
        yield return new WaitForSeconds(time);
        audioSourceVoiceOver.PlayOneShot(clip);
    }

    private void NextState(float time)
    {
        StartCoroutine(NextStateInTime(time));
    }

    IEnumerator NextStateInTime(float time)
    {
        yield return new WaitForSeconds(time);
        state++;
        UpdateState();
    }
}
