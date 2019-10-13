﻿using UnityEngine;

public class BaseAsset : MonoBehaviour, IBaseAsset
{
    [SerializeField] private float life;

    public void TakeDamage(Vector3 position)
    {
        life--;
        var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.CFX_Explosion_B_Smoke_Text);
        explosion.transform.position = position;
        explosion.SetActive(true);

        if (life <= 0)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        var smoke = ObjectPool.Instance.GetFromPoolInactive(Pools.Smoke);
        smoke.transform.position = transform.position;
        smoke.SetActive(true);
        BaseAssetManager.Instance.DeregisterBaseAsset(gameObject);
        Destroy(gameObject);
    }
}