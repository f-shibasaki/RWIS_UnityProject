using System;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    [SerializeField]
    private Button _playButton;
    
    [SerializeField]
    private Button _tutorialButton;

    // クリックした時に実行する関数
    public Action OnPlayGame { get; set; }
    public Action OnStartTutorial { get; set; }

    void OnEnable()
    {
        RemoveAllListeners();
        _playButton.onClick.AddListener(() =>
        {
            OnPlayGame.Invoke();
            Debug.Log("Play");
        });
        
        _tutorialButton.onClick.AddListener(() =>
        {
            OnStartTutorial.Invoke();
        });
    }
    
    void OnDisable()
    {
        RemoveAllListeners();
    }
    
    void RemoveAllListeners()
    {
        _playButton.onClick.RemoveAllListeners();
        _tutorialButton.onClick.RemoveAllListeners();
    }
}
