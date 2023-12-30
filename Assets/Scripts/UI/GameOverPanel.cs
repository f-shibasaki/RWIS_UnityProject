using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField]
    private Button _restartButton;
    
    [SerializeField]
    private Button _homeButton;
    

    public Action OnRestartGame { get; set; }
    public Action OnReturnHome { get; set; }

    void OnEnable()
    {
        _restartButton.onClick.AddListener(() =>
        {
            OnRestartGame.Invoke();
        });
        
        _homeButton.onClick.AddListener(() =>
        {
            OnReturnHome.Invoke();
        });
    }
    
    void OnDisable()
    {
        _restartButton.onClick.RemoveAllListeners();
        _homeButton.onClick.RemoveAllListeners();
    }
}
