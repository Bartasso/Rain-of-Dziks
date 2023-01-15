using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using Suriyun;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    [SerializeField] private GameObject player;
    [SerializeField] private AudioSource audioSource;
    
    private int _enemyIdleAnimationID = 1;
    private int _enemyMoveAnimationID = 2;
    private int _enemyAttackAnimationID = 3;
    private int _enemyDamageAnimationID = 4;
    private int _enemyDieAnimationID = 5;
    private AnimatorController _animatorController;
    private Animator _animator;
    
    private float _currentHealth;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animatorController = GetComponent<AnimatorController>();
    }

    private void Start()
    {
        _currentHealth = enemyScriptableObject.health;
        AssignAnimationIDs();
        StartCoroutine(EnemyIdle());
    }

    IEnumerator EnemyIdle()
    {
        _animator.SetBool(_enemyIdleAnimationID, true);
        while (Vector3.Distance(transform.position, player.transform.position) > enemyScriptableObject.engageDistance)
        {
            if (_currentHealth <= 0)
            {
                _animator.SetTrigger(_enemyDieAnimationID);
                yield return new WaitForSeconds(2.5f);
                Destroy(this.gameObject);
            }
            yield return null;
        }

        _animator.SetBool(_enemyIdleAnimationID, false);
        StartCoroutine(EnemyMoveToPlayer());
    }


    IEnumerator EnemyMoveToPlayer()
    {
        _animator.SetBool(_enemyMoveAnimationID, true);
        var move = false;
        while (Vector3.Distance(transform.position, player.transform.position) > enemyScriptableObject.attackRange)
        {
            if (_currentHealth <= 0)
            {
                _animator.SetTrigger(_enemyDieAnimationID);
                yield return new WaitForSeconds(1);
                Destroy(this.gameObject);
            }
            
            Vector3 destination = Vector3.MoveTowards(transform.position, player.transform.position,
                enemyScriptableObject.moveSpeed * Time.deltaTime);
            destination.y = transform.position.y;
            transform.position = destination;
            transform.LookAt(player.transform);
            if (Vector3.Distance(transform.position, player.transform.position) > enemyScriptableObject.engageDistance)
            {
                move = true;
                break;
            }

            yield return null;
        }

        _animator.SetBool(_enemyMoveAnimationID, false);
        StartCoroutine(move ? EnemyIdle() : EnemyAttack());
    }

    IEnumerator EnemyAttack()
    {
        if (_currentHealth <= 0)
        {
            _animator.SetTrigger(_enemyDieAnimationID);
            yield return new WaitForSeconds(1);
            Destroy(this.gameObject);
        }
        _animator.SetTrigger(_enemyAttackAnimationID);
        yield return new WaitForSeconds(enemyScriptableObject.attackSpeed); // timer that waits attacks but enables movement
        StartCoroutine(EnemyMoveToPlayer());
    }

    private void AssignAnimationIDs()
    {
        _enemyIdleAnimationID = Animator.StringToHash("Idle");
        _enemyMoveAnimationID = Animator.StringToHash("Move");
        _enemyAttackAnimationID = Animator.StringToHash("Attacked");
        _enemyDamageAnimationID = Animator.StringToHash("Damaged");
        _enemyDieAnimationID = Animator.StringToHash("Dead");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            _currentHealth -= other.gameObject.GetComponent<BulletProjectile>().bulletDamage;
        }
    }
}
