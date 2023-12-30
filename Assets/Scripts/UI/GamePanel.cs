using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField]
    private Button _pauseButton;
    

    public Action OnPauseGame { get; set; }

    void OnEnable()
    {
        _pauseButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.AddListener(() =>
        {
            OnPauseGame.Invoke();
        });
    }
    
    void OnDisable()
    {
        _pauseButton.onClick.RemoveAllListeners();
    }
}
