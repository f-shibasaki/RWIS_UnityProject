using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField]
    private HorizontalTutorialPanel _horizontalMovePanel;

    [SerializeField]
    private RotationTutorialPanel _rotationPanel;

    public Action FinishHorizontalTutorialDelegate { get; set; }
    public Action FinishTutorialDelegate { get; set; }

    void Awake()
    {
        _horizontalMovePanel.OnFinish = OnFinishHorizontalTutorial;
        _rotationPanel.OnFinish = OnFinishTutorial;
        
        DisableAllPanels();
        _horizontalMovePanel.gameObject.SetActive(true);
    }

    void OnFinishHorizontalTutorial()
    {
        _horizontalMovePanel.gameObject.SetActive(false);
        
        FinishHorizontalTutorialDelegate.Invoke();
        
        _rotationPanel.gameObject.SetActive(true);
    }
    
    void OnFinishTutorial()
    {
        _rotationPanel.gameObject.SetActive(false);
        
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
    }
}
