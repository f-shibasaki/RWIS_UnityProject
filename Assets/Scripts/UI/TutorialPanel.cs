using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    private Button _finishButton;

    [SerializeField] 
    private Button _backButton;
    
    public Action OnBack { get; set; }
    public Action OnFinish { get; set; }

    void OnEnable()
    {
        RemoveAllListeners();
        _finishButton.onClick.AddListener(() =>
        {
            OnFinish.Invoke();
        });
        
        if (_backButton == null) return;
        _backButton.onClick.AddListener(() =>
        {
            OnBack.Invoke();
        });
    }
    
    void OnDisable()
    {
        RemoveAllListeners();
    }
    
    void RemoveAllListeners()
    {
        _finishButton.onClick.RemoveAllListeners();
        if (_backButton == null) return;
        _backButton.onClick.RemoveAllListeners();
    }
}
