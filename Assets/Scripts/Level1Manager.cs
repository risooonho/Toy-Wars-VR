﻿using System.Collections;
using UnityEngine;

public class Level1Manager : MonoBehaviour, ILevelManager
{
    public static Level1Manager Instance { get; private set; }

    public int state;

    [SerializeField] GameObject popUpTargetEnemySpawner1 = default;
    [SerializeField] GameObject popUpTargetEnemySpawner2 = default;
    [SerializeField] GameObject popUpTargetEnemySpawner3 = default;

    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly1 = default;
    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly2 = default;
    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly3 = default;

    [SerializeField] GameObject spitfireEnemySpawnerDolly1 = default;
    [SerializeField] GameObject spitfireEnemySpawnerDolly2 = default;
    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly4 = default;
    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly5 = default;
    [SerializeField] GameObject attackHelicopterEnemySpawnerDolly6 = default;

    [SerializeField] GameObject zeppelin = default;

    [SerializeField] AudioClip[] sound_effects = default;

    [SerializeField] GameObject playerStatistics = default;
    [SerializeField] GameObject thanksForPlayingOurDemo = default;

    [SerializeField] AudioClip[] NarrationSequences1 = default;

    [SerializeField] AudioClip PrettyEasyWhenTheyDontShootBack = default;
    [SerializeField] AudioClip YouveGotSomeSkills = default;
    [SerializeField] AudioClip YoureDownToTheFinalFour = default;

    [SerializeField] AudioClip AngelsAndMinistersOfGraceDefendUs = default;
    [SerializeField] AudioClip NotBadRecruit = default;
    [SerializeField] AudioClip YouMustPlayALotOfFortnite = default;

    [SerializeField] AudioClip[] NarrationSequences2 = default;
    [SerializeField] AudioClip[] NarrationSequences3 = default;

    [SerializeField] AudioClip ILoveTheSmellOfOrangeJuiceInTheMorning = default;

    [SerializeField] AudioClip[] NarrationSequences4 = default;

    [SerializeField] AudioClip BGM_MainMenu = default;
    [SerializeField] AudioClip BGM_PopUpTargets = default;
    [SerializeField] AudioClip BGM_Action = default;
    [SerializeField] AudioClip BGM_Boss = default;
    [SerializeField] AudioClip BGM_Win = default;

    [SerializeField] AudioClip BaseWarning = default;


    private AudioManager audioManager;

    private readonly float POPUPTIMER_TIME_LIMIT = 15;
    private float PopUpTargetEndTime;

