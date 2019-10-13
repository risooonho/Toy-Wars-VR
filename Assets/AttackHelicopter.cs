﻿using System.Collections;
using UnityEngine;

public class AttackHelicopter : MonoBehaviour, IEnemy
{
    private float life = 3;
    private bool isShaking = false;
    private Material originalMaterial;
    private Material material;
    [SerializeField] private Material red;
    private MeshRenderer meshRenderer;

    private GameObject target;
    private int state = 0;
    // 0 == acquireTarget
    // 1 == moveToTarget
    // 2 == fireOnTarget

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    private void Update()
    {
        if (state == 0)
        {
            target = BaseAssetManager.Instance.GetTopBaseAsset();
            if (target != null)
            {
                state = 1;
            }
        }
        else if (state == 1)
        {
            var distance = Vector3.Distance(transform.position, target.transform.position);
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if (distance > 100)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 50);
            }
            else
            {
                state = 2;
            }
        }
        else if (state == 2)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            Debug.Log("firing on target");
        }
    }

    public void DamageEnemy(Vector3 position)
    {
        life--;
        var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.CFX_Explosion_B_Smoke_Text);
        explosion.transform.position = position;
        explosion.SetActive(true);

        if (life <= 0)
        {
            DestroySelf();
            EnemyManager.Instance.DeregisterEnemy();
        }
        else
        {
            if (!isShaking)
            {
                isShaking = true;
                StartCoroutine(DamageShake());
                StartCoroutine(FlashRed());
            }
        }
    }

    IEnumerator DamageShake()
    {
        Vector3 oldPosition = transform.localPosition;
        transform.localPosition = transform.localPosition + Random.insideUnitSphere * 2;

        while (Vector3.Distance(transform.localPosition, oldPosition) > 0.1)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, oldPosition, Time.deltaTime * 20);
            yield return null;
        }
        transform.localPosition = oldPosition;
        isShaking = false;
    }

    IEnumerator FlashRed()
    {
        meshRenderer.material = red;
        yield return new WaitForSeconds(0.05f);
        meshRenderer.material = originalMaterial;
    }

    private void DestroySelf()
    {
        gameObject.layer = LayerMask.NameToLayer("DyingEnemy");
        var rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.AddRelativeTorque(new Vector3(Random.Range(1, 3), Random.Range(-3, 3), Random.Range(1, 2)), ForceMode.Impulse);
        var smoke = ObjectPool.Instance.GetFromPoolInactive(Pools.Smoke);
        smoke.transform.position = transform.position;
        smoke.transform.SetParent(transform);
        smoke.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DyingEnemy") && collision.gameObject.layer == LayerMask.NameToLayer("Statics"))
        {
            var explosion = ObjectPool.Instance.GetFromPoolInactive(Pools.Large_CFX_Explosion_B_Smoke_Text);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            ObjectPool.Instance.DeactivateAndAddToPool(gameObject);
        }
    }
}
