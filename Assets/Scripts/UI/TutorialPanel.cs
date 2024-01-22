using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    private Button _finishButton;
    
    public Action OnFinish { get; set; }

    void OnEnable()
    {
        RemoveAllListeners();
        _finishButton.onClick.AddListener(() =>
        {
            OnFinish.Invoke();
        });
    }
    
    void OnDisable()
    {
        RemoveAllListeners();
    }
    
    void RemoveAllListeners()
    {
        _finishButton.onClick.RemoveAllListeners();
    }
}
