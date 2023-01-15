using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using Cinemachine;
using Scriptable_Objects;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderMask;
    [SerializeField] private WeaponScriptableObject gunScriptableObject;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject gunObject;

    public TextMeshProUGUI ammoDisplay;
    private int _ammoCountCurrent;
    private const float TimeAfterShootStart = 5f;
    private float _timeAfterShootLeft = 0f;
    private bool _shooting;
    private bool _reloading;
    private StarterAssetsInputs _starterAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private Animator _animator;
    private float _nextFire;
    
    private void Awake()
    {
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        _ammoCountCurrent = gunScriptableObject.maxAmmo;
    }

    private void Update()
    {
        ammoDisplay.text = _ammoCountCurrent + "/" + gunScriptableObject.maxAmmo;
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
            gunObject.transform.localPosition =
                Vector3.Lerp(gunObject.transform.localPosition, gunScriptableObject.firePosition, Time.deltaTime * 10f);
            gunObject.transform.localRotation = Quaternion.Lerp(gunObject.transform.localRotation,
                Quaternion.Euler(gunScriptableObject.fireRotation), Time.deltaTime * 10f);
        }
        else
        {
            gunObject.transform.localPosition =
                Vector3.Lerp(gunObject.transform.localPosition, gunScriptableObject.idlePosition, Time.deltaTime * 10f);
            gunObject.transform.localRotation = Quaternion.Lerp(gunObject.transform.localRotation,
                Quaternion.Euler(gunScriptableObject.idleRotation), Time.deltaTime * 10f);
        }

        _animator.SetLayerWeight(1, _shooting
            ? Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f)
            : Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
    }

    private void Fire(Vector3 mouseWorldPosition)
    {
        if (Time.fixedTime > gunScriptableObject.fireRate + _nextFire)
        {
            _timeAfterShootLeft = TimeAfterShootStart;
            _thirdPersonController.SetRotateOnMove(false);
            var position = spawnBulletPosition.position;
            Vector3 aimDir = (mouseWorldPosition - position).normalized;
            Transform bulletTransform = Instantiate(gunScriptableObject.bulletObject.transform, position,
                Quaternion.LookRotation(aimDir, Vector3.up));
            bulletTransform.GetComponent<BulletProjectile>().Setup(aimDir,gunScriptableObject.bulletSpeed, gunScriptableObject.damage);
            _shooting = true;
            _nextFire = Time.fixedTime;
            _ammoCountCurrent--;
            Instantiate(gunScriptableObject.muzzlePrefab, spawnBulletPosition.transform);
            
            if (gunScriptableObject.projectileToDisableOnFire != null)
            {
                gunScriptableObject.projectileToDisableOnFire.SetActive(false);
                Invoke(nameof(ReEnableDisabledProjectile), 3);
            }
            
            if (audioSource != null)
            {
                if (audioSource.transform.IsChildOf(transform))
                {
                    audioSource.Play();
                }
                else
                {
                    var newAS = Instantiate(audioSource);
                    if ((newAS = Instantiate(audioSource)) != null && newAS.outputAudioMixerGroup != null &&
                        newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(gunScriptableObject.audioPitch.x, gunScriptableObject.audioPitch.y));
                        newAS.pitch = Random.Range(gunScriptableObject.audioPitch.x, gunScriptableObject.audioPitch.y);
                        newAS.PlayOneShot(gunScriptableObject.gunShotClip);
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }
            
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
        _ammoCountCurrent = gunScriptableObject.maxAmmo;
    }
    
    private void ReEnableDisabledProjectile()
    {
        gunScriptableObject.projectileToDisableOnFire.SetActive(true);
    }
}