﻿using UnityEngine;


public class ChinookSpawner : MonoBehaviour
{
    private AudioManager audioManager;
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.LoadClip(@"Audio\SFX\helicopter_idle");

        for (int i = 0; i < 10; i++)
        {
            var newChinook = ObjectPool.Instance.GetFromPoolActiveSetTransform(Pools.EnemyChinook, transform);
            newChinook.transform.position = transform.position + new Vector3(Random.Range(-80, 80), Random.Range(10, 100), Random.Range(-80, 80));
            newChinook.transform.SetParent(transform);
            EnemyManager.Instance.RegisterEnemy(newChinook);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 20);
    }
}
