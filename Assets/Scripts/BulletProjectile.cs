using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody _bulletRigidbody;
    [SerializeField] private Transform vfxNotEnemyHit;
    [SerializeField] private Transform vfxNormalHit;
    [SerializeField] private Transform vfxCriticalHit;
    [SerializeField] public float bulletDamage;
    [SerializeField] public float bulletSpeed;

    private Vector3 _shootDir;
    private float _maxDistance = 200;

    private void Awake()
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir, float bullet_Speed, float bullet_Damage)
    {
        _shootDir = shootDir;
        bulletSpeed = bullet_Speed;
        bulletDamage = bullet_Damage;
    }

    private void FixedUpdate()
    {
        transform.position += _shootDir * (bulletSpeed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x) > _maxDistance || Mathf.Abs(transform.position.y) > _maxDistance || Mathf.Abs(transform.position.z) > _maxDistance)
        {
            Destroy(this.gameObject);
        }
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