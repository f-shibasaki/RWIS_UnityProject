using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField]
    private TutorialPanel _horizontalMovePanel;
    [SerializeField]
    private TutorialPanel _rotationPanel;
    [SerializeField]
    private TutorialPanel _holdPanel;
    [SerializeField]
    private TutorialPanel _swipePanel;

    public Action FinishHorizontalTutorialDelegate { get; set; }
    public Action FinishRotationTutorialDelegate { get; set; }
    public Action FinishHoldTutorialDelegate { get; set; }
    public Action FinishTutorialDelegate { get; set; }

    void Awake()
    {
        _horizontalMovePanel.OnFinish = OnFinishHorizontalTutorial;
        _rotationPanel.OnFinish = OnFinishRotationTutorial;
        _holdPanel.OnFinish = OnFinishHoldTutorial;
        _swipePanel.OnFinish = OnFinishTutorial;

        DisableAllPanels();
        _horizontalMovePanel.gameObject.SetActive(true);
    }

    void OnFinishHorizontalTutorial()
    {
        _horizontalMovePanel.gameObject.SetActive(false);
        
        FinishHorizontalTutorialDelegate.Invoke();
        
        _rotationPanel.gameObject.SetActive(true);
    }
    
    void OnFinishRotationTutorial()
    {
        _rotationPanel.gameObject.SetActive(false);
        
        FinishRotationTutorialDelegate.Invoke();
        
        _holdPanel.gameObject.SetActive(true);
    }
    
    void OnFinishHoldTutorial()
    {
        _holdPanel.gameObject.SetActive(false);
        
        FinishHoldTutorialDelegate.Invoke();
        
        _swipePanel.gameObject.SetActive(true);
    }
    
    void OnFinishTutorial()
    {
        _swipePanel.gameObject.SetActive(false);
        
        FinishTutorialDelegate.Invoke();
    }
    
    public void OnStartTutorial()
    {
        _horizontalMovePanel.gameObject.SetActive(true);
    }
    
    public void DisableAllPanels()
    {
        _horizontalMovePanel.gameObject.SetActive(false);
        _rotationPanel.gameObject.SetActive(false);
        _holdPanel.gameObject.SetActive(false);
        _swipePanel.gameObject.SetActive(false);
    }
}
