using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody _bulletRigidbody;
    [SerializeField] private Transform vfxNotEnemyHit;
    [SerializeField] private Transform vfxNormalHit;
    [SerializeField] private Transform vfxCriticalHit;
    public float speed = 50f;
    private Vector3 _shootDir;
    private float _bulletSpeed;

    private void Awake()
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir, float bulletSpeed)
    {
        _shootDir = shootDir;
        _bulletSpeed = bulletSpeed;

    }

    private void FixedUpdate()
    {
        transform.position += _shootDir * (_bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("Weapon")) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(vfxNormalHit, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(vfxNotEnemyHit, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}