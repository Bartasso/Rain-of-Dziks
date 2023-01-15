using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy Scriptable Object")]

    public class EnemyScriptableObject : ScriptableObject
    {
        public float health;
        public float damage;
        public bool isRanged;
        public float attackRange;
        public float engageDistance;
        public float armour;
        public float attackSpeed;
        public AudioClip attackSound;
        public GameObject projectile;
        public float moveSpeed;
    }
}
