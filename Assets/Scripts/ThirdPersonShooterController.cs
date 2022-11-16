using System;
using System.Collections;
using System.Collections.Generic;
using BigRookGames.Weapons;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderMask;
    [SerializeField] private Transform bulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Vector3 FirePosition;
    [SerializeField] private Vector3 FireRotation;
    [SerializeField] private Vector3 IdlePosition;
    [SerializeField] private Vector3 IdleRotation;

    [SerializeField] private GameObject GunObject;
    // [SerializeField] private Transform debugTransform;
    public TextMeshProUGUI ammoDisplay;
    
    [Header("Weapon Stats")] public bool isBlast = false;
    public float blastRadius = 0;
    public float fireRate = 0.001f;
    public float accuracy = 1;
    public float damage = 10;
    public int ammoCountMax = 30;
    
    private int _ammoCountCurrent;
    private const float TimeAfterShootStart = 5f;
    private float _timeAfterShootLeft = 0f;
    private bool _shooting;
    private bool _reloading;
    private StarterAssetsInputs _starterAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private Animator _animator;
    private float _nextFire;
    
    void Awake()
    {
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        _ammoCountCurrent = ammoCountMax;
    }

    void Update()
    {
        ammoDisplay.text = _ammoCountCurrent + "/" + ammoCountMax;
        _reloading = _thirdPersonController.Reloading;
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
            {
                mouseWorldPosition = raycastHit.point;
            }
        }

        if (!_reloading)
        {
            if (_starterAssetsInputs.shoot)
            {
                Fire(mouseWorldPosition);
            }
        }

        if ((_ammoCountCurrent == 0 || _starterAssetsInputs.reload) && !_reloading)
        {
            Reload();
        }
        
        if (_timeAfterShootLeft > 0)
        {
            _timeAfterShootLeft -= Time.deltaTime;
            Vector3 worldAimTarget = mouseWorldPosition;
            var position = transform.position;
            worldAimTarget.y = position.y;
            Vector3 shootDirection = (worldAimTarget - position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, shootDirection, Time.deltaTime * 20f);
            if (_timeAfterShootLeft <= 0)
            {
                _thirdPersonController.SetRotateOnMove(true);
                _shooting = false;
            }
        }

        if (_shooting)
        {
            GunObject.transform.localPosition =
                Vector3.Lerp(GunObject.transform.localPosition, FirePosition, Time.deltaTime * 10f);
            GunObject.transform.localRotation = Quaternion.Lerp(GunObject.transform.localRotation,
                Quaternion.Euler(FireRotation), Time.deltaTime * 10f);
        }
        else
        {
            GunObject.transform.localPosition =
                Vector3.Lerp(GunObject.transform.localPosition, IdlePosition, Time.deltaTime * 10f);
            GunObject.transform.localRotation = Quaternion.Lerp(GunObject.transform.localRotation,
                Quaternion.Euler(IdleRotation), Time.deltaTime * 10f);
        }

        _animator.SetLayerWeight(1, _shooting
            ? Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f)
            : Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
    }

    private void Fire(Vector3 mouseWorldPosition)
    {
        if (Time.fixedTime > fireRate + _nextFire)
        {
            _timeAfterShootLeft = TimeAfterShootStart;
            _thirdPersonController.SetRotateOnMove(false);
            var position = spawnBulletPosition.position;
            Vector3 aimDir = (mouseWorldPosition - position).normalized;
            Instantiate(bulletProjectile, position, Quaternion.LookRotation(aimDir, Vector3.up));
            _shooting = true;
            GunObject.GetComponent<GunfireController>().FireWeapon();
            _nextFire = Time.fixedTime;
            _ammoCountCurrent--;
        }
    }

    private void Reload()
    {
        if (!_starterAssetsInputs.reload)
        {
            _starterAssetsInputs.reload = true;
        }
    }

    public void RefreshAmmo()
    {
        _ammoCountCurrent = ammoCountMax;
    }
}