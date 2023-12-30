using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    private Button _homeButton;
    [SerializeField]
    private Button _retryButton;
    [SerializeField]
    private Button _resumeButton;
    

    public Action OnClickHomeButton { get; set; }

    public Action OnClickRetryButton { get; set; }
    public Action OnClickResumeButton { get; set; }

    void OnEnable()
    {
        _homeButton.onClick.AddListener(() =>
        {
            OnClickHomeButton.Invoke();
        });
        _retryButton.onClick.AddListener(() =>
        {
            OnClickRetryButton.Invoke();
        });
        _resumeButton.onClick.AddListener(() =>
        {
            OnClickResumeButton.Invoke();
        });
    }
    
    void OnDisable()
    {
        _homeButton.onClick.RemoveAllListeners();
        _retryButton.onClick.RemoveAllListeners();
        _resumeButton.onClick.RemoveAllListeners();
    }
}
