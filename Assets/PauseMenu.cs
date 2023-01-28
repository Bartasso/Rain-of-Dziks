using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    private StarterAssetsControlls _starterAssets;
    


    private void Awake()
    {
        _starterAssets = new StarterAssetsControlls();
    }

    private void OnEnable()
    {
        _starterAssets.Enable();
    }

    private void OnDisable()
    {
        _starterAssets.Disable();
    }

    private void Update()
    {
        var isEscapePressed = _starterAssets.Player.Escape.triggered;
        
        if (gameIsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        if (isEscapePressed)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
}