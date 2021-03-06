﻿using System.Collections;
using UnityEngine;
using Cinemachine;

public class Spitfire : MonoBehaviour, IEnemy
{
    private float maxLife = 3;
    private float currLife;
    private Material originalMaterial;
    private Material material;
    [SerializeField] private Material red = default;
    [SerializeField] private GameObject target;
    [SerializeField] private Transform barrel;
    private MeshRenderer meshRenderer;
    private Rigidbody rigidBody;
    private GameObject smoke;
    private Vector3 pos0;
    private Vector3 pos1;
    private AudioManager audioManager;
    private CinemachineDollyCart cinemachineDollyCart;
    private int sourceKey = -1;
    private int cannonSource = -1;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        cinemachineDollyCart = GetComponent<CinemachineDollyCart>();
        audioManager = AudioManager.GetAudioManager();
    }

    public void Init()
    {
        BindAudio();
        currLife = maxLife;
        FireWeapon(Random.Range(8, 10), 5);
    }

    private void BindAudio()
    {
        if (audioManager != null)
        {
            sourceKey = audioManager.ReserveSource("engine_generator_loop_03", occluding: true, spacial_blend: 1f, pitch: 1f, looping: true);
            audioManager.SetReservedMixer(sourceKey, 3);
            audioManager.BindReserved(sourceKey, transform);
            audioManager.PlayReserved(sourceKey);

            cannonSource = audioManager.ReserveSource("big one", occluding: true, spacial_blend: 1.0f, pitch: 1.0f);
            audioManager.BindReserved(cannonSource, transform);
        }
    }

    private void UnbindAudio()
    {
        if (audioManager != null)
        {
            audioManager.UnbindReserved(sourceKey);
            sourceKey = -1;
            audioManager.UnbindReserved(cannonSource);
            cannonSource = -1;
        }
    }

    private void Update()
    {
        StorePositions();
        CheckReachedEndOfPath();
    }

    private void CheckReachedEndOfPath()
    {
        if (cinemachineDollyCart.m_Position > cinemachineDollyCart.m_Path.PathLength - 1)
        {
            UnbindAudio();
            EnemyManager.Instance.DeregisterEnemyNoPoints(gameObject);
            ObjectPool.Instance.DeactivateAndAddToPool(gameObject);
        }
    }

    public void DamageEnemy(Vector3 position)
    {
        if (currLife <= 0)
            return;

        currLife--;
        var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.CFX_Explosion_B_Smoke_Text);
        explosion.transform.GetComponent<Explosion>().Init(position);
        explosion.SetActive(true);

        if (currLife <= 0)
        {
            EnemyManager.Instance.DeregisterEnemyWithPoints(gameObject);
            DestroySelf();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        meshRenderer.material = red;
        yield return new WaitForSeconds(0.05f);
        meshRenderer.material = originalMaterial;
    }

    private void StorePositions()
    {
        pos0 = pos1;
        pos1 = transform.position;
    }

    private void DestroySelf()
    {
        gameObject.layer = LayerMask.NameToLayer("DyingEnemy");
        cinemachineDollyCart.enabled = false;
        rigidBody.isKinematic = false;
        rigidBody.velocity = (pos1 - pos0) / Time.deltaTime;
        rigidBody.AddRelativeTorque(new Vector3(Random.Range(1, 3), Random.Range(-3, 3), Random.Range(1, 2)), ForceMode.Impulse);
        smoke = ObjectPool.Instance.GetFromPoolInactive(Pools.Smoke);
        smoke.transform.position = transform.position;
        smoke.transform.SetParent(transform);
        smoke.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DyingEnemy") && collision.gameObject.layer == LayerMask.NameToLayer("Statics"))
        {
            UnbindAudio();
            var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.Large_CFX_Explosion_B_Smoke_Text);
            explosion.transform.GetComponent<Explosion>().Init(transform.position);
            explosion.SetActive(true);
            ObjectPool.Instance.DeactivateAndAddToPool(smoke);
            ObjectPool.Instance.DeactivateAndAddToPool(gameObject);
        }
    }

    public void FireWeapon(float time, int repeat)
    {
        StartCoroutine(FireWeaponInTime(time, repeat));
    }

    IEnumerator FireWeaponInTime(float time, int repeat)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < repeat; i++)
        {
            audioManager.PlayReserved(cannonSource);
            var enemyBullet = ObjectPool.Instance.GetFromPoolInactive(Pools.YellowLaserMissile);
            enemyBullet.GetComponent<LaserProjectile>().Init(barrel, target.transform.position - transform.position);
            enemyBullet.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DestroyEnemy()
    {
    }

    public bool IsVulnerable()
    {
        return true;
    }

    public void SetVulnerability(bool value)
    {
    }
}