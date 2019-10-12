﻿using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager Instance { get; private set; }

    int state = 0;

    [SerializeField] GameObject[] enemySpawners;
    [SerializeField] GameObject[] timelines;
    [SerializeField] AudioClip audioClipWowGreatShot;
    [SerializeField] AudioClip audioClipYouGotAllTheTargets;
    private AudioSource audioSource;


    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        UpdateState();
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
            enemySpawners[0].SetActive(true);
            NextState(0);
        }
        else if (state == 2) //Waiting for the Player to defeat all the targets
        {
            if (EnemyManager.Instance.GetEnemyCount() == 9)
            {
                PlayAudio(audioClipWowGreatShot, 0);
            }

            if (EnemyManager.Instance.GetEnemyCount() <= 0)
            {
                PlayAudio(audioClipYouGotAllTheTargets, 0);
                NextState(0);
            }
        }
        else if (state == 3)
        {

        }
    }

    private void PlayAudio(AudioClip clip, float time)
    {
        StartCoroutine(PlayAudioInTime(clip, time));
    }

    IEnumerator PlayAudioInTime(AudioClip clip, float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(clip);
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