    private void Awake()
    {
        Instance = this;
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

    private void PopUpTargetTimerNotification()
    {
        GotoState(5, 0);
    }

    private void NarrateSequenceAndNextState(AudioClip[] clips)
    {
        StartCoroutine(NarrateSequenceAndNextStateCR(clips));
    }

    IEnumerator NarrateSequenceAndNextStateCR(AudioClip[] clips)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            audioManager.PlayNarration(clips[i]);
            yield return new WaitForSeconds(clips[i].length);
        }
        NextState(0);
    }

    public void UpdateState()
    {
        if (state == -1)
        {
            NextState(1);
            audioManager.ChangeBGM(BGM_PopUpTargets);
            audioManager.StartBGM();
        }
        else if (state == 0) //Opening scene, audio introduction
        {
            NarrateSequenceAndNextState(NarrationSequences1);
        }
        else if (state == 1) //pop up first set of 4 targets
        {
            TVCamera.Instance.ScreenOn();
            PopUpTargetEndTime = Time.time + POPUPTIMER_TIME_LIMIT;
            TVCamera.Instance.StartTimer();
            Invoke("PopUpTargetTimerNotification", POPUPTIMER_TIME_LIMIT);

            popUpTargetEnemySpawner1.SetActive(true);
            NextState(0);
        }
        else if (state == 2) //pop up second set of 4 targets
        {
            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() == 4)
            {
                popUpTargetEnemySpawner2.SetActive(true);
                NextState(0);
            }
        }
        else if (state == 3) //pop up third set of 4 targets
        {
            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() == 8)
            {
                popUpTargetEnemySpawner3.SetActive(true);
                NextState(0);
            }
        }
        else if (state == 4) //All targets defeated or time expires
        {
            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() == 12)
            {
                NarrateSequenceAndNextState(NarrationSequences2);
            }
        }
        else if (state == 5) //Time fail
        {
            popUpTargetEnemySpawner1.SetActive(false);
            popUpTargetEnemySpawner2.SetActive(false);
            popUpTargetEnemySpawner3.SetActive(false);
            TVCamera.Instance.StopTimer();

            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() <= 4)
            {
                audioManager.PlayNarration(AngelsAndMinistersOfGraceDefendUs);
                NextState(AngelsAndMinistersOfGraceDefendUs.length + 1);
            }
            else if (EnemyManager.Instance.GetTotalEnemiesDeregistered() <= 8)
            {
                audioManager.PlayNarration(NotBadRecruit);
                NextState(NotBadRecruit.length + 1);
            }
            else
            {
                audioManager.PlayNarration(YouMustPlayALotOfFortnite);
                NextState(YouMustPlayALotOfFortnite.length + 1);
            }
            
            EnemyManager.Instance.ResetTotalEnemiesDeregistered();
        }
        else if (state == 6)
        {
            NextState(1);
            TVCamera.Instance.Screenoff();
        }
        else if (state == 7)
        {
            audioManager.PlayNarration(BaseWarning);
            NextState(3);
        }
        else if (state == 8) // Wave #1: Narration
        {
            NarrateSequenceAndNextState(NarrationSequences2);
            audioManager.ChangeBGM(BGM_Action);
            audioManager.StartBGM();
        }
        else if (state == 9) // Wave #1: Attack Helicopters appear
        {
            ActivateSpawner(attackHelicopterEnemySpawnerDolly1, 0);
            ActivateSpawner(attackHelicopterEnemySpawnerDolly2, 7);
            ActivateSpawner(attackHelicopterEnemySpawnerDolly3, 14);
            NextState(0);
        }
        else if (state == 10)
        {
            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() == 15)
            {
                NextState(0);
            }
        }
        else if (state == 11)
        {
            EnemyManager.Instance.ResetTotalEnemiesDeregistered();
            NarrateSequenceAndNextState(NarrationSequences3); //"Sir the enemy has regrouped..."
        }
        else if (state == 12) // Wave #2: Spitfires appear
        {
            ActivateSpawner(spitfireEnemySpawnerDolly1, 0);
            ActivateSpawner(spitfireEnemySpawnerDolly2, 1);
            ActivateSpawner(attackHelicopterEnemySpawnerDolly4, 13);
            ActivateSpawner(attackHelicopterEnemySpawnerDolly5, 12);
            ActivateSpawner(attackHelicopterEnemySpawnerDolly6, 10);
            NextState(0);
        }
        else if (state == 13) // Wave #2: Waiting for the Player to defeat all the targets
        {
            if (EnemyManager.Instance.GetTotalEnemiesDeregistered() == 10)
            {
                audioManager.ChangeBGM(BGM_Boss);
                audioManager.StartBGM();
                NarrateSequenceAndNextState(NarrationSequences4);
            }
        }
        else if (state == 14) // Wave #2: Spitfires appear
        {
            ActivateSpawner(zeppelin, 0);
            NextState(0);
        }
        else if (state == 15) // Wave #2: Waiting for the Player to defeat all the targets
        {

        }


        else if (state == 20)
        {
            playerStatistics.SetActive(true);
            thanksForPlayingOurDemo.SetActive(true);
        }
    }

    private void ActivateSpawner(GameObject spawner, float time)
    {
        StartCoroutine(ActivateSpawnerInTime(spawner, time));
    }

    IEnumerator ActivateSpawnerInTime(GameObject spawner, float time)
    {
        yield return new WaitForSeconds(time);
        spawner.SetActive(true);
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

    private void GotoState(int newState, float time)
    {
        StartCoroutine(NextStateInTime(newState, time));
    }

    IEnumerator NextStateInTime(int newState, float time)
    {
        yield return new WaitForSeconds(time);
        state = newState;
        UpdateState();
    }
}
