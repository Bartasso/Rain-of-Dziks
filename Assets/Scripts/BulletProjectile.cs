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

    private void Awake()
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _bulletRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;
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