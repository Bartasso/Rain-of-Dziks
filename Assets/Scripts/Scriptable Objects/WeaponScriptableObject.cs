using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon Scriptable Object")]

    public class WeaponScriptableObject : ScriptableObject
    {
        [Header("Weapon Objects")] 
        public GameObject gunObject;
        public GameObject bulletObject;
        public GameObject muzzlePrefab;

        [Tooltip(
            "Sometimes a mesh will want to be disabled on fire. For example: when a rocket is fired, we instantiate a new rocket, and disable" +
            " the visible rocket attached to the rocket launcher")]
        public GameObject projectileToDisableOnFire;

        [Header("Audio")] 
        public AudioClip gunShotClip;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

        [Header("Weapon Stats")] 
        public float bulletSpeed;
        public float blastRadius;
        public float fireRate;
        public float accuracy;
        public float damage;
        public int maxAmmo;
        public float reloadTime;
    
        [Header("Weapon position")] 
        public Vector3 firePosition;
        public Vector3 fireRotation;
        public Vector3 idlePosition;
        public Vector3 idleRotation;

    }
}