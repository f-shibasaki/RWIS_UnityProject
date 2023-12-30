using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private HomePanel _homePanel;

    [SerializeField]
    private GamePanel _gamePanel;
    
    [SerializeField]
    private PausePanel _pausePanel;
    
    [SerializeField]
    private GameOverPanel _gameOverPanel;
    
    // [SerializeField]
    // private GameObject _tutorialPanel;

    public Action StartGameDelegate { get; set; }
    public Action PauseGameDelegate { get; set; }
    public Action ResumeGameDelegate { get; set; }
    public Action ResetGyroDelegate { get; set; }

    void Awake()
    {
        _homePanel.OnPlayGame = OnStartGame;
        _homePanel.OnStartTutorial = OnStartTutorial;
        
        _gamePanel.OnPauseGame = OnPauseGame;

        _pausePanel.OnClickResumeButton = OnResumeGame;
        _pausePanel.OnClickRetryButton = OnStartGame;
        _pausePanel.OnClickHomeButton = OnReturnHome;
        _pausePanel.OnClickResetGyroButton = OnResetGyro;
        
        _gameOverPanel.OnRestartGame = OnStartGame;
        _gameOverPanel.OnReturnHome = OnReturnHome;
        
        DisableAllPanels();
        _homePanel.gameObject.SetActive(true);
    }

    public void OnStartGame()
    {
        DisableAllPanels();
        
        StartGameDelegate.Invoke();
        
        _gamePanel.gameObject.SetActive(true);
    }
    
    public void OnGameOver()
    {
        _gamePanel.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(true);
    }
    
    public void OnStartTutorial()
    {
        
    }
    
    public void OnReturnHome()
    {
        DisableAllPanels();
        _homePanel.gameObject.SetActive(true);
    }

    public void OnPauseGame()
    {
        PauseGameDelegate.Invoke();
        
        _gamePanel.gameObject.SetActive(false);
        _pausePanel.gameObject.SetActive(true);
    }
    
    public void OnResumeGame()
    {
        ResumeGameDelegate.Invoke();
        
        _pausePanel.gameObject.SetActive(false);
        _gamePanel.gameObject.SetActive(true);
    }
    
    public void OnResetGyro()
    {
        ResetGyroDelegate.Invoke();
    }

    private void DisableAllPanels()
    {
        _homePanel.gameObject.SetActive(false);
        _gamePanel.gameObject.SetActive(false);
        _pausePanel.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);
        // _tutorialPanel.SetActive(false);
    }
}
