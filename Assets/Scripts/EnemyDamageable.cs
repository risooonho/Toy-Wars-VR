﻿using System.Collections;
using UnityEngine;

public class EnemyDamageable : MonoBehaviour, IEnemy
{
    [SerializeField] private float life = default;

    [SerializeField] private MeshRenderer[] meshRenderers = default;
    [SerializeField] private Material[] originalMaterials = default;
    [SerializeField] private Material flashRed = default;
    [SerializeField] private GameObject[] blownOffParts = default;
    [SerializeField] private GameObject firePoint = default;

    private bool vulnerability = true;

    private void Awake()
    {
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].sharedMaterials[0];
        }
    }

    public void Init()
    {
        
    }

    public void DamageEnemy(Vector3 position)
    {
        if (life <= 0)
            return;

        life--;
        var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.CFX_Explosion_B_Smoke_Text);
        explosion.transform.GetComponent<Explosion>().Init(transform.position);
        explosion.transform.position = position;
        explosion.SetActive(true);

        if (life <= 0)
        {
            Zeppelin.Instance.PartDestroyed(gameObject);
            DestroySelf();
        }
        else
        {
            StartCoroutine(DamageFlash());
        }
        Zeppelin.Instance.TakeDamage();
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].sharedMaterial = flashRed;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].sharedMaterial = originalMaterials[i];
        }
    }

    public void DestroyEnemy()
    {
    }

    virtual public void DestroySelf()
    {
        for (int i = 0; i < blownOffParts.Length; i++)
        {
            blownOffParts[i].SetActive(false);
        }
        var fire = ObjectPool.Instance.GetFromPoolInactive(Pools.CFX4Fire);
        fire.transform.SetParent(transform);
        fire.transform.position = transform.position;
        fire.SetActive(true);

        gameObject.layer = LayerMask.NameToLayer("DyingEnemy");
        var smoke = ObjectPool.Instance.GetFromPoolInactive(Pools.Smoke);
        smoke.transform.position = transform.position;
        smoke.transform.SetParent(transform);
        smoke.SetActive(true);
    }

    public bool IsVulnerable()
    {
        return vulnerability;
    }

    public void SetVulnerability(bool value)
    {
        vulnerability = value;
    }
}
