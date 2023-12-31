using System;
using UnityEngine;
using UnityEngine.UI;

public class RotationTutorialPanel : MonoBehaviour
{
    [SerializeField]
    private Button _finishButton;
    

    // クリックした時に実行する関数
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
