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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animatorController = GetComponent<AnimatorController>();
    }

    private void Start()
    {
        AssignAnimationIDs();
        StartCoroutine(EnemyIdle());
    }

    IEnumerator EnemyIdle()
    {
        Debug.Log("Idle");
        _animator.SetBool(_enemyIdleAnimationID, true);
        while (Vector3.Distance(transform.position, player.transform.position) > enemyScriptableObject.engageDistance)
        {
            yield return null;
        }
        _animator.SetBool(_enemyIdleAnimationID, false);
        StartCoroutine(EnemyMoveToPlayer());
    }
    
    
    IEnumerator EnemyMoveToPlayer()
    {
        Debug.Log("Move");
        _animator.SetBool(_enemyMoveAnimationID, true);
        var move = false;
        while (Vector3.Distance(transform.position, player.transform.position) > enemyScriptableObject.attackRange)
        {

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
        Debug.Log("Attack");
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
    
}
