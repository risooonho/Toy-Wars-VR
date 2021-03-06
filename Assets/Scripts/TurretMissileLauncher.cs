﻿using UnityEngine;
using Valve.VR;

public class TurretMissileLauncher : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    private float timeBetweenMissiles = 1;
    private float lastTimeFired;

    [SerializeField] private GameObject missileLauncherTip = default;
    [SerializeField] private GameObject missileLauncherAimingPoint = default;


    void Update()
    {
        GetVRInput();
    }

    private void GetVRInput()
    {
        if (fireAction.state)
        {
            if (Time.time > lastTimeFired)
            {
                lastTimeFired = Time.time + timeBetweenMissiles;
                SpawnMissile();
            }
        }
    }

    private void SpawnMissile()
    {
        var turretMissile = ObjectPool.Instance.GetFromPoolInactive(Pools.TurretMissile);
        turretMissile.GetComponent<TurretMissile>().Init(EnemyManager.Instance.GetNearestEnemy(transform.position), missileLauncherTip.transform.position, missileLauncherAimingPoint.transform.position - missileLauncherTip.transform.position);
        turretMissile.SetActive(true);
    }
}
